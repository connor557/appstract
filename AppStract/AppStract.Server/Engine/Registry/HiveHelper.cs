#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
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
using System.Linq;
using Microsoft.Win32;

namespace AppStract.Engine.Engine.Registry
{
  /// <summary>
  /// Provides helper methods for actions related to registry hives.
  /// </summary>
  public static class HiveHelper
  {

    #region Private Classes

    /// <summary>
    /// Maps all ways to identify a hive.
    /// </summary>
    private static class HiveMap
    {

      #region Variables

      private static readonly uint[] _handles
        = new[]
            {
              0x80000000,
              0x80000001,
              0x80000002,
              0x80000003,
              0x80000004,
              0x80000005,
              0x80000006
            };

      private static readonly RegistryHive[] _hives
        = new[]
            {
              RegistryHive.ClassesRoot,
              RegistryHive.CurrentUser,
              RegistryHive.LocalMachine,
              RegistryHive.Users,
              RegistryHive.PerformanceData,
              RegistryHive.CurrentConfig,
              RegistryHive.DynData
            };

      private static readonly string[] _names
        = new[]
            {
              "HKEY_CLASSES_ROOT",
              "HKEY_CURRENT_USER",
              "HKEY_LOCAL_MACHINE",
              "HKEY_USERS",
              "HKEY_PERFORMANCE_DATA",
              "HKEY_CURRENT_CONFIG",
              "HKEY_DYN_DATA"
            };

      private static readonly RegistryKey[] _keys
        = new[]
            {
              Microsoft.Win32.Registry.ClassesRoot,
              Microsoft.Win32.Registry.CurrentUser,
              Microsoft.Win32.Registry.LocalMachine,
              Microsoft.Win32.Registry.Users,
              Microsoft.Win32.Registry.PerformanceData,
              Microsoft.Win32.Registry.CurrentConfig,
              Microsoft.Win32.Registry.DynData
            };

      #endregion

      #region Public Methods

      public static bool IsLegalId(int id)
      {
        return id >= 0 && id < _handles.Length;
      }

      public static bool IsDefined(uint value)
      {
        return _handles.Contains(value);
      }
      public static bool IsDefined(RegistryHive value)
      {
        return _hives.Contains(value);
      }
      public static bool IsDefined(string value)
      {
        return _names.Contains(value.ToUpperInvariant());
      }
      public static bool IsDefined(RegistryKey value)
      {
        return _keys.Contains(value);
      }

      public static int GetIdFor(uint value)
      {
        return FindIndex(_handles, value, (i, j) => i == j);
      }
      public static int GetIdFor(RegistryHive value)
      {
        return FindIndex(_hives, value, (i, j) => i == j);
      }
      public static int GetIdFor(string value)
      {
        value = value.ToUpperInvariant();
        return FindIndex(_names, value, (i, j) => i == j);
      }
      public static int GetIdFor(RegistryKey value)
      {
        return FindIndex(_keys, value, (i, j) => i.Name == j.Name);
      }

      public static uint GetHandleById(int id)
      {
        return _handles[id];
      }
      public static RegistryHive GetHiveById(int id)
      {
        return _hives[id];
      }
      public static string GetNameById(int id)
      {
        return _names[id];
      }
      public static RegistryKey GetKeyById(int id)
      {
        return _keys[id];
      }

      #endregion

      #region Private Methods

      private static int FindIndex<T>(IEnumerable<T> array, T value, Func<T, T, bool> comparer)
      {
        int i = 0;
        foreach (var item in array)
        {
          if (comparer(value, item))
            return i;
          i++;
        }
        return -1;
      }

      #endregion

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the specified handle is predefined for a hive.
    /// </summary>
    /// <param name="hKey">The handle to check.</param>
    /// <returns>True if <paramref name="hKey"/> is a predefined handle.</returns>
    public static bool IsHiveHandle(uint hKey)
    {
      return HiveMap.IsDefined(hKey);
    }

    /// <summary>
    /// Returns whether the specified handle is predefined for a hive.
    /// </summary>
    /// <param name="hKey">The handle to check.</param>
    /// <param name="registryHive">The <see cref="RegistryHive"/> matching the handle.</param>
    /// <returns>True if <paramref name="hKey"/> is a predefined handle.</returns>
    public static bool IsHiveHandle(uint hKey, out RegistryHive registryHive)
    {
      var id = HiveMap.GetIdFor(hKey);
      if (HiveMap.IsLegalId(id))
      {
        registryHive = HiveMap.GetHiveById(id);
        return true;
      }
      registryHive = default(RegistryHive);
      return false;
    }

    /// <summary>
    /// Returns whether the specified handle is predefined for a hive.
    /// </summary>
    /// <param name="hKey">The handle to check.</param>
    /// <param name="hiveName">The name of the hive matching the handle.</param>
    /// <returns>True if <paramref name="hKey"/> is a predefined handle.</returns>
    public static bool IsHiveHandle(uint hKey, out string hiveName)
    {
      var id = HiveMap.GetIdFor(hKey);
      if (HiveMap.IsLegalId(id))
      {
        hiveName = HiveMap.GetNameById(id);
        return true;
      }
      hiveName = null;
      return false;
    }

    /// <summary>
    /// Returns whether a hive with the given <paramref name="name"/> is defined.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns>True if <paramref name="name"/> identifies a registry hive.</returns>
    public static bool IsHiveName(string name)
    {
      return HiveMap.IsDefined(name);
    }

    /// <summary>
    /// Returns the hive of which the specified <paramref name="keyName"/> belongs to.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if the <see cref="RegistryHive"/>
    /// can't be extracted from the specified <paramref name="keyName"/>.
    /// </exception>
    /// <param name="keyName">Key to extract and return the <see cref="RegistryHive"/> for.</param>
    /// <returns>The <see cref="RegistryHive"/> to which the <paramref name="keyName"/> belongs.</returns>
    public static RegistryHive GetHive(string keyName)
    {
      string tmp;
      return GetHive(keyName, out tmp);
    }

    /// <summary>
    /// Returns the hive containing the specified <paramref name="keyName"/> as a <see cref="RegistryKey"/>.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if the <see cref="RegistryHive"/>
    /// can't be extracted from the specified <paramref name="keyName"/>.
    /// </exception>
    /// <param name="keyName">The full key name. Used to extract the rootkey from.</param>
    /// <param name="subKeyName">The name of the hive's subkey, as specified in <paramref name="keyName"/>.</param>
    /// <returns>The top-level <see cref="RegistryKey"/> of which <paramref name="keyName"/> is a member.</returns>
    public static RegistryHive GetHive(string keyName, out string subKeyName)
    {
      int index = keyName.IndexOf("\\");
      if (index == 0)
        throw new ApplicationException("Can't extract the root key from " + keyName);
      if (index != -1)
      {
        subKeyName = keyName.Substring(index + 1);
        keyName = keyName.Substring(0, index).ToUpperInvariant();
      }
      else
        subKeyName = null;
      var id = HiveMap.GetIdFor(keyName);
      if (HiveMap.IsLegalId(id))
        return HiveMap.GetHiveById(id);
      throw new ApplicationException(string.Format("The requested hive \"{0}\" can't be found.", keyName));
    }

    /// <summary>
    /// Returns the registry hive with the specified handle.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if the <see cref="RegistryHive"/>
    /// can't be determined from <paramref name="hKey"/>.
    /// </exception>
    /// <param name="hKey"></param>
    /// <returns></returns>
    public static RegistryHive GetHive(uint hKey)
    {
      RegistryHive hive;
      if (!IsHiveHandle(hKey, out hive))
        throw new ApplicationException("Can't determine a registry hive from key handle " + hKey);
      return hive;
    }

    #endregion

    #region Extension Methods

    /// <summary>
    /// Returns the full name of the root key associated with the current <see cref="RegistryHive"/>.
    /// </summary>
    /// <param name="registryHive">Indicator of the registry hive which full name must be returned.</param>
    /// <returns>The full name of the root key matching the <see cref="RegistryHive"/>.</returns>
    public static string AsRegistryHiveName(this RegistryHive registryHive)
    {
      return HiveMap.GetNameById(HiveMap.GetIdFor(registryHive));
    }

    /// <summary>
    /// Returns the root key associated with the current <see cref="RegistryHive"/>.
    /// </summary>
    /// <param name="registryHive">Indicator of the top-level key to return.</param>
    /// <returns>The root <see cref="RegistryKey"/> matching the <see cref="RegistryHive"/>.</returns>
    public static RegistryKey AsRegistryKey(this RegistryHive registryHive)
    {
      var id = HiveMap.GetIdFor(registryHive);
      return HiveMap.IsLegalId(id)
               ? HiveMap.GetKeyById(id)
               : null;
    }

    #endregion

  }
}
