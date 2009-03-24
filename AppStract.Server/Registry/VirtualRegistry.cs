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

using AppStract.Core.Synchronization;
using AppStract.Server.Registry.Data;
using AppStract.Core.Data;
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

    public void Initialize(IRegistrySynchronizer dataSource)
    {
      _virtualRegistry.LoadData(dataSource);
    }

    public bool OpenKey(string keyName, out uint hResult)
    {
      var accessMechanism = RegistryHelper.DetermineAccessMechanism(keyName);
      return accessMechanism == AccessMechanism.Transparent
        ? _transparantRegistry.OpenKey(keyName, out hResult)
        : _virtualRegistry.OpenKey(keyName, out hResult);
    }

    public bool OpenKey(uint hKey, string subKeyName, out uint hResult)
    {
      string keyName = RegistryHelper.GetHiveAsString(hKey);
      if (keyName != null
          || _virtualRegistry.IsKnownKey(hKey, out keyName)
          || _transparantRegistry.IsKnownKey(hKey, out keyName))
      {
        return OpenKey(RegistryHelper.CombineKeys(keyName, subKeyName), out hResult);
      }
      /// Can't find the hKey.
      /// -> Bug: Where did the process get it from?
      hResult = 0;
      return false;
    }

    public void CloseKey(uint hKey)
    {
      /// The virtual registry doesn't close keys,
      /// so only call the buffer to close the key.
      _transparantRegistry.CloseKey(hKey);
    }

    public StateCode CreateKey(uint hKey, string subKey, out uint phkResult, out RegCreationDisposition creationDisposition)
    {
      phkResult = 0;
      creationDisposition = RegCreationDisposition.INVALID;
      string keyName = RegistryHelper.GetHiveAsString(hKey);
      if (keyName == null)
      {
        /// The handle is not for a root key, check the databases.
        if (!_virtualRegistry.IsKnownKey(hKey, out keyName)
            && !_transparantRegistry.IsKnownKey(hKey, out keyName))
          return StateCode.InvalidHandle;
      }
      keyName = RegistryHelper.CombineKeys(keyName, subKey);
      AccessMechanism access = RegistryHelper.DetermineAccessMechanism(keyName);
      return access == AccessMechanism.Transparent
               ? _transparantRegistry.CreateKey(keyName, out phkResult, out creationDisposition)
               : _virtualRegistry.CreateKey(keyName, out phkResult, out creationDisposition);
    }

    public StateCode DeleteKey(uint hKey)
    {
      if (RegistryHelper.IsHiveHandle(hKey))
        return StateCode.AccessDenied;
      if (_virtualRegistry.DeleteKey(hKey) == StateCode.Succes)
        return StateCode.Succes;
      return _transparantRegistry.DeleteKey(hKey);
    }

    public StateCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      /// ToDo: Implement the following in the hook handler!
      /// 
      /// An application typically calls RegEnumValue to determine the value names and then
      /// RegQueryValueEx to retrieve the data for the names.
      /// 
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      if (RegistryHelper.IsHiveHandle(hKey))
        return StateCode.AccessDenied;
      if (_virtualRegistry.IsKnownKey(hKey))
        return _virtualRegistry.QueryValue(hKey, valueName, out value);
      if (_transparantRegistry.IsKnownKey(hKey))
        return _transparantRegistry.QueryValue(hKey, valueName, out value);
      return StateCode.InvalidHandle;
    }

    public StateCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      if (RegistryHelper.IsHiveHandle(hKey))
        return StateCode.AccessDenied;
      if (_virtualRegistry.IsKnownKey(hKey))
        return _virtualRegistry.SetValue(hKey, value);
      if (_transparantRegistry.IsKnownKey(hKey))
        return _transparantRegistry.SetValue(hKey, value);
      return StateCode.InvalidHandle;
    }

    public StateCode DeleteValue(uint hKey, string valueName)
    {
      if (RegistryHelper.IsHiveHandle(hKey))
        return StateCode.AccessDenied;
      if (_virtualRegistry.IsKnownKey(hKey))
        return _virtualRegistry.DeleteValue(hKey, valueName);
      if (_transparantRegistry.IsKnownKey(hKey))
        return _transparantRegistry.DeleteValue(hKey, valueName);
      return StateCode.InvalidHandle;
    }

    #endregion

  }
}
