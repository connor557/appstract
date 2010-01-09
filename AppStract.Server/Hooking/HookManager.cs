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
using AppStract.Core.System.Logging;
using EasyHook;

namespace AppStract.Server.Hooking
{
  /// <summary>
  /// Manages the available hooks, defined as instances of <see cref="HookData"/>.
  /// <see cref="Initialize"/> must be called before <see cref="HookManager"/> can provide any data or install any hook.
  /// </summary>
  public static class HookManager
  {

    #region Variables

    /// <summary>
    /// Whether <see cref="HookManager"/> is initialized.
    /// </summary>
    private static bool _initialized;
    /// <summary>
    /// All hooks that must be installed in the guest process.
    /// </summary>
    private static IList<HookData> _hooks;
    /// <summary>
    /// The hooks that are currently installed in the guest process.
    /// </summary>
    private static IList<LocalHook> _installedHooks;
    /// <summary>
    /// The object to lock when executing actions on any of the global variables.
    /// </summary>
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
    /// <param name="hookHandler">The object containing the methods to associate with hooked functions.</param>
    public static void Initialize(object inCallback, HookImplementations hookHandler)
    {
      lock (_syncRoot)
      {
        if (_initialized)
          return;
        try
        {
          GuestCore.Log(new LogMessage(LogLevel.Debug, "HookManager starts initialization procedure."));
          var hooks = new List<HookData>(8);
          // Hooks regarding the filesystem
          hooks.Add(new HookData("Create Directory",
                                 LocalHook.GetProcAddress("kernel32.dll", "CreateDirectoryW"),
                                 new HookDelegates.DCreateDirectory(hookHandler.DoCreateDirectory),
                                 inCallback));
          hooks.Add(new HookData("Create File",
                                 LocalHook.GetProcAddress("kernel32.dll", "CreateFileW"),
                                 new HookDelegates.DCreateFile(hookHandler.DoCreateFile),
                                 inCallback));
          hooks.Add(new HookData("Load Library",
                                 LocalHook.GetProcAddress("kernel32.dll", "LoadLibraryExW"),
                                 new HookDelegates.DLoadLibraryEx(hookHandler.DoLoadLibraryEx),
                                 inCallback));
          // Hooks regarding the registry
          hooks.Add(new HookData("Set Registry Value",
                                 LocalHook.GetProcAddress("advapi32.dll", "RegSetValueExW"),
                                 new HookDelegates.DSetValue(hookHandler.RegSetValueEx),
                                 inCallback));
          hooks.Add(new HookData("Query Registry Value",
                                 LocalHook.GetProcAddress("advapi32.dll", "RegQueryValueExW"),
                                 new HookDelegates.DQueryValue(hookHandler.RegQueryValue_Hooked),
                                 inCallback));
          hooks.Add(new HookData("Open Registry Key",
                                 LocalHook.GetProcAddress("advapi32.dll", "RegOpenKeyExW"),
                                 new HookDelegates.DOpenKey(hookHandler.RegOpenKey_Hooked),
                                 inCallback));
          hooks.Add(new HookData("Create Registry Key",
                                 LocalHook.GetProcAddress("advapi32.dll", "RegCreateKeyExW"),
                                 new HookDelegates.DCreateKey(hookHandler.RegCreateKeyEx_Hooked),
                                 inCallback));
          hooks.Add(new HookData("Close Registry Key",
                                 LocalHook.GetProcAddress("advapi32.dll", "RegCloseKey"),
                                 new HookDelegates.DCloseKey(hookHandler.RegCloseKey_Hooked),
                                 inCallback));
          _hooks = hooks;
          _initialized = true;
          GuestCore.Log(new LogMessage(LogLevel.Information, "HookManager is initialized."));
        }
        catch (Exception e)
        {
          GuestCore.Log(new LogMessage(LogLevel.Critical, "HookManager failed to initialize the API Hooks", e), false);
          GuestCore.TerminateProcess(-1, ExitMethod.Kill);
        }
      }
    }

    /// <summary>
    /// Installs all available hooks in the local process.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if <see cref="Initialize"/> hasn't been called before the current call.
    /// </exception>
    public static void InstallHooks()
    {
      GuestCore.Log(new LogMessage(LogLevel.Debug, "HookManager starts installing the API hooks."));
      lock (_syncRoot)
      {
        if (!_initialized)
          throw new ApplicationException("The current instance has not yet been initialized.");
        _installedHooks = new List<LocalHook>();
        foreach (var hook in _hooks)
        {
          try
          {
            var localHook = LocalHook.Create(hook.TargetEntryPoint, hook.Handler, hook.Callback);
            /// 0-value in exclusive access control list: don't intercept calls from current thread
            localHook.ThreadACL.SetExclusiveACL(new[] { 0 });
            _installedHooks.Add(localHook);
            GuestCore.Log(new LogMessage(LogLevel.Debug, "HookManager installed API Hook: " + hook.Description));
          }
          catch (Exception e)
          {
            GuestCore.Log(new LogMessage(LogLevel.Critical, "HookManager failed to install API Hook: " + hook.Description, e),
                          false);
            GuestCore.TerminateProcess(-1, ExitMethod.Kill);
          }
        }
      }
      GuestCore.Log(new LogMessage(LogLevel.Debug, "HookManager finished installing the API hooks."));
    }

    #endregion

  }
}
