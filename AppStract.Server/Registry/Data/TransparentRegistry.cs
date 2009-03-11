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
using AppStract.Core.Virtualization.Registry;
using Microsoft.Win32;
using ValueType = AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Buffers keys that are read from the host's registry
  /// without being saved to the virtual registry.
  /// </summary>
  public class TransparentRegistry : RegistryDatabase
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
    /// <param name="keyFullPath"></param>
    /// <returns></returns>
    public override uint? OpenKey(string keyFullPath)
    {
      RegistryKey hostRegistryKey = ReadKeyFromHostRegistry(keyFullPath, false);
      if (hostRegistryKey == null)
        return null;
      hostRegistryKey.Close();
      /// Key exists, buffer it before returning.
      VirtualRegistryKey virtualRegistryKey = ConstructRegistryKey(keyFullPath);
      WriteKey(virtualRegistryKey, false);
      return virtualRegistryKey.Index;
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

    public override StateCode CreateKey(string keyFullPath, out uint? hKey)
    {
      hKey = null;
      /// Create the key in the real registry.
      RegistryKey registryKey = CreateKeyInHostRegistry(keyFullPath);
      if (registryKey == null)
        return StateCode.AccessDenied;
      registryKey.Close();
      /// Buffer the created key.
      return base.CreateKey(keyFullPath, out hKey);
    }

    public override StateCode DeleteKey(uint hKey)
    {
      /// Overriden, first delete the real key.
      string keyName;
      if (!IsKnownKey(hKey, out keyName))
        return StateCode.InvalidHandle;
      int index = keyName.LastIndexOf(@"\");
      if (index < 1)
        return StateCode.InvalidHandle;
      string subKeyName = keyName.Substring(index);
      keyName = keyName.Substring(0, index);
      RegistryKey registryKey = ReadKeyFromHostRegistry(keyName, true);
      try
      {
        registryKey.DeleteSubKeyTree(subKeyName);
      }
      catch (System.Security.SecurityException)
      {
        return StateCode.AccessDenied;  
      }
      catch (UnauthorizedAccessException)
      {
        return StateCode.AccessDenied;
      }
      /// Now call the base, to delete the key from the database/buffer.
      return base.DeleteKey(hKey);
    }

    public override StateCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(null, ValueType.INVALID);
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return StateCode.InvalidHandle;
      try
      {
        object o = Microsoft.Win32.Registry.GetValue(keyPath, valueName, null);
        if (o == null)
          return StateCode.NotFound;
        value = new VirtualRegistryValue(o, ValueType.REG_NONE);
        return StateCode.Succes;
      }
      catch
      {
        return StateCode.AccessDenied;
      }
    }

    public override StateCode SetValue(uint hKey, string valueName, VirtualRegistryValue value)
    {
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return StateCode.InvalidHandle;
      try
      {
        Microsoft.Win32.Registry.SetValue(keyPath, valueName, value.Data);
      }
      catch
      {
        return StateCode.AccessDenied;
      }
      return StateCode.Succes;
    }

    public override StateCode DeleteValue(uint hKey, string valueName)
    {
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return StateCode.InvalidHandle;
      try
      {
        RegistryKey registryKey = RegistryHelper.GetHiveAsKey(keyPath, out keyPath);
        if (registryKey == null)
          return StateCode.NotFound;
        registryKey.DeleteValue(valueName, true);
        return StateCode.Succes;
      }
      catch (ArgumentException)
      {
        return StateCode.NotFound;
      }
      catch
      {
        return StateCode.AccessDenied;
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Reads the specified key from the host's registry.
    /// </summary>
    /// <param name="keyFullPath">The full path of the key, including the root key.</param>
    /// <param name="writable">Set to true if write access is required.</param>
    /// <returns>Null if key isn't read from the host's registry.</returns>
    private static RegistryKey ReadKeyFromHostRegistry(string keyFullPath, bool writable)
    {
      string subKeyName;
      RegistryKey registryKey = RegistryHelper.GetHiveAsKey(keyFullPath, out subKeyName);
      if (registryKey == null)
        return null;
      if (subKeyName == null)
        return registryKey;
      RegistryKey subRegistryKey;
      try
      {
        subRegistryKey = registryKey.OpenSubKey(subKeyName, writable);
        registryKey.Close();
      }
      catch
      {
        return null;
      }
      return subRegistryKey;
    }

    /// <summary>
    /// Creates the specified key in the host's registry, and returns it.
    /// </summary>
    /// <param name="keyFullPath">The full path of the key, including the root key.</param>
    /// <returns></returns>
    private static RegistryKey CreateKeyInHostRegistry(string keyFullPath)
    {
      string subKeyName;
      RegistryKey registryKey = RegistryHelper.GetHiveAsKey(keyFullPath, out subKeyName);
      if (registryKey == null)
        return null;
      if (subKeyName == null)
        return registryKey;
      RegistryKey subRegistryKey;
      try
      {
        subRegistryKey = registryKey.CreateSubKey(subKeyName);
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
