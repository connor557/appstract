#region Copyright (C) 2008-2009 Simon Allaeys

/*
    Copyright (C) 2008-2009 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using AppStract.Core.System.Logging;
using AppStract.Server.Registry.Data;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Registry;
using Microsoft.Win32.Interop;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// Implementation of a virtualized Windows registry. <see cref="VirtualRegistry"/> functions as a switch between
  /// the caller and <see cref="VirtualRegistryData"/> &amp; <see cref="TransparentRegistry"/>.
  /// </summary>
  public class VirtualRegistry
  {

    #region Variables

    /// <summary>
    /// The virtual registry.
    /// </summary>
    private readonly VirtualRegistryData _virtualRegistry;
    /// <summary>
    /// Contains the open keys leading to hives that are not portable.
    /// </summary>
    private readonly TransparentRegistry _transparantRegistry;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the virtual Windows registry.
    /// </summary>
    public VirtualRegistry()
    {
      IndexGenerator indexGenerator = new IndexGenerator();
      /// Reserved indices for static virtual keys.
      indexGenerator.ExcludedRanges.Add(new IndexRange(0, 20));
      /// Reserved indices for registry rootkeys.
      indexGenerator.ExcludedRanges.Add(new IndexRange(0x80000000, 0x80000006));
      _virtualRegistry = new VirtualRegistryData(indexGenerator);
      _transparantRegistry = new TransparentRegistry(indexGenerator);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the <see cref="VirtualRegistry"/>.
    /// Must be called before any other method is called,
    /// in order for the <see cref="VirtualRegistry"/>-object to function properly.
    /// </summary>
    /// <param name="dataSource">The <see cref="IRegistryLoader"/> to request the initialization data from.</param>
    public void Initialize(IRegistryLoader dataSource)
    {
      _virtualRegistry.LoadData(dataSource);
    }

    /// <summary>
    /// Tries to open the key with the name specified.
    /// </summary>
    /// <param name="keyName">The name of the key to open.</param>
    /// <param name="hResult">The open handle for the opened key.</param>
    /// <returns>Whether the key could be opened.</returns>
    public bool OpenKey(string keyName, out uint hResult)
    {
      var accessMechanism = RegistryHelper.DetermineAccessMechanism(keyName);
      return accessMechanism == AccessMechanism.Transparent
        ? _transparantRegistry.OpenKey(keyName, out hResult)
        : _virtualRegistry.OpenKey(keyName, out hResult);
    }

    /// <summary>
    /// Tries to open a subkey of the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle for the key to open a subkey from.</param>
    /// <param name="subKeyName">The name of the subkey to open.</param>
    /// <param name="hResult">The open handle for the opened subkey.</param>
    /// <returns>Whether the subkey could be opened.</returns>
    public bool OpenKey(uint hKey, string subKeyName, out uint hResult)
    {
      /// Combine subkey and the key associated with the hKey
      /// to one single keyname, and pass it to OpenKey(string, out uint).
      string keyName = RegistryHelper.GetHiveAsString(hKey);
      if (keyName != null
          || _virtualRegistry.IsKnownKey(hKey, out keyName)
          || _transparantRegistry.IsKnownKey(hKey, out keyName))
      {
        return OpenKey(RegistryHelper.CombineKeys(keyName, subKeyName), out hResult);
      }
      /// Can't find the hKey.
      /// -> Bug? Where did the process get it from?
      GuestCore.Log(new LogMessage(LogLevel.Error,
                                   "The external process [PID{0}] tried to open a key from an unknown key handle. Parameters: hKey={1};subKeyName={2}",
                                   GuestCore.ProcessId, hKey, subKeyName));
      hResult = 0;
      return false;
    }

    /// <summary>
    /// Closes the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle to close.</param>
    public void CloseKey(uint hKey)
    {
      /// The virtual registry doesn't close keys,
      /// so only call the buffer to close the key.
      _transparantRegistry.CloseKey(hKey);
    }

    /// <summary>
    /// Creates a subkey with the given name for the key handle specified.
    /// </summary>
    /// <param name="hKey">The open key handle to create a subkey for.</param>
    /// <param name="subKeyName">The name of the subkey to create.</param>
    /// <param name="phkResult">The open handle to the created subkey.</param>
    /// <param name="creationDisposition">Whether the subkey is created or updated.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    public NativeResultCode CreateKey(uint hKey, string subKeyName, out uint phkResult, out RegCreationDisposition creationDisposition)
    {
      phkResult = 0;
      creationDisposition = RegCreationDisposition.NoKeyCreated;
      string keyName = RegistryHelper.GetHiveAsString(hKey);
      if (keyName == null)
      {
        /// The handle is not for a root key, check the databases.
        if (!_virtualRegistry.IsKnownKey(hKey, out keyName)
            && !_transparantRegistry.IsKnownKey(hKey, out keyName))
          return NativeResultCode.InvalidHandle;
      }
      keyName = RegistryHelper.CombineKeys(keyName, subKeyName);
      AccessMechanism access = RegistryHelper.DetermineAccessMechanism(keyName);
      return access == AccessMechanism.Transparent
               ? _transparantRegistry.CreateKey(keyName, out phkResult, out creationDisposition)
               : _virtualRegistry.CreateKey(keyName, out phkResult, out creationDisposition);
    }

    /// <summary>
    /// Deletes the key associated with the open key handle specified.
    /// </summary>
    /// <param name="hKey">The open handle of key to delete.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    public NativeResultCode DeleteKey(uint hKey)
    {
      if (RegistryHelper.IsHiveHandle(hKey))
        /// Not allowed to delete a root-key.
        return NativeResultCode.AccessDenied;
      if (_virtualRegistry.DeleteKey(hKey) == NativeResultCode.Succes)
        return NativeResultCode.Succes;
      return _transparantRegistry.DeleteKey(hKey);
    }

    /// <summary>
    /// Returns the <see cref="VirtualRegistryValue"/> associated with the given <paramref name="valueName"/>
    /// and the key handle specified.
    /// </summary>
    /// <param name="hKey">The key to get the <see cref="VirtualRegistryValue"/> from.</param>
    /// <param name="valueName">The name of the <see cref="VirtualRegistryValue"/> to return.</param>
    /// <param name="value">The returned <see cref="VirtualRegistryValue"/>.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    public NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      if (RegistryHelper.IsHiveHandle(hKey))
        /// Not allowed to access values of root-keys.
        return NativeResultCode.AccessDenied;
      if (_virtualRegistry.IsKnownKey(hKey))
        return _virtualRegistry.QueryValue(hKey, valueName, out value);
      if (_transparantRegistry.IsKnownKey(hKey))
        return _transparantRegistry.QueryValue(hKey, valueName, out value);
      /// None of the registries knows the handle.
      return NativeResultCode.InvalidHandle;
    }

    /// <summary>
    /// Sets the given <see cref="VirtualRegistryValue"/> to the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle to set the <paramref name="value"/> for.</param>
    /// <param name="value">The <see cref="VirtualRegistryValue"/> to set.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    public NativeResultCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      if (RegistryHelper.IsHiveHandle(hKey))
        /// Not allowed to access values of root-keys.
        return NativeResultCode.AccessDenied;
      if (_virtualRegistry.IsKnownKey(hKey))
        return _virtualRegistry.SetValue(hKey, value);
      if (_transparantRegistry.IsKnownKey(hKey))
        return _transparantRegistry.SetValue(hKey, value);
      /// None of the registries knows the handle.
      return NativeResultCode.InvalidHandle;
    }

    /// <summary>
    /// Deletes the given value from the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle to delete the value from.</param>
    /// <param name="valueName">The value to delete.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    public NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      if (RegistryHelper.IsHiveHandle(hKey))
        /// Not allowed to access values of root-keys.
        return NativeResultCode.AccessDenied;
      if (_virtualRegistry.IsKnownKey(hKey))
        return _virtualRegistry.DeleteValue(hKey, valueName);
      if (_transparantRegistry.IsKnownKey(hKey))
        return _transparantRegistry.DeleteValue(hKey, valueName);
      /// None of the registries knows the handle.
      return NativeResultCode.InvalidHandle;
    }

    #endregion

  }
}
