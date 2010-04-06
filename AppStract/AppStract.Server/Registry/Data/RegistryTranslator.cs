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
using System.Security.Principal;
using Microsoft.Win32;

namespace AppStract.Server.Registry.Data
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

    /// <summary>
    /// The string representing the CurrentUser of the virtual environment.
    /// </summary>
    private const string _virtualCurrentUser = "s-appstract-user";
    /// <summary>
    /// The string representing the CurrentConfig of the virtual environment.
    /// </summary>
    private const string _virtualCurrentHardwareProfile = "appstract-config";
    /// <summary>
    /// The string representing the SID of the current user of the host operating system.
    /// Also referred to as the real current user.
    /// </summary>
    private static readonly string _currentUserSid;
    /// <summary>
    /// The string representing the current hardware profile (CurrentConfig) of the host operating system.
    /// Also referred to as the real current config, or real current hardware profile.
    /// </summary>
    private static readonly string _currentHardwareProfile;
    /// <summary>
    /// The full path to the real current user's profile.
    /// </summary>
    private static readonly string _currentUserFullPath;
    /// <summary>
    /// The full path to the real current hardware profile.
    /// </summary>
    private static readonly string _currentHardwareProfileFullPath;
    /// <summary>
    /// The full path to the virtual current user's profile.
    /// </summary>
    private static readonly string _virtualCurrentUserFullPath;
    /// <summary>
    /// The full path to the  virtual current hardware profile.
    /// </summary>
    private static readonly string _virtualCurrentHardwareProfileFullPath;

    #endregion

    #region Constructors

    static RegistryTranslator()
    {
      // First set the values for the virtual paths, these values are required by ToRealPath(string)
      // ToRealPath(string) is called when for example GetCurrentProfileNumber(string) accesses the registry.
      _virtualCurrentUserFullPath = @"hkey_users\" + _virtualCurrentUser;
      _virtualCurrentHardwareProfileFullPath = @"hkey_local_machine\system\currentcontrolset\hardware profiles\"
                                               + _virtualCurrentHardwareProfile;
      // Default value is ".DEFAULT", which is also the default value used by Windows.
      _currentUserSid = GetCurrentUserSID(".default");
      _currentUserFullPath = @"hkey_users\" + _currentUserSid;
      // Default value is "0001", the most common value for profile-numbers.
      _currentHardwareProfile = GetCurrentProfileNumber("0001");
      _currentHardwareProfileFullPath = @"hkey_local_machine\system\currentcontrolset\hardware profiles\"
                                        + _currentHardwareProfile;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the given <paramref name="win32RegistryPath"/> as a normal "HKEY_" full registry path.
    /// The return value is a lower cased string.
    /// </summary>
    /// <remarks>
    /// Example of a win32 registry path:
    ///   \REGISTRY\MACHINE\SOFTWARE\Microsoft
    /// Example of the returned value:
    ///   hkey_local_machine\software\microsoft
    /// </remarks>
    /// <param name="win32RegistryPath"></param>
    /// <returns></returns>
    public static string FromWin32Path(string win32RegistryPath)
    {
      win32RegistryPath = win32RegistryPath.ToLowerInvariant();
      if (!win32RegistryPath.StartsWith(@"\registry\"))
        return win32RegistryPath;
      win32RegistryPath = win32RegistryPath.Substring(@"\registry\".Length);
      if (win32RegistryPath.StartsWith(@"user\"))
      {
        win32RegistryPath = win32RegistryPath.Substring(@"user\".Length);
        return win32RegistryPath.StartsWith(_currentUserSid)
                 ? @"hkey_current_user" + win32RegistryPath.Substring(_currentUserSid.Length)
                 : @"hkey_users\" + win32RegistryPath;
      }
      if (win32RegistryPath.StartsWith(@"machine\"))
        return "hkey_local_machine" + win32RegistryPath.Substring("machine".Length);
      // Throw exception, other possible values are undocumented and not yet researched
      throw new NotImplementedException("No implementation for \"\\REGISTRY\\" + win32RegistryPath +
                                        "\". Only \"\\REGISTRY\\USER\\\" and \"\\REGISTRY\\MACHINE\\\" are implemented.");
    }

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
      // Does the path lead to the current user?
      if (fullRegistryPath.StartsWith("hkey_current_user"))
        return (fullRegistryPath.Replace("hkey_current_user",
                                         _virtualCurrentUserFullPath));
      if (fullRegistryPath.StartsWith(_currentUserFullPath))
        return (fullRegistryPath.Replace(_currentUserFullPath,
                                         _virtualCurrentUserFullPath));
      // Does the path lead to the current config?
      if (fullRegistryPath.StartsWith("hkey_current_config"))
        return (fullRegistryPath.Replace("hkey_current_config",
                                         _virtualCurrentHardwareProfileFullPath));
      if (fullRegistryPath.StartsWith(@"hkey_local_machine\system\currentcontrolset\hardware profiles\current"))
        return (fullRegistryPath.Replace(@"hkey_local_machine\system\currentcontrolset\hardware profiles\current",
                                         _virtualCurrentHardwareProfileFullPath));
      if (fullRegistryPath.StartsWith(_currentHardwareProfileFullPath))
        return (fullRegistryPath.Replace(_currentHardwareProfileFullPath,
                                         _virtualCurrentHardwareProfileFullPath));
      // Nothing to replace.
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
      // Does it lead to the virtual current config?
      if (virtualFullRegistryPath.StartsWith(_virtualCurrentHardwareProfileFullPath))
        return (virtualFullRegistryPath.Replace(_virtualCurrentHardwareProfileFullPath,
                                                "hkey_current_config"));
      // Does it lead to the virtual current user?
      if (virtualFullRegistryPath.StartsWith(_virtualCurrentUserFullPath))
        return (virtualFullRegistryPath.Replace(_virtualCurrentUserFullPath,
                                                "hkey_current_user"));
      // Nothing to replace.
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
      // http://www.microsoft.com/technet/prodtechnol/windows2000serv/reskit/regentry/69675.mspx?mfr=true
      //   To determine which numbered subkey under Hardware Profiles represents the current hardware profile,
      //   see CurrentConfig in HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\IDConfigDB.
      //   The value of CurrentConfig corresponds to the number of the subkey that contains the current
      //   hardware profile.
      RegistryKey currentProfileKey
        = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\IDConfigDB\");
      if (currentProfileKey == null)
        return defaultValue;
      object currentProfileValue = currentProfileKey.GetValue("CurrentConfig");
      return currentProfileValue != null
               ? currentProfileValue.ToString().PadLeft(4, '0')
               : defaultValue;
    }

    #endregion

  }
}
