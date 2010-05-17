#region Copyright (C) 2009-2010 Simon Allaeys

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

    public override NativeResultCode OpenKey(RegistryRequest request)
    {
      // Always create a new VirtualRegistryKey, no matter if if one already exists for keyName.
      // Why? There is no counter for the number of users of each handle.
      // => if one user closes the handle, other users won't be able to use it anymore.
      if (HostRegistry.KeyExists(request.KeyFullPath))
      {
        request.Handle = BufferKey(request.KeyFullPath);
        return NativeResultCode.Success;
      }
      request.Handle = 0;
      return NativeResultCode.FileNotFound;
    }

    public override NativeResultCode CreateKey(RegistryRequest request, out RegCreationDisposition creationDisposition)
    {
      // Create the key in the real registry.
      var registryKey = HostRegistry.CreateKey(request.KeyFullPath, out creationDisposition);
      if (registryKey != null)
      {
        registryKey.Close();
        request.Handle = BufferKey(request.KeyFullPath);
        return NativeResultCode.Success;
      }
      request.Handle = 0;
      return NativeResultCode.AccessDenied;
    }

    public override NativeResultCode CloseKey(RegistryRequest request)
    {
      // In the transparent registry keys are closed in one of the following two ways:
      // - If hKey is an alias, the alias is removed.
      // - If hKey is not an alias, hKey is removed from the internal dictionary by base.DeleteKey()
      //   BUT if hKey has aliases pointing to it, the removal needs to wait until all aliases are closed.
      uint realKey;
      if (IsAlias(request.Handle, out realKey))
      {
        RemoveAlias(request.Handle);
        lock (_keysPendingClosureSyncRoot)
          if (_keysPendingClosure.Contains(realKey)
              && !HasAliases(realKey))
          {
            base.DeleteKey(new RegistryRequest {Handle = realKey});
            _keysPendingClosure.Remove(realKey);
          }
        return NativeResultCode.Success;
      }
      if (!HasAliases(request.Handle))
        return base.DeleteKey(request);
      lock (_keysPendingClosureSyncRoot)
        if (!_keysPendingClosure.Contains(request.Handle))
          _keysPendingClosure.Add(request.Handle);
      return NativeResultCode.Success;
    }

    public override NativeResultCode DeleteKey(RegistryRequest request)
    {
      // Overriden, first delete the real key.
      if (HiveHelper.IsHiveHandle(request.Handle))
        return NativeResultCode.AccessDenied;
      if (!IsKnownKey(request))
        return NativeResultCode.InvalidHandle;
      var index = request.KeyFullPath.LastIndexOf(@"\");
      var subKeyName = request.KeyFullPath.Substring(index + 1);
      var keyFullPath = request.KeyFullPath.Substring(0, index);
      var registryKey = HostRegistry.OpenKey(keyFullPath, true);
      try
      {
        if (registryKey != null)
          registryKey.DeleteSubKeyTree(subKeyName);
      }
      catch (ArgumentException)
      {
        // Key is not found in real registry, call base to delete it from the buffer.
        base.DeleteKey(request);
        return NativeResultCode.FileNotFound;
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
      // Real key is deleted, now delete the virtual one.
      return base.DeleteKey(request);
    }

    public override NativeResultCode QueryValue(RegistryValueRequest request)
    {
      if (IsKnownKey(request))
      {
        try
        {
          ValueType valueType;
          var data = HostRegistry.QueryValue(request.KeyFullPath, request.Value.Name, out valueType);
          if (data != null)
          {
            request.Value = new VirtualRegistryValue(request.Value.Name, data.ToByteArray(), valueType);
            return NativeResultCode.Success;
          }
          return NativeResultCode.FileNotFound;
        }
        catch
        {
          return NativeResultCode.AccessDenied;
        }
      }
      return NativeResultCode.InvalidHandle;
    }

    public override NativeResultCode SetValue(RegistryValueRequest request)
    {
      if (IsKnownKey(request))
      {
        try
        {
          // Bug: Will the registry contain a correct value here?
          Microsoft.Win32.Registry.SetValue(request.KeyFullPath, request.Value.Name, request.Value.Data,
                                            request.Value.Type.AsValueKind());
          return NativeResultCode.Success;
        }
        catch
        {
          return NativeResultCode.AccessDenied;
        }
      }
      return NativeResultCode.InvalidHandle;
    }

    public override NativeResultCode DeleteValue(RegistryValueRequest request)
    {
      if (IsKnownKey(request))
      {
        try
        {
          var regKey = HostRegistry.OpenKey(request.KeyFullPath, true);
          if (regKey == null)
            return NativeResultCode.FileNotFound;
          regKey.DeleteValue(request.Value.Name, true);
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
      return NativeResultCode.InvalidHandle;
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
