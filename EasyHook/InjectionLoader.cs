/*
    EasyHook - The reinvention of Windows API hooking
 
    Copyright (C) 2009 Christoph Husse

    This library is free software; you can redistribute it and/or
    modify it under the terms of the GNU Lesser General Public
    License as published by the Free Software Foundation; either
    version 2.1 of the License, or (at your option) any later version.

    This library is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
    Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public
    License along with this library; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

    Please visit http://www.codeplex.com/easyhook for more information
    about the project and latest updates.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace EasyHook
{
#pragma warning disable 1591
#pragma warning disable 0618
#pragma warning disable 0028

  /// <summary>
  /// <see cref="InjectionLoader"/> initializes and runs the <see cref="IEntryPoint"/> of an injected library.
  /// </summary>
  public class InjectionLoader
  {

    #region Private Classes

    /// <summary>
    /// Struct to be marshalled from an unmanaged block of memory.
    /// The pointer to such a block of memory is typically provided by a parameter of the <see cref="InjectionLoader.Main"/> method.
    /// Contains the process ID of the host,
    /// and a pointer to a memory block containing an instance of <see cref="ManagedRemoteInfo"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    class REMOTE_ENTRY_INFO : RemoteHooking.IContext
    {
      public Int32 m_HostPID;
      public IntPtr UserData;
      public Int32 UserDataSize;

      #region IContext Members

      public Int32 HostPID { get { return m_HostPID; } }

      #endregion
    };

    /// <summary>
    /// Wraps the data needed for the connection to the host.
    /// </summary>
    private class HostConnectionData
    {

      #region Enums

      public enum ConnectionState
      {
        Invalid = 0,
        NoChannel = 1,
        Valid = Int32.MaxValue
      }

      #endregion

      #region Variables

      private REMOTE_ENTRY_INFO _unmanagedInfo;
      private HelperServiceInterface _helperInterface;
      private ManagedRemoteInfo _remoteInfo;
      private ConnectionState _state;

      #endregion

      #region Properties

      /// <summary>
      /// Gets the state of the current <see cref="HostConnectionData"/>.
      /// </summary>
      public ConnectionState State
      {
        get { return _state; }
      }

      /// <summary>
      /// Gets the unmanaged data containing the pointer to the memory block containing <see cref="RemoteInfo"/>;
      /// </summary>
      public REMOTE_ENTRY_INFO UnmanagedInfo
      {
        get { return _unmanagedInfo; }
      }

      public HelperServiceInterface HelperInterface
      {
        get { return _helperInterface; }
      }

      public ManagedRemoteInfo RemoteInfo
      {
        get { return _remoteInfo; }
      }

      #endregion

      #region Constructors

      private HostConnectionData()
      {
        _state = ConnectionState.Invalid;
        _helperInterface = null;
        _remoteInfo = null;
        _unmanagedInfo = null;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Loads <see cref="HostConnectionData"/> from the <see cref="IntPtr"/> specified.
      /// </summary>
      /// <param name="unmanagedInfoPointer"></param>
      public static HostConnectionData LoadData(IntPtr unmanagedInfoPointer)
      {
        var data = new HostConnectionData
        {
          _state = ConnectionState.Valid,
          _unmanagedInfo = new REMOTE_ENTRY_INFO()
        };
        try
        {
          /// Get the unmanaged data
          Marshal.PtrToStructure(unmanagedInfoPointer, data._unmanagedInfo);
          using (Stream passThruStream = new MemoryStream())
          {
            byte[] passThruBytes = new byte[data._unmanagedInfo.UserDataSize];
            BinaryFormatter format = new BinaryFormatter();
            Marshal.Copy(data._unmanagedInfo.UserData, passThruBytes, 0, data._unmanagedInfo.UserDataSize);
            passThruStream.Write(passThruBytes, 0, passThruBytes.Length);
            passThruStream.Position = 0;
            data._remoteInfo = (ManagedRemoteInfo)format.Deserialize(passThruStream);
          }
          /// Connect the HelperServiceInterface
          data._helperInterface = RemoteHooking.IpcConnectClient<HelperServiceInterface>(data._remoteInfo.ChannelName);
          // Ensure that the connection is working...
          data._helperInterface.Ping();
          if (!_connectedChannels.Contains(data._remoteInfo.ChannelName))
          {
            _connectedChannels.Add(data._remoteInfo.ChannelName);
            return new HostConnectionData { _state = ConnectionState.NoChannel };
          }
        }
        catch (Exception ExtInfo)
        {
          Config.PrintError(ExtInfo.ToString());
          return new HostConnectionData { _state = ConnectionState.Invalid };
        }
        return data;
      }

      #endregion

    }

    #endregion

    #region Variables

    private static readonly List<string> _connectedChannels = new List<string>();

    #endregion

    #region Public Methods

    public static int Main(string inParam)
    {
      if (inParam == null) return 0;
      var ptr = (IntPtr)Int64.Parse(inParam, System.Globalization.NumberStyles.HexNumber);
      var connection = HostConnectionData.LoadData(ptr);
      if (connection.State != HostConnectionData.ConnectionState.Valid)
        return (int)connection.State;
      // Adjust host PID in case of WOW64 bypass and service help...
      connection.UnmanagedInfo.m_HostPID = connection.RemoteInfo.HostPID;

      try
      {
        // Prepare parameter array.
        var paramArray = new object[1 + connection.RemoteInfo.UserParams.Length];
        paramArray[0] = (RemoteHooking.IContext)connection.UnmanagedInfo;
        for (int i = 0; i < connection.RemoteInfo.UserParams.Length; i++)
          paramArray[i + 1] = connection.RemoteInfo.UserParams[i];
        /// Load the user library and initialize the IEntryPoint.
        return LoadUserLibrary(connection.RemoteInfo.UserLibrary, paramArray, connection.HelperInterface);
      }
      catch (Exception ExtInfo)
      {
        Config.PrintWarning(ExtInfo.ToString());
        return -1;
      }
      finally
      {
        if (_connectedChannels.Contains(connection.RemoteInfo.ChannelName))
          _connectedChannels.Remove(connection.RemoteInfo.ChannelName);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Calls <see cref="LocalHook.Release"/> if an entrypoint has been initialized.
    /// </summary>
    /// <param name="entryPoint"></param>
    private static void Release(Type entryPoint)
    {
      if (entryPoint == null)
        return;
      LocalHook.Release();
    }

    /// <summary>
    /// Loads the user library,
    /// creates an instance for the <see cref="IEntryPoint"/> specified in the library
    /// and invokes the Run() method specified in that instance.
    /// </summary>
    /// <param name="userLibrary">The assembly provided by the user, located in the application directory or in the global assembly cache.</param>
    /// <param name="paramArray">Array of parameters to use with the constructor and with the Run() method.</param>
    /// <param name="helperServiceInterface"><see cref="HelperServiceInterface"/> to use for reporting to the host side.</param>
    /// <returns>The exit code to be returned by the main() method.</returns>
    private static int LoadUserLibrary(string userLibrary, object[] paramArray, HelperServiceInterface helperServiceInterface)
    {
      Type entryPoint = null;
      MethodInfo runMethod;
      object instance;

      try
      {
        entryPoint = FindEntryPoint(userLibrary);
        /// Determine if a Run() method is defined, before initializing an instance for the type.
        runMethod = FindMatchingMethod(entryPoint, "Run", paramArray);
        if (runMethod == null)
          throw new MissingMethodException(ConstructMissingMethodExceptionMessage("Run", paramArray));
        /// Initialize an object for the entrypoint.
        instance = InitializeInstance(entryPoint, paramArray);
        if (instance == null)
          throw new MissingMethodException(ConstructMissingMethodExceptionMessage(entryPoint.Name, paramArray));
        /// Notify the host about successful injection.
        helperServiceInterface.InjectionCompleted(RemoteHooking.GetCurrentProcessId());
      }
      catch (Exception e)
      {
        /// Provide error information on host side.
        try
        {
          helperServiceInterface.InjectionException(RemoteHooking.GetCurrentProcessId(), e);
        }
        finally
        {
          Release(entryPoint);
        }
        return -1;
      }

      try
      {
        /// After this it is safe to enter the Run() method, which will block until assembly unloading...
        /// From now on the user library has to take care about error reporting!
        runMethod.Invoke(instance, BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding |
                                   BindingFlags.InvokeMethod, null, paramArray, null);
      }
      finally
      {
        Release(entryPoint);
      }
      return 0;
    }

    /// <summary>
    /// Finds the <see cref="IEntryPoint"/> in the specified <see cref="Assembly"/>.
    /// </summary>
    /// <exception cref="EntryPointNotFoundException">
    /// An <see cref="EntryPointNotFoundException"/> is thrown if the given user library does not export a proper type implementing
    /// the <see cref="IEntryPoint"/> interface.
    /// </exception>
    /// <param name="userAssemblyPartialName">The partial name of the assembly provided by the user.</param>
    /// <returns>The <see cref="Type"/> functioning as <see cref="IEntryPoint"/> for the user provided <see cref="Assembly"/>.</returns>
    private static Type FindEntryPoint(string userAssemblyPartialName)
    {
      var userAssembly = Assembly.LoadWithPartialName(userAssemblyPartialName);
      var exportedTypes = userAssembly.GetExportedTypes();
      for (int i = 0; i < exportedTypes.Length; i++)
      {
        if (exportedTypes[i].GetInterface("EasyHook.IEntryPoint") != null)
          return exportedTypes[i];
      }
      throw new EntryPointNotFoundException("The given user library does not export a proper type implementing the 'EasyHook.IEntryPoint' interface.");
    }

    /// <summary>
    /// Finds a user defined Run() method in the specified <see cref="Type"/> matching the specified <paramref name="paramArray"/>.
    /// </summary>
    /// <param name="methodName">Name of the method to search.</param>
    /// <param name="objectType"><see cref="Type"/> to extract the method from.</param>
    /// <param name="paramArray">Array of parameters to match to the method's defined parameters.</param>
    /// <returns><see cref="MethodInfo"/> for the matching method, if any.</returns>
    private static MethodInfo FindMatchingMethod(Type objectType, string methodName, object[] paramArray)
    {
      var methods = objectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
      foreach (var method in methods)
      {
        if (method.Name == methodName
            && MethodMatchesParameters(method, paramArray))
          return method;
      }
      return null;
    }

    /// <summary>
    /// Initializes an instance from the specified <see cref="Type"/> using the specified <paramref name="parameters"/>.
    /// </summary>
    /// <param name="objectType"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private static object InitializeInstance(Type objectType, object[] parameters)
    {
      var constructors = objectType.GetConstructors();
      foreach (var constructor in constructors)
      {
        if (MethodMatchesParameters(constructor, parameters))
          return constructor.Invoke(parameters);
      }
      return null;
    }

    /// <summary>
    /// Returns whether the specified <paramref name="paramArray"/> can be used as parameters when invoking the specified <paramref name="method"/>.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="paramArray"></param>
    /// <returns></returns>
    private static bool MethodMatchesParameters(MethodBase method, object[] paramArray)
    {
      var parameters = method.GetParameters();
      if (parameters.Length != paramArray.Length) return false;
      for (int i = 0; i < paramArray.Length; i++)
      {
        if (!parameters[i].ParameterType.IsInstanceOfType(paramArray[i]))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Constructs a message for a <see cref="MissingMethodException"/> containing more specific information about the expected paramaters.
    /// </summary>
    /// <param name="methodName">Name of the missing method.</param>
    /// <param name="paramArray">Array of the expected parameters.</param>
    /// <returns></returns>
    private static string ConstructMissingMethodExceptionMessage(string methodName, object[] paramArray)
    {
      var msg = new StringBuilder("The given user library does not export a proper " + methodName + "(");
      foreach (var param in paramArray)
        msg.Append(param.GetType() + ", ");
      return msg.ToString(0, msg.Length - 2) + ") method in the 'EasyHook.IEntryPoint' interface.";
    }

    #endregion

  }
}
