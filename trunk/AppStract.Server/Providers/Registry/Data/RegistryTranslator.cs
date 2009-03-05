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

using System.Security.Principal;
using Microsoft.Win32;

namespace AppStract.Server.Providers.Registry.Data
{
  /// <summary>
  /// <see cref="RegistryTranslator"/> provides two-way translations between virtual
  /// registry paths and real ones.
  /// </summary>
  /// <remarks>
  /// The registry is case-insensitive, so all translations are processed in lower casing
  /// in order to reduce complexity.
  /// </remarks>
  public static class RegistryTranslator
  {

    #region Variables

    private const string _virtualCurrentUser = "S-APPSTRACT-User";
    private const string _virtualCurrentHardwareProfile = "APPSTRACT-Config";
    private static readonly string _currentUserSid;
    private static readonly string _currentHardwareProfile;
    private static readonly string _currentUserFullPath;
    private static readonly string _currentHardwareProfileFullPath;
    private static readonly string _virtualCurrentUserFullPath;
    private static readonly string _virtualCurrentHardwareProfileFullPath;

    #endregion

    #region Properties

    #endregion

    #region Constructors

    static RegistryTranslator()
    {
      /// Default value is ".DEFAULT", which is also the default value used by Windows.
      _currentUserSid = GetCurrentUserSID(".DEFAULT");
      _currentUserFullPath = @"hkey_users\" + _currentUserSid;
      _virtualCurrentUserFullPath = @"hkey_users\" + _virtualCurrentUser;
      /// Default value is "0001", the most common value for profile-numbers.
      _currentHardwareProfile = GetCurrentProfileNumber("0001");
      _currentHardwareProfileFullPath = @"hkey_local_machine\system\currentcontrolset\hardware profiles\"
                                               + _currentHardwareProfile;
      _virtualCurrentHardwareProfileFullPath = @"hkey_local_machine\system\currentcontrolset\hardware profiles\"
                                               + _virtualCurrentHardwareProfile;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Redirects the given <paramref name="fullRegistryPath"/> to the equivalent path
    /// used by the virtual registry. The return value is a lower cased string.
    /// </summary>
    /// <remarks>
    /// The returned virtual path can be converted to a real path using <see cref="ToRealPath"/>.
    /// </remarks>
    /// <param name="fullRegistryPath"></param>
    /// <returns></returns>
    public static string ToVirtualPath(string fullRegistryPath)
    {
      fullRegistryPath = fullRegistryPath.ToLowerInvariant();
      /// Does the path lead to the current user?
      if (fullRegistryPath.StartsWith("hkey_current_user"))
        return (fullRegistryPath.Replace("hkey_current_user",
                                         _virtualCurrentUserFullPath));
      if (fullRegistryPath.StartsWith(_currentUserFullPath))
        return (fullRegistryPath.Replace(_currentUserFullPath,
                                         _virtualCurrentUserFullPath));
      /// Does the path lead to the current config?
      if (fullRegistryPath.StartsWith("hkey_current_config"))
        return (fullRegistryPath.Replace("hkey_current_config",
                                         _virtualCurrentHardwareProfileFullPath));
      if (fullRegistryPath.StartsWith(@"hkey_local_machine\system\currentcontrolset\hardware profiles\current"))
        return (fullRegistryPath.Replace(@"hkey_local_machine\system\currentcontrolset\hardware profiles\current",
                                         _virtualCurrentHardwareProfileFullPath));
      if (fullRegistryPath.StartsWith(_currentHardwareProfileFullPath))
        return (fullRegistryPath.Replace(_currentHardwareProfileFullPath,
                                         _virtualCurrentHardwareProfileFullPath));
      /// Nothing to replace.
      return fullRegistryPath;
    }

    /// <summary>
    /// Returns the equivalent path of <paramref name="virtualFullRegistryPath"/>,
    /// used by the host's registry. The return value is a lower cased string.
    /// It's possible for the return value to contain aliases, the shortest path has the highest priority.
    /// </summary>
    /// <param name="virtualFullRegistryPath"></param>
    /// <returns></returns>
    public static string ToRealPath(string virtualFullRegistryPath)
    {
      virtualFullRegistryPath = virtualFullRegistryPath.ToLowerInvariant();
      /// Does it lead to the virtual current config?
      if (virtualFullRegistryPath.StartsWith(_virtualCurrentHardwareProfileFullPath))
        return (virtualFullRegistryPath.Replace(_virtualCurrentHardwareProfileFullPath,
                                                "hkey_current_config"));
      /// Does it lead to the virtual current user?
      if (virtualFullRegistryPath.StartsWith(_virtualCurrentUserFullPath))
        return (virtualFullRegistryPath.Replace(_virtualCurrentUserFullPath,
                                                "hkey_current_user"));
      /// Nothing to replace.
      return virtualFullRegistryPath;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the SID of the current user.
    /// </summary>
    /// <param name="defaultValue">The value to return if the SID can't be acquired.</param>
    /// <returns></returns>
    private static string GetCurrentUserSID(string defaultValue)
    {
      WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
      return currentIdentity == null
               ? defaultValue
               : currentIdentity.User.Value;
    }

    /// <summary>
    /// Returns the number of the current hardware profile.
    /// </summary>
    /// <param name="defaultValue">The value to return if the profile number can't be acquired.</param>
    /// <returns></returns>
    private static string GetCurrentProfileNumber(string defaultValue)
    {
      /// http://www.microsoft.com/technet/prodtechnol/windows2000serv/reskit/regentry/69675.mspx?mfr=true
      ///   To determine which numbered subkey under Hardware Profiles represents the current hardware profile,
      ///   see CurrentConfig in HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\IDConfigDB.
      ///   The value of CurrentConfig corresponds to the number of the subkey that contains the current
      ///   hardware profile.
      RegistryKey currentProfileKey
        = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\IDConfigDB\");
      if (currentProfileKey == null)
        return defaultValue;
      object currentProfileValue = currentProfileKey.GetValue("CurrentConfig");
      if (currentProfileValue == null)
        return defaultValue;
      string value = currentProfileValue.ToString();
      while (value.Length < 4)
        value = "0" + value;
      return value;
    }

    #endregion

  }
}
