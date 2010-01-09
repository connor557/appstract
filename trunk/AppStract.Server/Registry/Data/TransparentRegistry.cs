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
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Registry;
using Microsoft.Win32;
using Microsoft.Win32.Interop;
using ValueType = AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Buffers keys that are read from the host's registry
  /// without being saved to the virtual registry.
  /// </summary>
  public sealed class TransparentRegistry : RegistryBase
  {

    #region Variables

    #endregion

    #region Constructors

    public TransparentRegistry(IndexGenerator indexGenerator)
      : base(indexGenerator)
    {
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Tries to read the specified key from the host's registry.
    /// The index of the buffered key is returned, if the key is successfully read.
    /// Else, the return value is null.
    /// </summary>
    /// <param name="keyName"></param>
    /// <param name="hResult"></param>
    /// <returns></returns>
    public override bool OpenKey(string keyName, out uint hResult)
    {
      hResult = 0;
      RegistryKey hostRegistryKey = ReadKeyFromHostRegistry(keyName, false);
      if (hostRegistryKey == null)
        return false;
      hostRegistryKey.Close();
      /// Key exists, buffer it before returning.
      VirtualRegistryKey virtualRegistryKey = ConstructRegistryKey(keyName);
      WriteKey(virtualRegistryKey, false);
      hResult = virtualRegistryKey.Handle;
      return true;
    }

    /// <summary>
    /// Closes a key.
    /// </summary>
    /// <param name="hKey">Key to close.</param>
    public void CloseKey(uint hKey)
    {
      _keysSynchronizationLock.EnterWriteLock();
      try
      {
        _keys.Remove(hKey);
      }
      finally
      {
        _keysSynchronizationLock.ExitWriteLock();
      }
      _indexGenerator.Release(hKey);
    }

    public override NativeResultCode CreateKey(string keyFullPath, out uint hKey, out RegCreationDisposition creationDisposition)
    {
      hKey = 0;
      /// Create the key in the real registry.
      RegistryKey registryKey = CreateKeyInHostRegistry(keyFullPath, out creationDisposition);
      if (registryKey == null)
        return NativeResultCode.AccessDenied;
      registryKey.Close();
      /// Buffer the created key.
      return base.CreateKey(keyFullPath, out hKey, out creationDisposition);
    }

    public override NativeResultCode DeleteKey(uint hKey)
    {
      /// Overriden, first delete the real key.
      string keyName;
      if (!IsKnownKey(hKey, out keyName))
        return NativeResultCode.InvalidHandle;
      int index = keyName.LastIndexOf(@"\");
      if (index < 1)
        return NativeResultCode.InvalidHandle;
      string subKeyName = keyName.Substring(index);
      keyName = keyName.Substring(0, index);
      RegistryKey registryKey = ReadKeyFromHostRegistry(keyName, true);
      try
      {
        registryKey.DeleteSubKeyTree(subKeyName);
      }
      catch (System.Security.SecurityException)
      {
        return NativeResultCode.AccessDenied;  
      }
      catch (UnauthorizedAccessException)
      {
        return NativeResultCode.AccessDenied;
      }
      /// Now call the base, to delete the key from the database/buffer.
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
        object defValue = new object();
        object o = Microsoft.Win32.Registry.GetValue(keyPath, valueName, defValue);
        if (o == defValue)
          return NativeResultCode.FileNotFound;
        /// ToDo: Get the ValueType from the Registry using RegistryKey.GetValueKind();
        value = new VirtualRegistryValue(valueName, o, ValueType.REG_NONE);
        return NativeResultCode.Succes;
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
        Microsoft.Win32.Registry.SetValue(keyPath, value.Name, value.Data);
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
      return NativeResultCode.Succes;
    }

    public override NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return NativeResultCode.InvalidHandle;
      try
      {
        RegistryKey registryKey = RegistryHelper.GetHiveAsKey(keyPath, out keyPath);
        if (registryKey == null)
          return NativeResultCode.NotFound;
        registryKey.DeleteValue(valueName, true);
        return NativeResultCode.Succes;
      }
      catch (ArgumentException)
      {
        return NativeResultCode.NotFound;
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Creates the specified key in the host's registry, and returns it.
    /// </summary>
    /// <param name="keyFullPath">The full path of the key, including the root key.</param>
    /// <param name="creationDisposition">Whether the key has been opened or created.</param>
    /// <returns></returns>
    private static RegistryKey CreateKeyInHostRegistry(string keyFullPath, out RegCreationDisposition creationDisposition)
    {
      string subKeyName;
      RegistryKey registryKey = RegistryHelper.GetHiveAsKey(keyFullPath, out subKeyName);
      creationDisposition = RegCreationDisposition.NoKeyCreated;
      if (registryKey == null)
        return null;
      if (subKeyName == null)
        return registryKey;
      RegistryKey subRegistryKey;
      try
      {
        subRegistryKey = registryKey.OpenSubKey(subKeyName);
        if (subRegistryKey != null)
        {
          creationDisposition = RegCreationDisposition.OpenedExistingKey;
        }
        else
        {
          subRegistryKey = registryKey.CreateSubKey(subKeyName);
          creationDisposition = RegCreationDisposition.CreatedNewKey;
        }
        registryKey.Close();
      }
      catch
      {
        return null;
      }
      return subRegistryKey;
    }

    #endregion

  }
}
