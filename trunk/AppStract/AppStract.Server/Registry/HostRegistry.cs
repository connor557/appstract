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
using System.Runtime.InteropServices;
using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.Registry;
using AppStract.Utilities.Extensions;
using Microsoft.Win32;
using ValueType = AppStract.Core.Virtualization.Engine.Registry.ValueType;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// Provides helper methods for manipulating the host's registry.
  /// </summary>
  public static class HostRegistry
  {

    #region Public Methods

    /// <summary>
    /// Combines two keynames to one keyname.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// An <see cref="ArgumentNullException"/> is thrown if any of the parameters is null.
    /// </exception>
    /// <param name="keyName">First part of the keyname.</param>
    /// <param name="subKeyName">Name of the subkey, the second part of the keyname.</param>
    /// <returns>The combined keyname.</returns>
    public static string CombineKeyNames(string keyName, string subKeyName)
    {
      if (keyName == null)
        throw new ArgumentNullException("keyName");
      if (subKeyName == null)
        throw new ArgumentNullException("subKeyName");
      if (subKeyName.Length == 0)
        return keyName;
      if (keyName.EndsWith(@"\"))
      {
        return subKeyName.StartsWith(@"\")
                 ? keyName + subKeyName.Substring(1)
                 : keyName + subKeyName;
      }
      return subKeyName.StartsWith(@"\")
               ? keyName + subKeyName
               : keyName + @"\" + subKeyName;
    }

    /// <summary>
    /// Creates and returns a <see cref="RegistryKey"/> for the specified key path.
    /// </summary>
    /// <param name="keyPath">The full path of the key, including the root key.</param>
    /// <param name="creationDisposition">Whether the key has been opened or created.</param>
    /// <returns></returns>
    public static RegistryKey CreateKey(string keyPath, out RegCreationDisposition creationDisposition)
    {
      string subKeyName;
      var registryKey = HiveHelper.GetHive(keyPath, out subKeyName).AsRegistryKey();
      creationDisposition = RegCreationDisposition.NoKeyCreated;
      if (registryKey == null || subKeyName == null)
        return registryKey;
      try
      {
        var subRegistryKey = registryKey.OpenSubKey(subKeyName);
        if (subRegistryKey != null)
        {
          creationDisposition = RegCreationDisposition.OpenedExistingKey;
        }
        else
        {
          subRegistryKey = registryKey.CreateSubKey(subKeyName);
          creationDisposition = RegCreationDisposition.CreatedNewKey;
        }
        return subRegistryKey;
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Closes the specified handle, which is supposed to be an open handle provided by the host's registry.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns></returns>
    public static bool CloseKey(uint hKey)
    {
      return NativeAPI.RegCloseKey(hKey) == NativeResultCode.Success;
    }

    /// <summary>
    /// Returns the name of registry key which has been assigned the specified handle by the host registry.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns>The registry key name associated to the handle. If no name can be retrieved, null is returned.</returns>
    public static string GetKeyNameByHandle(uint hKey)
    {
      var ptrSize = 0;
      var ptr = Marshal.AllocHGlobal(ptrSize);
      var resultCode = NativeAPI.NtQueryKey(new UIntPtr(hKey), NativeAPI.KeyInformationClass.KeyNameInformation,
                                            ptr, ptrSize, out ptrSize);
      Marshal.FreeHGlobal(ptr);
      if (resultCode != NativeResultCode.BufferTooSmall && resultCode != NativeResultCode.BufferOverflow)
        return null; // No data available
      ptr = Marshal.AllocHGlobal(ptrSize);
      resultCode = NativeAPI.NtQueryKey(new UIntPtr(hKey), NativeAPI.KeyInformationClass.KeyNameInformation,
                                        ptr, ptrSize, out ptrSize);
      // If success, the pointer is expected to point to a KEY_NAME_INFORMATION struct instance.
      // This struct always has an integer on the first 4 bytes and characters on all following bytes, if any.
      // All pointed data is read as one string, the key name is then obtained by removing the first 4 bytes.
      var keyName = resultCode == NativeResultCode.Success
                      ? ptr.Read<string>((uint)ptrSize).Substring(2)
                      : null;
      Marshal.FreeHGlobal(ptr);
      if (resultCode != NativeResultCode.Success)
        Marshal.ThrowExceptionForHR((int)resultCode);
      keyName = Data.RegistryTranslator.FromWin32Path(keyName);
      return keyName;
    }

    /// <summary>
    /// Returns whether the key with the specified path exists in the current host's registry.
    /// </summary>
    /// <param name="keyFullPath">The full path of the key, including the root key.</param>
    /// <returns>Whether the key exist's in the current host's registry.</returns>
    public static bool KeyExists(string keyFullPath)
    {
      var key = OpenKey(keyFullPath, false);
      if (key == null)
        return false;
      key.Close();
      return true;
    }

    /// <summary>
    /// Opens a <see cref="RegistryKey"/> matching the specified <paramref name="keyPath"/>.
    /// </summary>
    /// <param name="keyPath"></param>
    /// <param name="writable"></param>
    /// <returns>The opened <see cref="RegistryKey"/>; Or null, in case the method failed</returns>
    public static RegistryKey OpenKey(string keyPath, bool writable)
    {
      var key = HiveHelper.GetHive(keyPath, out keyPath).AsRegistryKey();
      if (keyPath == null)
        return key;
      try
      {
        return key.OpenSubKey(keyPath, writable);
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Returns the value for <paramref name="valueName"/> under the specified <paramref name="keyPath"/>.
    /// </summary>
    /// <param name="keyPath"></param>
    /// <param name="valueName"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static object QueryValue(string keyPath, string valueName, out ValueType valueType)
    {
      valueType = ValueType.REG_NONE;
      var key = OpenKey(keyPath, false);
      if (key == null)
        return null;
      var value = key.GetValue(valueName);
      if (value == null)
        return null;
      valueType = key.GetValueKind(valueName).AsValueType();
      return value;
    }

    #endregion

  }
}
