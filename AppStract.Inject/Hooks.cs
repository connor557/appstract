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
using EasyHook;

namespace AppStract.Inject
{

  /// <summary>
  /// Provides an <see cref="IEnumerable{T}"/> containing all available <see cref="HookData"/>.
  /// <see cref="Initialize"/> must be called before <see cref="Hooks"/> can provide that data.
  /// </summary>
  internal static class Hooks
  {

    #region Variables

    private static bool _initialized;
    private static IList<HookData> _hooks;

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the manager.
    /// </summary>
    /// <param name="inCallback">An uninterpreted callback that will later be available through <see cref="HookRuntimeInfo.Callback"/>.</param>
    /// <param name="hookHandler"></param>
    public static void Initialize(object inCallback, HookImplementations hookHandler)
    {
      List<HookData> hooks = new List<HookData>();
      /// Hooks regarding the filesystem
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("kernel32.dll", "CreateFileW"),
                  new HookDelegates.DCreateFile(hookHandler.DoCreateFile),
                  inCallback));
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("kernel32.dll", "CreateDirectoryW"),
                  new HookDelegates.DCreateDirectory(hookHandler.DoCreateDirectory),
                  inCallback));
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("kernel32.dll", "LoadLibraryExW"),
                  new HookDelegates.DLoadLibraryEx(hookHandler.DoLoadLibraryEx),
                  inCallback));
      /// Hooks regarding the registry
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("advapi32.dll", "RegSetValueExW"),
                  new HookDelegates.DSetValue(hookHandler.RegSetValueEx),
                  inCallback));
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("advapi32.dll", "RegQueryValueExW"),
                  new HookDelegates.DQueryValue(hookHandler.RegQueryValue_Hooked),
                  inCallback));
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("advapi32.dll", "RegOpenKeyExW"),
                  new HookDelegates.DOpenKey(hookHandler.RegOpenKey_Hooked),
                  inCallback));
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("advapi32.dll", "RegCreateKeyExW"),
                  new HookDelegates.DCreateKey(hookHandler.RegCreateKeyEx_Hooked),
                  inCallback));
      hooks.Add(new HookData(
                  LocalHook.GetProcAddress("advapi32.dll", "RegCloseKey"),
                  new HookDelegates.DCloseKey(hookHandler.RegCloseKey_Hooked),
                  inCallback));
      _hooks = hooks;
      _initialized = true;
    }

    /// <summary>
    /// Returns all available hooks.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<HookData> GetAvailableHooks()
    {
      if (!_initialized)
        throw new ApplicationException("The current instance has not yet been initialized.");
      return _hooks;
    }

    #endregion

  }
}
