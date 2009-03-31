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

namespace AppStract.Server.Hooking
{

  /// <summary>
  /// Manages the available hooks, defined by <see cref="HookData"/>.
  /// <see cref="Initialize"/> must be called before <see cref="HookManager"/> can provide any data or install any hook.
  /// </summary>
  public static class HookManager
  {

    #region Variables

    private static bool _initialized;
    private static IList<HookData> _hooks;
    private static IList<LocalHook> _installedHooks;
    private static readonly object _syncRoot;

    #endregion

    #region Properties

    public static IEnumerable<LocalHook> InstalledHooks
    {
      get{ lock (_syncRoot) return _installedHooks; }
    }

    #endregion

    #region Constructors

    static HookManager()
    {
      _syncRoot = new object();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the manager.
    /// </summary>
    /// <param name="inCallback">An uninterpreted callback that will later be available through <see cref="HookRuntimeInfo.Callback"/>.</param>
    /// <param name="hookHandler"></param>
    public static void Initialize(object inCallback, HookImplementations hookHandler)
    {
      lock (_syncRoot)
      {
        if (_initialized)
          return;
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
    }

    /// <summary>
    /// Returns all available hooks.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if <see cref="Initialize"/> hasn't been called before the current call.
    /// </exception>
    /// <returns></returns>
    public static IEnumerable<HookData> GetAvailableHooks()
    {
      lock (_syncRoot)
      {
        if (!_initialized)
          throw new ApplicationException("The current instance has not yet been initialized.");
      }
      return _hooks;
    }

    /// <summary>
    /// Installs all available hooks in the local process.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if <see cref="Initialize"/> hasn't been called before the current call.
    /// </exception>
    public static void InstallHooks()
    {
      lock (_syncRoot)
      {
        var hooks = GetAvailableHooks();
        _installedHooks = new List<LocalHook>();
        foreach (var hook in hooks)
        {
          var localHook = LocalHook.Create(hook.TargetEntryPoint, hook.Handler, hook.Callback);
          /// 0-value in exclusive access control list: don't intercept calls from current thread
          localHook.ThreadACL.SetExclusiveACL(new[] {0});
          _installedHooks.Add(localHook);
        }
      }
    }

    #endregion

  }
}
