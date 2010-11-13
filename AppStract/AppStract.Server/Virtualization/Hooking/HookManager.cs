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
using EasyHook;

namespace AppStract.Engine.Virtualization.Hooking
{
  /// <summary>
  /// Manages the API hooks which are available and/or installed for the current process.
  /// </summary>
  /// <remarks>
  /// <see cref="HookManager"/> comes with the following limitations:
  /// - It is not possible to instantiate more than one instance of <see cref="HookManager"/>.
  ///   -> API hooks are process wide.
  /// - It is not possible to safely dispose an instance of the <see cref="HookManager"/> class.
  ///   -> The hook handlers will anyway still stay in memory.
  /// </remarks>
  public class HookManager
  {

    #region Public Classes

    /// <summary>
    /// The list of threads that are ensured to not be intercepted by any of the installed API hooks.
    /// </summary>
    /// <remarks>
    /// Registering a thread for exclusion should always be done as following:
    /// <code>
    /// HookAccessControlList myAcl;
    /// ...
    /// using (myAcl.GetHookingExclusion())
    /// {
    ///   // Perform all actions that must NOT be intercepted by any of the hook handlers.
    /// } // The exclusion is disposed and the current thread will be intercepted again.
    /// </code>
    /// The exclusion is also disposed when the finalizer of the object returned by <see cref="GetHookingExclusion"/> is called.
    /// </remarks>
    public class HookAccessControlList
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
          Dispose(false);
        }

        #endregion

        #region Private Methods

        private void Dispose(bool disposing)
        {
          lock (_disposeLock)
          {
            if (_isDisposed) return;
            _isDisposed = true;
          }
          EndHookingExclusion(_threadId);
          if (disposing)
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
          Dispose(true);
        }

        #endregion

      }

      #endregion

      #region Variables

      private static readonly object _aclLock = new object();
      private static readonly Dictionary<int, int> _acl = new Dictionary<int, int>();

      #endregion

      #region Constructors

      internal HookAccessControlList()
      {
        
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Ensures that the current thread will not be intercepted by any of the installed API hooks.
      /// The hooking exclusion only works for the current underlying native thread of the current managed thread,
      /// it is not guaranteed that the underlying native thread will never change. Therefore it is recommended
      /// that the returned object is disposed as soon as the application logic allows it.
      /// </summary>
      /// <remarks>
      /// A thread can call this method recursively. The hook exclusion is undone when all of the returned objects are disposed.
      /// </remarks>
      /// <returns>An object that, when disposed, will exit the hooking exclusion of the current thread.</returns>
      public IDisposable GetHookingExclusion()
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

      #endregion

      #region Private Methods

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
    /// Indicates whether or not an instance of <see cref="HookManager"/> exists in the current process.
    /// </summary>
    private static bool _isInstantiated;
    /// <summary>
    /// The list of threads that are ensured to not be intercepted by any of the installed API hooks.
    /// </summary>
    private readonly HookAccessControlList _acl;
    /// <summary>
    /// All hooks that must be installed in the guest process.
    /// </summary>
    private readonly ICollection<HookProvider> _hookProviders;
    /// <summary>
    /// The hooks that are currently installed in the guest process.
    /// </summary>
    /// <remarks>
    /// <see cref="_installedHooks"/> makes sure that the installed API hooks stay referenced,
    /// keeping them from being garbage collected.
    /// </remarks>
    private readonly ICollection<LocalHook> _installedHooks;
    /// <summary>
    /// The object to lock when executing actions on any of the global variables.
    /// </summary>
    private readonly object _syncRoot;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the list of threads that are ensured to not be intercepted by any of the installed API hooks.
    /// </summary>
    public HookAccessControlList ThreadACL
    {
      get { return _acl; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="HookManager"/>.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if another instance of <see cref="HookManager"/> already exists in the current process.
    /// Instantiating another instance of <see cref="HookManager"/> would result in unexpected behavior and will corrupt <see cref="ThreadACL"/>.
    /// </exception>
    public HookManager()
    {
      if (_isInstantiated)
        throw new ApplicationException("There is already a running HookManager for the current process.");
      _isInstantiated = true;
      _acl = new HookAccessControlList();
      _hookProviders = new List<HookProvider>();
      _installedHooks = new List<LocalHook>();
      _syncRoot = new object();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Installs all known API hooks in the local process.
    /// </summary>
    /// <exception cref="HookingException">
    /// A <see cref="HookingException"/> is thrown if the installation of any of the API hooks fails.
    /// </exception>
    public void InstallHooks()
    {
      EngineCore.Log.Debug("Invoking API hook installation procedure.");
      lock (_syncRoot)
        foreach (var hookProvider in _hookProviders)
          hookProvider.InstallHooks(InstallHook);
      EngineCore.Log.Debug("Finished API hook installation.");
    }

    /// <summary>
    /// Registers a <see cref="HookProvider"/> to the current <see cref="HookManager"/>.
    /// The registered provider will be able to provide API hooks when <see cref="InstallHooks"/> is called.
    /// </summary>
    /// <param name="hookProvider"></param>
    public void RegisterHookProvider(HookProvider hookProvider)
    {
      lock (_syncRoot)
        if (!_hookProviders.Contains(hookProvider))
          _hookProviders.Add(hookProvider);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Installs an API hook based on the specified data.
    /// </summary>
    /// <param name="targetEntryPoint">The target entry point that should be hooked.</param>
    /// <param name="hookHandler">A handler with the same signature as the original entry point.</param>
    /// <param name="callback">An uninterpreted callback.</param>
    private void InstallHook(IntPtr targetEntryPoint, Delegate hookHandler, object callback)
    {
      var localHook = LocalHook.Create(targetEntryPoint, hookHandler, callback);
      localHook.ThreadACL.SetExclusiveACL(new int[0]);
      lock (_syncRoot)
        _installedHooks.Add(localHook);
    }

    #endregion

  }
}
