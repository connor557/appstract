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

using System;
using System.Linq;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Extensions;
using AppStract.Utilities.Interop;
using AppStract.Utilities.Observables;
using Microsoft.Win32.Interop;
using ValueType = AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Database for all the keys known by the virtual registry.
  /// </summary>
  public sealed class VirtualRegistryData : RegistryBase
  {

    #region Constructors

    public VirtualRegistryData(IndexGenerator indexGenerator)
      : base(indexGenerator, new ObservableDictionary<uint, VirtualRegistryKey>())
    {
    }

    #endregion

    #region Public Methods

    public void LoadData(IRegistryLoader dataSource)
    {
      using (_keysSynchronizationLock.EnterDisposableWriteLock())
        dataSource.LoadRegistryTo((ObservableDictionary<uint, VirtualRegistryKey>)_keys);
    }

    public override bool OpenKey(string keyFullPath, out uint hResult)
    {
      hResult = 0;
      string requestedKey = keyFullPath;
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      // We're not sure if this method is read-only, depends on whether or not WriteKey() is called
      using (_keysSynchronizationLock.EnterDisposableUpgradeableReadLock())
      {
        // Try to find the key in the virtual registry.
        VirtualRegistryKey virtualRegistryKey
          = _keys.Values.FirstOrDefault(key => key.Path.ToLowerInvariant() == keyFullPath);
        if (virtualRegistryKey == null && KeyExistsInHostRegistry(requestedKey))
        {
          // The key doesn't exist yet. Create the key in the virtual registry,
          // but ONLY IF the key exists in the current host's registry.
          virtualRegistryKey = ConstructRegistryKey(keyFullPath);
          WriteKey(virtualRegistryKey, false);
        }
        if (virtualRegistryKey == null)
          return false;
        hResult = virtualRegistryKey.Handle;
        return true;
      }
    }

    public override NativeResultCode CreateKey(string keyFullPath, out uint hKey, out RegCreationDisposition creationDisposition)
    {
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      return base.CreateKey(keyFullPath, out hKey, out creationDisposition);
    }

    public override NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      VirtualRegistryKey key;
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        if (!_keys.Keys.Contains(hKey))
          return NativeResultCode.InvalidHandle;
        key = _keys[hKey];
      }
      if (key.Values.Keys.Contains(valueName))
      {
        value = key.Values[valueName];
        return NativeResultCode.Succes;
      }
      AccessMechanism access = RegistryHelper.DetermineAccessMechanism(key.Path);
      if (access == AccessMechanism.Transparent)
        throw new ApplicationException("The application tries to handle a transparant key with the virtual registry.");
      try
      {
        ValueType valueType;
        var data = RegistryHelper.ReadValueFromRegistry(key.Path, valueName, out valueType);
        if (data == null)
          return NativeResultCode.FileNotFound;
        value = new VirtualRegistryValue(valueName, MarshallingHelpers.ToByteArray(data), valueType);
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
      if (access == AccessMechanism.CreateAndCopy)
        key.Values.Add(valueName, value);
      return NativeResultCode.Succes;
    }

    public override NativeResultCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      VirtualRegistryKey key;
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        if (!_keys.Keys.Contains(hKey))
          return NativeResultCode.InvalidHandle;
        key = _keys[hKey];
      }
      if (key.Values.Keys.Contains(value.Name))
        key.Values[value.Name] = value;
      else
        key.Values.Add(value.Name, value);
      return NativeResultCode.Succes;
    }

    public override NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      VirtualRegistryKey key;
      using (_keysSynchronizationLock.EnterDisposableReadLock())
      {
        if (!_keys.Keys.Contains(hKey))
          return NativeResultCode.InvalidHandle;
        key = _keys[hKey];
      }
      return key.Values.Remove(valueName)
               ? NativeResultCode.Succes
               : NativeResultCode.NotFound;
    }

    #endregion

  }
}
