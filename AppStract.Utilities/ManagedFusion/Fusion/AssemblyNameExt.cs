#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using AppStract.Utilities.Extensions;
using AppStract.Utilities.Helpers;

namespace AppStract.Utilities.ManagedFusion.Fusion
{
  /// <summary>
  /// Extension methods for <see cref="AssemblyName"/> and for <see cref="IAssemblyName"/>.
  /// </summary>
  internal static class AssemblyNameExt
  {

    #region Public Methods

    /// <summary>
    /// Converts the value of this instance to its equivalent <see cref="AssemblyName"/>.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static AssemblyName ToAssemblyName(this IAssemblyName assemblyName)
    {
      var result = new AssemblyName();
      result.Name = assemblyName.GetName();
      result.Version = assemblyName.GetVersion();
      result.CultureInfo = new CultureInfo(assemblyName.GetProperty<string>(AssemblyNamePropertyId.Culture));
      result.CodeBase = assemblyName.GetProperty<string>(AssemblyNamePropertyId.CodebaseUrl);
      result.SetPublicKey(assemblyName.GetProperty<byte[]>(AssemblyNamePropertyId.PublicKey));
      result.SetPublicKeyToken(assemblyName.GetProperty<byte[]>(AssemblyNamePropertyId.PublicKeyToken));
      // ToDo: The following line will always return null, why? And how to fix this?
      //  assemblyName.GetProperty<object>(AssemblyNamePropertyId.ProcessorIdArray);
      // A workaround is available by using the displayname of the IAssemblyName
      var tmp = assemblyName.GetDisplayName(DisplayNameFlags.ProcessArchitecture);
      tmp = tmp.Substring(tmp.LastIndexOf('=') + 1);
      ProcessorArchitecture architecture;
      if (ParserHelper.TryParseEnum(tmp, out architecture))
        result.ProcessorArchitecture = architecture;
      return result;
    }

    /// <summary>
    /// Converts the value of this instance to its equivalent <see cref="IAssemblyName"/>.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static IAssemblyName ToIAssemblyName(this AssemblyName assemblyName)
    {
      IAssemblyName result;
      AssemblyCache.CreateAssemblyNameObject(out result, assemblyName.FullName, CreateDisposition.ParseDisplayName, IntPtr.Zero);
      result.SetProperty(AssemblyNamePropertyId.CodebaseUrl, assemblyName.CodeBase);
      result.SetProperty(AssemblyNamePropertyId.Culture, assemblyName.CultureInfo == null ? null : assemblyName.CultureInfo.ToString());
      result.SetProperty(AssemblyNamePropertyId.MajorVersion, assemblyName.Version == null ? null : (object)(short)assemblyName.Version.Major);
      result.SetProperty(AssemblyNamePropertyId.MinorVersion, assemblyName.Version == null ? null : (object)(short)assemblyName.Version.Minor);
      result.SetProperty(AssemblyNamePropertyId.Name, assemblyName.Name);
      result.SetProperty(AssemblyNamePropertyId.PublicKey, assemblyName.GetPublicKey());
      result.SetProperty(AssemblyNamePropertyId.PublicKeyToken, assemblyName.GetPublicKeyToken());
      return result;
    }

    /// <summary>
    /// Returns the most specific available name, which is still compatible with the fusion API.
    /// </summary>
    /// <remarks>
    /// <see cref="AssemblyName.FullName"/> is not compatible with fusion because of "PublicKeyToken" being specified.
    /// </remarks>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public static string GetFusionCompatibleFullName(this AssemblyName assemblyName)
    {
      return assemblyName.Name
             + (assemblyName.Version == null ? "" : ", Version=" + assemblyName.Version)
             + (assemblyName.CultureInfo == null || string.IsNullOrEmpty(assemblyName.CultureInfo.Name)
                  ? ""
                  : ", Culture=" + assemblyName.CultureInfo.Name);
    }

    #endregion

    #region Private Methods - IAssemblyName

    private static string GetName(this IAssemblyName name)
    {
      uint bufferSize = 255;
      var buffer = new StringBuilder((int)bufferSize);
      name.GetName(ref bufferSize, buffer);
      return buffer.ToString();
    }

    private static string GetDisplayName(this IAssemblyName name, DisplayNameFlags which)
    {
      uint bufferSize = 255;
      var buffer = new StringBuilder((int)bufferSize);
      name.GetDisplayName(buffer, ref bufferSize, which);
      return buffer.ToString();
    }

    private static Version GetVersion(this IAssemblyName name)
    {
      uint versionHi, versionLow;
      name.GetVersion(out versionHi, out versionLow);
      return new Version((int) (versionHi >> 16), (int) (versionHi & 0xFFFF),
                         (int) (versionLow >> 16), (int) (versionLow & 0xFFFF));
    }

    private static T GetProperty<T>(this IAssemblyName name, AssemblyNamePropertyId propertyId)
    {
      uint bufferSize = 512;
      var bufferPointer = Marshal.AllocHGlobal((int)bufferSize);
      try
      {
        var hResult = name.GetProperty(propertyId, bufferPointer, ref bufferSize);
        if (!Microsoft.Win32.Interop.WinError.Succeeded(hResult))
          Marshal.ThrowExceptionForHR(hResult);
        return bufferSize > 0  // IAssemblyName.GetProperty() will always return a bufferSize greater than 0
                 ? bufferPointer.Read<T>(bufferSize)
                 : default(T);
      }
      finally
      {
        Marshal.FreeHGlobal(bufferPointer);
      }
    }

    private static void SetProperty(this IAssemblyName name, AssemblyNamePropertyId propertyId, object value)
    {
      int allocatedBytes = 0;
      var ptr = value == null ? IntPtr.Zero : value.ToPointer(out allocatedBytes);
      try
      {
        // First clear the property
        var hResult = name.SetProperty(propertyId, IntPtr.Zero, 0);
        Marshal.ThrowExceptionForHR(hResult);
        // Now set the property
        hResult = name.SetProperty(propertyId, ptr, (uint)allocatedBytes);
        Marshal.ThrowExceptionForHR(hResult);
      }
      finally
      {
        Marshal.FreeHGlobal(ptr);
      }
    }

    #endregion

  }
}
