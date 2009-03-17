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
using System.Collections.Generic;
using AppStract.Core.Data;
using AppStract.Core.Virtualization.Registry;
using ValueType = AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Database for all the keys known by the virtual registry.
  /// </summary>
  public class VirtualRegistryData : RegistryBase
  {

    #region Constructors

    public VirtualRegistryData(IndexGenerator indexGenerator, IEnumerable<VirtualRegistryKey> keys)
      : base(indexGenerator)
    {
      foreach (VirtualRegistryKey key in keys)
        _keys.Add(key.Index, key);
    }

    #endregion

    #region Public Methods

    public override uint? OpenKey(string keyFullPath)
    {
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      return base.OpenKey(keyFullPath);
    }

    public override StateCode  CreateKey(string keyFullPath, out uint? hKey)
    {
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      return base.CreateKey(keyFullPath, out hKey);
    }

    public override StateCode QueryValue(uint hkey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(null, ValueType.INVALID);
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
        object o = Microsoft.Win32.Registry.GetValue(key.Path, valueName, null);
        if (o == null)
          return StateCode.NotFound;
        value = new VirtualRegistryValue(o, ValueType.REG_NONE);
      }
      catch
      {
        return StateCode.AccessDenied;
      }
      if (access == AccessMechanism.CreateAndCopy)
        key.Values.Add(valueName, value);
      return StateCode.Succes;
    }

    public override StateCode SetValue(uint hKey, string valueName, VirtualRegistryValue value)
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
      if (key.Values.Keys.Contains(valueName))
        key.Values[valueName] = value;
      else
        key.Values.Add(valueName, value);
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
