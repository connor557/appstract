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
using System.Linq;
using EasyHook;

namespace AppStract.Server.Hooking
{
  /// <summary>
  /// Manages the API hooks which are available and/or installed for the current process.
  /// </summary>
  /// <remarks>
  /// <see cref="Initialize"/> must be called before <see cref="HookManager"/> can provide any data or install any hook.
  /// </remarks>
  public static class HookManager
  {

    #region Public Classes

    /// <summary>
    /// The list of threads that are ensured to not be intercepted by any of the installed API Hooks.
    /// </summary>
    /// <remarks>
    /// Registering a thread for exclusion should always be done as following:
    /// <code>
    /// using (HookManager.ACL.GetHookingExclusion())
    /// {
    ///   // Perform all actions that must NOT be intercepted by any of the hook handlers.
    /// } // The exclusion is disposed and the excluded thread will be intercepted again.
    /// </code>
    /// The exclusion is also disposed when the finalizer of the object returned by <see cref="GetHookingExclusion"/> is called.
    /// </remarks>
    public static class ACL
    {

      #region Private Classes

      /// <summary>
      /// Represents a hooking exclusion.
      /// No calls will be intercepted from the native thread with id <see cref="_threadId"/>
      /// as long as <see cref="Dispose"/> or the destructor has not been called.
      /// </summary>
      private sealed class HookingExclusion : IDisposable
      {

        #region Variables

        private readonly int _threadId;
        private readonly object _disposeLock;
        private bool _isDisposed;

        #endregion

        #region Constructor/Destructor

        public HookingExclusion(int threadId)
        {
          _threadId = threadId;
          _disposeLock = new object();
        }

        ~HookingExclusion()
        {
          Dispose();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
          lock (_disposeLock)
          {
            if (_isDisposed) return;
            _isDisposed = true;
          }
          EndHookingExclusion(_threadId);
        }

        #endregion

      }

      #endregion

      #region Variables

      private static readonly object _aclLock = new object();
      private static readonly Dictionary<int, int> _acl = new Dictionary<int, int>();

      #endregion

      #region Static Methods

      /// <summary>
      /// Ensures that the current thread will not be intercepted by any of the installed API hooks.
      /// The hooking exclusion only works for the current underlying native thread of the current managed thread,
      /// it is not guaranteed that the underlying native thread will never change. Therefore it is recommended
      /// that the returned object is disposed as soon as the application logic allows it.
      /// </summary>
      /// <remarks>
      /// A thread can call this method recursively. The hook exclusion is undone when all of the returned objects are disposed.
      /// </remarks>
      /// <returns>An object that when disposed, will exit the hooking exclusion of the current thread.</returns>
      public static IDisposable GetHookingExclusion()
      {
        var currentThreadId = AppDomain.GetCurrentThreadId();
        lock (_aclLock)
        {
          if (_acl.ContainsKey(currentThreadId))
          {
            _acl[currentThreadId]++;
          }
          else
          {
            _acl.Add(currentThreadId, 1);
            LocalHook.GlobalThreadACL.SetExclusiveACL(_acl.Keys.ToArray());
          }
        }
        return new HookingExclusion(currentThreadId);
      }

      private static void EndHookingExclusion(int threadId)
      {
        lock (_aclLock)
        {
          if (!_acl.ContainsKey(threadId))
            return;
          _acl[threadId]--;
          if (_acl[threadId] > 0)
            return;
          _acl.Remove(threadId);
          LocalHook.GlobalThreadACL.SetExclusiveACL(_acl.Keys.ToArray());
        }
      }

      #endregion

    }

    #endregion

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
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if <see cref="HookManager"/> is already initialized.
    /// </exception>
    /// <param name="inCallback">An uninterpreted callback that will later be available through <see cref="HookRuntimeInfo.Callback"/>.</param>
    /// <param name="hookHandler">The object containing the methods to associate with hooked functions.</param>
    internal static void Initialize(object inCallback, HookImplementations hookHandler)
    {
      lock (_syncRoot)
      {
        if (_initialized)
          throw new ApplicationException("HookManager is already initialized.");
        GuestCore.Log.Debug("HookManager starts initialization procedure.");
        var hooks = new List<HookData>(17);
        // Hooks regarding the filesystem
        hooks.Add(new HookData("Create Directory [Unicode]",
                               "kernel32.dll", "CreateDirectoryW",
                               new HookDelegates.DCreateDirectory_Unicode(hookHandler.DoCreateDirectory),
                               inCallback));
        hooks.Add(new HookData("Create Directory [Ansi]",
                               "kernel32.dll", "CreateDirectoryA",
                               new HookDelegates.DCreateDirectory_Ansi(hookHandler.DoCreateDirectory),
                               inCallback));
        hooks.Add(new HookData("Create File [Unicode]",
                               "kernel32.dll", "CreateFileW",
                               new HookDelegates.DCreateFile_Unicode(hookHandler.DoCreateFile),
                               inCallback));
        hooks.Add(new HookData("Create File [Ansi]",
                               "kernel32.dll", "CreateFileA",
                               new HookDelegates.DCreateFile_Ansi(hookHandler.DoCreateFile),
                               inCallback));
        hooks.Add(new HookData("Delete File [Unicode]",
                               "kernel32.dll", "DeleteFileW",
                               new HookDelegates.DDeleteFile_Unicode(hookHandler.DoDeleteFile),
                               inCallback));
        hooks.Add(new HookData("Delete File [Ansi]",
                               "kernel32.dll", "DeleteFileA",
                               new HookDelegates.DDeleteFile_Ansi(hookHandler.DoDeleteFile),
                               inCallback));
        hooks.Add(new HookData("Load Library [Unicode]",
                               "kernel32.dll", "LoadLibraryExW",
                               new HookDelegates.DLoadLibraryEx_Unicode(hookHandler.DoLoadLibraryEx),
                               inCallback));
        hooks.Add(new HookData("Load Library [Ansi]",
                               "kernel32.dll", "LoadLibraryExA",
                               new HookDelegates.DLoadLibraryEx_Ansi(hookHandler.DoLoadLibraryEx),
                               inCallback));
        // Hooks regarding the registry
        hooks.Add(new HookData("Open Registry Key [Unicode]",
                               "advapi32.dll", "RegOpenKeyExW",
                               new HookDelegates.DOpenKey_Unicode(hookHandler.RegOpenKey_Hooked),
                               inCallback));
        hooks.Add(new HookData("Open Registry Key [Ansi]",
                               "advapi32.dll", "RegOpenKeyExA",
                               new HookDelegates.DOpenKey_Ansi(hookHandler.RegOpenKey_Hooked),
                               inCallback));
        hooks.Add(new HookData("Create Registry Key [Unicode]",
                               "advapi32.dll", "RegCreateKeyExW",
                               new HookDelegates.DCreateKey_Unicode(hookHandler.RegCreateKeyEx_Hooked),
                               inCallback));
        hooks.Add(new HookData("Create Registry Key [Ansi]",
                               "advapi32.dll", "RegCreateKeyExA",
                               new HookDelegates.DCreateKey_Ansi(hookHandler.RegCreateKeyEx_Hooked),
                               inCallback));
        hooks.Add(new HookData("Close Registry Key",
                               "advapi32.dll", "RegCloseKey",
                               new HookDelegates.DCloseKey(hookHandler.RegCloseKey_Hooked),
                               inCallback));
        hooks.Add(new HookData("Set Registry Value [Unicode]",
                               "advapi32.dll", "RegSetValueExW",
                               new HookDelegates.DSetValue_Unicode(hookHandler.RegSetValueEx),
                               inCallback));
        hooks.Add(new HookData("Set Registry Value [Ansi]",
                               "advapi32.dll", "RegSetValueExA",
                               new HookDelegates.DSetValue_Ansi(hookHandler.RegSetValueEx),
                               inCallback));
        hooks.Add(new HookData("Query Registry Value [Unicode]",
                               "advapi32.dll", "RegQueryValueExW",
                               new HookDelegates.DQueryValue_Unicode(hookHandler.RegQueryValue_Hooked),
                               inCallback));
        hooks.Add(new HookData("Query Registry Value [Ansi]",
                               "advapi32.dll", "RegQueryValueExA",
                               new HookDelegates.DQueryValue_Ansi(hookHandler.RegQueryValue_Hooked),
                               inCallback));
        _hooks = hooks;
        _initialized = true;
        GuestCore.Log.Debug("HookManager is initialized.");
      }
    }

    /// <summary>
    /// Installs all available hooks in the local process.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if <see cref="Initialize"/> hasn't been called before the current call.
    /// </exception>
    /// <exception cref="HookingException">
    /// A <see cref="HookingException"/> is thrown if the installation of any of the API hooks fails.
    /// </exception>
    internal static void InstallHooks()
    {
      GuestCore.Log.Debug("HookManager starts installing the API hooks.");
      lock (_syncRoot)
      {
        if (!_initialized)
          throw new ApplicationException("The current instance has not yet been initialized.");
        _installedHooks = new List<LocalHook>();
        foreach (var hook in _hooks)
        {
          try
          {
            var localHook = LocalHook.Create(hook.GetTargetEntryPoint(), hook.Handler, hook.Callback);
            // Set an empty list for the excluded threads; All threads need to be intercepted.
            // Ideally we would set this list so some core threads would not be intercepted, but this is impossible to implement
            // since there is no way to fix a managed thread on a native os thread.
            localHook.ThreadACL.SetExclusiveACL(new int[0]);
            _installedHooks.Add(localHook);
            GuestCore.Log.Debug("HookManager installed API hook: " + hook);
          }
          catch (Exception e)
          {
            GuestCore.Log.Error("HookManager failed to install API hook: " + hook, e);
            throw new HookingException("HookManager failed to install API hook.", hook, e);
          }
        }
      }
    }

    #endregion

  }
}
