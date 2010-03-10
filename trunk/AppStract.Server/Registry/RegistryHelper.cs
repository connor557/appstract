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

namespace AppStract.Server.Registry
{
  /// <summary>
  /// Provides helper methods for the registry.
  /// </summary>
  public static class RegistryHelper
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
    /// Opens a <see cref="RegistryKey"/> matching the specified <paramref name="keyPath"/>, from the real registry.
    /// </summary>
    /// <param name="keyPath"></param>
    /// <param name="writable"></param>
    /// <returns>The opened <see cref="RegistryKey"/>; Or null, in case the method failed</returns>
    public static RegistryKey OpenRegistryKey(string keyPath, bool writable)
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
    /// Creates the specified key in the host's registry, and returns it.
    /// </summary>
    /// <param name="keyPath">The full path of the key, including the root key.</param>
    /// <param name="creationDisposition">Whether the key has been opened or created.</param>
    /// <returns></returns>
    public static RegistryKey CreateRegistryKey(string keyPath, out RegCreationDisposition creationDisposition)
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
    /// Returns whether the key with the specified path exists in the current host's registry.
    /// </summary>
    /// <param name="keyFullPath">The full path of the key, including the root key.</param>
    /// <returns>Whether the key exist's in the current host's registry.</returns>
    public static bool KeyExists(string keyFullPath)
    {
      var key = OpenRegistryKey(keyFullPath, false);
      if (key == null)
        return false;
      key.Close();
      return true;
    }

    /// <summary>
    /// Returns the value for <paramref name="valueName"/> under the specified <paramref name="keyPath"/>.
    /// This value is read from the real registry.
    /// </summary>
    /// <param name="keyPath"></param>
    /// <param name="valueName"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static object QueryRegistryValue(string keyPath, string valueName, out ValueType valueType)
    {
      valueType = ValueType.REG_NONE;
      var key = OpenRegistryKey(keyPath, false);
      if (key == null)
        return null;
      var value = key.GetValue(valueName);
      if (value == null)
        return null;
      valueType = key.GetValueKind(valueName).AsValueType();
      return value;
    }

    #endregion

    #region Extension Methods

    /// <summary>
    /// Returns the byte, as expected by Windows operating system,
    /// associated with the current <see cref="RegCreationDisposition"/>.
    /// </summary>
    /// <param name="creationDisposition">The <see cref="RegCreationDisposition"/> to return the associated byte for.</param>
    /// <returns></returns>
    public static byte AsByte(this RegCreationDisposition creationDisposition)
    {
      if (creationDisposition == RegCreationDisposition.CreatedNewKey)
        return 0x00000001;
      if (creationDisposition == RegCreationDisposition.OpenedExistingKey)
        return 0x00000002;
      return 0x00000000;
    }

    #endregion

  }
}
