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
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.Registry;
using AppStract.Utilities.Extensions;
using Microsoft.Win32;
using ValueType = AppStract.Core.Virtualization.Engine.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Buffers keys that are read from the host's registry.
  /// Open keys stay buffered until <see cref="CloseKey"/> is called.
  /// </summary>
  public sealed class TransparentRegistry : RegistryBase
  {

    #region Variables

    private readonly ICollection<uint> _keysPendingClosure;
    private readonly object _keysPendingClosureSyncRoot;

    #endregion

    #region Constructors

    public TransparentRegistry(IndexGenerator indexGenerator)
      : base(indexGenerator)
    {
      _keysPendingClosure = new List<uint>();
      _keysPendingClosureSyncRoot = new object();
    }

    #endregion

    #region Overridden Methods

    public override NativeResultCode OpenKey(string keyFullPath, out uint hKey)
    {
      // Always create a new VirtualRegistryKey, no matter if if one already exists for keyName.
      // Why? There is no counter for the number of users of each handle.
      // => if one user closes the handle, other users won't be able to use it anymore.
      if (!HostRegistry.KeyExists(keyFullPath))
      {
        hKey = 0;
        return NativeResultCode.FileNotFound;
      }
      hKey = BufferKey(keyFullPath);
      return NativeResultCode.Success;
    }

    public override NativeResultCode CreateKey(string keyFullPath, out uint hKey, out RegCreationDisposition creationDisposition)
    {
      // Create the key in the real registry.
      var registryKey = HostRegistry.CreateKey(keyFullPath, out creationDisposition);
      if (registryKey == null)
      {
        hKey = 0;
        return NativeResultCode.AccessDenied;
      }
      registryKey.Close();
      hKey = BufferKey(keyFullPath);
      return NativeResultCode.Success;
    }

    public override NativeResultCode CloseKey(uint hKey)
    {
      // In the transparent registry keys are closed in one of the following two ways:
      // - If hKey is an alias, the alias is removed.
      // - If hKey is not an alias, hKey is removed from the internal dictionary by base.DeleteKey()
      //   BUT if hKey has aliases pointing to it, the removal needs to wait until all aliases are closed.
      uint realKey;
      if (IsAlias(hKey, out realKey))
      {
        RemoveAlias(hKey);
        lock (_keysPendingClosureSyncRoot)
          if (_keysPendingClosure.Contains(realKey)
              && !HasAliases(realKey))
          {
            base.DeleteKey(realKey);
            _keysPendingClosure.Remove(realKey);
          }
        return NativeResultCode.Success;
      }
      if (!HasAliases(hKey))
        return base.DeleteKey(hKey);
      lock (_keysPendingClosureSyncRoot)
        if (!_keysPendingClosure.Contains(hKey))
          _keysPendingClosure.Add(hKey);
      return NativeResultCode.Success;
    }

    public override NativeResultCode DeleteKey(uint hKey)
    {
      // Overriden, first delete the real key.
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      string keyName;
      if (!IsKnownKey(hKey, out keyName))
        return NativeResultCode.InvalidHandle;
      int index = keyName.LastIndexOf(@"\");
      string subKeyName = keyName.Substring(index + 1);
      keyName = keyName.Substring(0, index);
      RegistryKey regKey = HostRegistry.OpenKey(keyName, true);
      try
      {
        if (regKey != null)
          regKey.DeleteSubKeyTree(subKeyName);
      }
      catch (ArgumentException)
      {
        // Key is not found in real registry, call base to delete it from the buffer.
        base.DeleteKey(hKey);
        return NativeResultCode.FileNotFound;
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
      // Real key is deleted, now delete the virtual one.
      return base.DeleteKey(hKey);
    }

    public override NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return NativeResultCode.InvalidHandle;
      try
      {
        ValueType valueType;
        var data = HostRegistry.QueryValue(keyPath, valueName, out valueType);
        if (data == null)
          return NativeResultCode.FileNotFound;
        value = new VirtualRegistryValue(valueName, data.ToByteArray(), valueType);
        return NativeResultCode.Success;
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
    }

    public override NativeResultCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return NativeResultCode.InvalidHandle;
      try
      {
        // Bug: Will the registry contain a correct value here?
        Microsoft.Win32.Registry.SetValue(keyPath, value.Name, value.Data, value.Type.AsValueKind());
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
      return NativeResultCode.Success;
    }

    public override NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return NativeResultCode.InvalidHandle;
      try
      {
        var regKey = HostRegistry.OpenKey(keyPath, true);
        if (regKey == null)
          return NativeResultCode.FileNotFound;
        regKey.DeleteValue(valueName, true);
        regKey.Close();
        return NativeResultCode.Success;
      }
      catch (ArgumentException)
      {
        return NativeResultCode.FileNotFound;
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Buffers a <see cref="VirtualRegistryKey"/> for <paramref name="keyName"/> and returns the assigned handle.
    /// </summary>
    /// <param name="keyName"></param>
    /// <returns></returns>
    private uint BufferKey(string keyName)
    {
      var key = ConstructRegistryKey(keyName);
      WriteKey(key, true);
      return key.Handle;
    }

    #endregion

  }
}
