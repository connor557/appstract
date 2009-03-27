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
using AppStract.Core.Data.Virtualization;
using AppStract.Core.Virtualization.Synchronization;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Observables;
using Microsoft.Win32.Interop;
using ValueType = AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Database for all the keys known by the virtual registry.
  /// </summary>
  public class VirtualRegistryData : RegistryBase
  {

    #region Constructors

    public VirtualRegistryData(IndexGenerator indexGenerator)
      : base(indexGenerator, new ObservableDictionary<uint, VirtualRegistryKey>())
    {
    }

    #endregion

    #region Public Methods

    public void LoadData(IRegistrySynchronizer dataSource)
    {
      _keysSynchronizationLock.EnterWriteLock();
      try
      {
        dataSource.LoadRegistryTo((ObservableDictionary<uint, VirtualRegistryKey>) _keys);
      }
      finally
      {
        _keysSynchronizationLock.ExitWriteLock();
      }
    }

    public override bool OpenKey(string keyFullPath, out uint hResult)
    {
      hResult = 0;
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      /// We're not sure if this method is read-only.
      _keysSynchronizationLock.EnterUpgradeableReadLock();
      try
      {
        /// Try to find the key in the virtual registry.
        VirtualRegistryKey virtualRegistryKey
          = _keys.Values.First(key => key.Path.ToLowerInvariant() == keyFullPath);
        if (virtualRegistryKey == null && KeyExistsInHostRegistry(keyFullPath))
        {
          /// The key doesn't exist yet.
          /// Create it key in the virtual registry,
          /// but ONLY IF the key exists in the current host's registry!
          virtualRegistryKey = ConstructRegistryKey(keyFullPath);
          WriteKey(virtualRegistryKey, false);
        }
        if (virtualRegistryKey == null)
          return false;
        hResult = virtualRegistryKey.Handle;
        return true;
      }
      finally
      {
        _keysSynchronizationLock.ExitUpgradeableReadLock();
      }
    }

    public override StateCode CreateKey(string keyFullPath, out uint hKey, out RegCreationDisposition creationDisposition)
    {
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      return base.CreateKey(keyFullPath, out hKey, out creationDisposition);
    }

    public override StateCode QueryValue(uint hkey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      VirtualRegistryKey key;
      _keysSynchronizationLock.EnterReadLock();
      try
      {
        if (!_keys.Keys.Contains(hkey))
          return StateCode.InvalidHandle;
        key = _keys[hkey];
      }
      finally
      {
        _keysSynchronizationLock.ExitReadLock();
      }
      if (key.Values.Keys.Contains(valueName))
      {
        value = key.Values[valueName];
        return StateCode.Succes;
      }
      AccessMechanism access = RegistryHelper.DetermineAccessMechanism(key.Path);
      if (access == AccessMechanism.Transparent)
        throw new ApplicationException("The application tries to handle a transparant key with the virtual registry.");
      try
      {
        object defValue = new object();
        object o = Microsoft.Win32.Registry.GetValue(key.Path, valueName, defValue);
        if (o == defValue)
          return StateCode.FileNotFound;
        value = new VirtualRegistryValue(valueName, o, ValueType.REG_NONE);
      }
      catch
      {
        return StateCode.AccessDenied;
      }
      if (access == AccessMechanism.CreateAndCopy)
        key.Values.Add(valueName, value);
      return StateCode.Succes;
    }

    public override StateCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      VirtualRegistryKey key;
      _keysSynchronizationLock.EnterReadLock();
      try
      {
        if (!_keys.Keys.Contains(hKey))
          return StateCode.InvalidHandle;
        key = _keys[hKey];
      }
      finally
      {
        _keysSynchronizationLock.ExitReadLock();
      }
      if (key.Values.Keys.Contains(value.Name))
        key.Values[value.Name] = value;
      else
        key.Values.Add(value.Name, value);
      return StateCode.Succes;
    }

    public override StateCode DeleteValue(uint hKey, string valueName)
    {
      VirtualRegistryKey key;
      _keysSynchronizationLock.EnterReadLock();
      try
      {
        if (!_keys.Keys.Contains(hKey))
          return StateCode.InvalidHandle;
        key = _keys[hKey];
      }
      finally
      {
        _keysSynchronizationLock.ExitReadLock();
      }
      return key.Values.Remove(valueName)
               ? StateCode.Succes
               : StateCode.NotFound;
    }

    #endregion

  }
}
