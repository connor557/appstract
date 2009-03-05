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
using Microsoft.Win32;

namespace AppStract.Server.Providers.Registry
{
  /// <summary>
  /// Provides helper methods for the registry.
  /// </summary>
  public static class RegistryHelper
  {

    #region Variables

    /// <summary>
    /// The predefined handles for hives, as used in the real registry.
    /// </summary>
    /// <remarks>
    /// Items are only changed during initialization, after this there are only read accesses.
    /// So there's no need for a lock when accessing this array.
    /// </remarks>
    private static readonly IDictionary<uint, RegistryHive> _hiveHandles;

    #endregion

    #region Constructors

    static RegistryHelper()
    {
      _hiveHandles = new Dictionary<uint, RegistryHive>(7)
                       {
                         {0x80000000, RegistryHive.ClassesRoot},
                         {0x80000001, RegistryHive.CurrentUser},
                         {0x80000002, RegistryHive.LocalMachine},
                         {0x80000003, RegistryHive.Users},
                         {0x80000004, RegistryHive.PerformanceData},
                         {0x80000005, RegistryHive.CurrentConfig},
                         {0x80000006, RegistryHive.DynData}
                       };
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the specified handle is predefined for a hive.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="registryHive"></param>
    /// <returns></returns>
    public static bool IsHiveHandle(uint hKey, out RegistryHive registryHive)
    {
      registryHive = RegistryHive.PerformanceData;
      if (_hiveHandles.ContainsKey(hKey))
      {
        registryHive = _hiveHandles[hKey];
        return true;
      }
      return false;
    }

    /// <summary>
    /// Returns whether the specified handle is predefined for a hive.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns></returns>
    public static bool IsHiveHandle(uint hKey)
    {
      return _hiveHandles.ContainsKey(hKey);
    }

    /// <summary>
    /// Returns the hive of which the specified <paramref name="key"/> belongs to.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static RegistryHive GetHive(string key)
    {
      /// Get the hive as string
      int index = key.IndexOf('\\');
      if (index == 0)
        throw new ApplicationException("Can't extract the root key from " + key);
      if (index != -1)
        key = key.Substring(0, index);
      key = key.ToUpperInvariant();
      /// Return the matching RegistryHive
      switch (key)
      {
        case "HKEY_USERS":
          return RegistryHive.Users;
        case "HKEY_LOCAL_MACHINE":
          return RegistryHive.LocalMachine;
        case "HKEY_CURRENT_USER":
          return RegistryHive.CurrentUser;
        case "HKEY_CURRENT_CONFIG":
          return RegistryHive.CurrentConfig;
        case "HKEY_CLASSES_ROOT":
          return RegistryHive.ClassesRoot;
        case "HKEY_PERFORMANCE_DATA":
          return RegistryHive.PerformanceData;
        case "HKEY_PERFORMANCE_NLSTEXT":
          return RegistryHive.PerformanceData;
        case "HKEY_PERFORMANCE_TEXT":
          return RegistryHive.PerformanceData;
        case "HKEY_DYN_DATA":
          return RegistryHive.DynData;
        default:
          throw new ApplicationException(string.Format("The requested hive \"{0}\" can't be found.", key));
      }
    }

    /// <summary>
    /// Returns a string representation of the specified registry hive.
    /// </summary>
    /// <param name="registryHive"></param>
    /// <returns></returns>
    public static string GetHiveAsString(RegistryHive registryHive)
    {
      switch (registryHive)
      {
        case RegistryHive.ClassesRoot:
          return "hkey_classes_root";
        case RegistryHive.CurrentConfig:
          return "hkey_current_config";
        case RegistryHive.CurrentUser:
          return "hkey_current_user";
        case RegistryHive.Users:
          return "hkey_users";
        case RegistryHive.LocalMachine:
          return "hkey_local_machine";
        case RegistryHive.DynData:
          return "hkey_dyn_data";
        case RegistryHive.PerformanceData:
          return "hkey_performance_data";
        default:
          return null;
      }
    }

    /// <summary>
    /// Returns a string representation of the registry hive with the specified handle.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns></returns>
    public static string GetHiveAsString(uint hKey)
    {
      RegistryHive hive;
      if (!IsHiveHandle(hKey, out hive))
        return null;
      return GetHiveAsString(hive);
    }

    /// <summary>
    /// Returns the hive containing the specified <paramref name="key"/> as a <see cref="RegistryKey"/>.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="subKeyName">The name of the subkey, extracted from <paramref name="key"/>.</param>
    /// <returns></returns>
    public static RegistryKey GetHiveAsKey(string key, out string subKeyName)
    {
      /// Get the name of the key.
      subKeyName = key;
      int index = subKeyName.IndexOf("\\");
      if (index > -1)
        subKeyName = subKeyName.Substring(index);
      else
        subKeyName = null;
      /// Get the hive to read from.
      RegistryHive registryHive = GetHive(key);
      /// Open the key from the host's registry.
      return GetHiveAsKey(registryHive);
    }

    /// <summary>
    /// Returns the hive, specified by <see cref="RegistryHive"/>, as a key.
    /// </summary>
    /// <param name="registryHive"></param>
    /// <returns></returns>
    public static RegistryKey GetHiveAsKey(RegistryHive registryHive)
    {
      switch (registryHive)
      {
        case RegistryHive.ClassesRoot:
          return Microsoft.Win32.Registry.ClassesRoot;
        case RegistryHive.CurrentConfig:
          return Microsoft.Win32.Registry.CurrentConfig;
        case RegistryHive.CurrentUser:
          return Microsoft.Win32.Registry.CurrentUser;
        case RegistryHive.Users:
          return Microsoft.Win32.Registry.Users;
        case RegistryHive.LocalMachine:
          return Microsoft.Win32.Registry.LocalMachine;
        case RegistryHive.DynData:
          return Microsoft.Win32.Registry.DynData;
        case RegistryHive.PerformanceData:
          return Microsoft.Win32.Registry.PerformanceData;
        default:
          return null;
      }
    }

    /// <summary>
    /// Returns the required access mechanism to use on a key.
    /// </summary>
    /// <param name="keyName"></param>
    /// <returns></returns>
    public static AccessMechanism DetermineAccessMechanism(string keyName)
    {
      return DetermineAccessMechanism(GetHive(keyName));
    }

    /// <summary>
    /// Returns the required access mechanism to use on a key.
    /// </summary>
    /// <param name="registryHive"></param>
    /// <returns></returns>
    public static AccessMechanism DetermineAccessMechanism(RegistryHive registryHive)
    {
      if (registryHive == RegistryHive.Users
          || registryHive == RegistryHive.CurrentUser)
        return AccessMechanism.CreateAndCopy;
      if (registryHive == RegistryHive.CurrentConfig
          || registryHive == RegistryHive.LocalMachine
          || registryHive == RegistryHive.ClassesRoot)
        return AccessMechanism.TransparentRead;
      if (registryHive == RegistryHive.PerformanceData
          || registryHive == RegistryHive.DynData)
        return AccessMechanism.Transparent;
      throw new ApplicationException("Can't determine required action for unknown keys of the " + registryHive);
    }

    /// <summary>
    /// Combines to keynames to one key.
    /// </summary>
    /// <param name="keyName"></param>
    /// <param name="subKeyName"></param>
    /// <returns></returns>
    public static string CombineKeys(string keyName, string subKeyName)
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

    #endregion

  }
}
