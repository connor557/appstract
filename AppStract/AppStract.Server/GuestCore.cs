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
using System.Diagnostics;
using AppStract.Core.System.Logging;
using AppStract.Core.System.IPC;
using AppStract.Server.FileSystem;
using AppStract.Server.Hooking;
using AppStract.Server.Registry;
using EasyHook;

namespace AppStract.Server
{
  /// <summary>
  /// The static core class of the guest's process.
  /// </summary>
  public static class GuestCore
  {

    #region Variables

    /// <summary>
    /// Indicates whether the <see cref="GuestCore"/> is initialized.
    /// </summary>
    private static bool _initialized;
    /// <summary>
    /// The process ID of the current process.
    /// </summary>
    private static int _currentProcessId;
    /// <summary>
    /// Tests connectivity and reports messages to the server.
    /// </summary>
    private static IServerReporter _serverReporter;
    /// <summary>
    /// Communicates synchronization queries to the server.
    /// </summary>
    private static SynchronizationBus _syncBus;
    /// <summary>
    /// Communicates log queries to the server.
    /// </summary>
    private static LogBus _logBus;
    /// <summary>
    /// Manages the API hooks.
    /// </summary>
    private static HookManager _hookManager;
    /// <summary>
    /// Collection of eventhandlers to call when requesting a process exit.
    /// </summary>
    private static readonly List<ExitRequestEventHandler> _exitRequestEventHandlersCollection = new List<ExitRequestEventHandler>(1);
    /// <summary>
    /// The object to lock when initializing the <see cref="GuestCore"/>.
    /// </summary>
    private static readonly object _initializationLock = new object();
    /// <summary>
    /// The object to lock when performing actions on <see cref="_exitRequestEventHandlersCollection"/>.
    /// </summary>
    private static readonly object _exitRequestEventLock = new object();

    #endregion

    #region Public Properties

    /// <summary>
    /// The current instance's log service.
    /// </summary>
    public static Logger Log
    {
      get { return _logBus; }
    }

    /// <summary>
    /// Gets the ID of the current process.
    /// </summary>
    public static int ProcessId
    {
      get { return _currentProcessId; }
    }

    /// <summary>
    /// Gets whether the core has been initialized for the current process.
    /// </summary>
    public static bool Initialized
    {
      get { return _initialized; }
    }

    /// <summary>
    /// Gets whether the connection from the guest to the host is up.
    /// </summary>
    public static bool Connected
    {
      get
      {
        try
        {
          _serverReporter.Ping();
          return true;
        }
        catch
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Gets the <see cref="HookManager"/> managing the API hooks of the current process.
    /// </summary>
    public static HookManager HookManager
    {
      get { return _hookManager; }
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a module requests the current process to exit.
    /// </summary>
    public static event ExitRequestEventHandler ExitRequestRaised
    {
      add { lock (_exitRequestEventLock) _exitRequestEventHandlersCollection.Add(value); }
      remove { lock (_exitRequestEventLock) _exitRequestEventHandlersCollection.Remove(value); }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the <see cref="GuestCore"/>.
    /// </summary>
    /// <param name="processSynchronizer">
    /// The <see cref="IProcessSynchronizer"/> to use for communication with the host.
    /// </param>
    public static void Initialize(IProcessSynchronizer processSynchronizer)
    {
      lock (_initializationLock)
      {
        if (_initialized) return;
        // Initialize variables.
        _currentProcessId = RemoteHooking.GetCurrentProcessId();
        _serverReporter = processSynchronizer;
        _syncBus = new SynchronizationBus(processSynchronizer, processSynchronizer);
        _logBus = new LogBus(processSynchronizer) {Enabled = true};
        // Attach ProcessExit event handler.
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        // Complete initialization.
        _syncBus.AutoFlush = true;
        _initialized = true;
        Log.Message("Successfully initialized core components.");
      }
    }

    /// <summary>
    /// Installs all hooks, <see cref="Initialize"/> must be called before.
    /// </summary>
    /// <exception cref="GuestException">
    /// A <see cref="GuestException"/> is thrown if <see cref="Initialize"/> hasn't been called
    /// before installing the hooks.
    /// </exception>
    public static void StartVirtualizationEngine()
    {
      lock (_initializationLock)
        if (!_initialized)
          throw new GuestException("The GuestCore must be initialized before hook installation can start.");
      try
      {
        // Initialize virtualization engine.
        var fileSystem = new FileSystemProvider(_syncBus, _syncBus.ResourceLoader.FileSystemRoot);
        var registry = new RegistryProvider(_syncBus);
        // Register required API hooks to the manager.
        _hookManager = new HookManager();
        _hookManager.RegisterHookProvider(new FileSystemHookProvider(fileSystem));
        _hookManager.RegisterHookProvider(new RegistryHookProvider(registry));
        // Install the API hooks.
        _hookManager.InstallHooks();
      }
      catch (Exception e)
      {
        Log.Critical("Failed to start the virtualization engine.", e);
        TerminateProcess(-1, ExitMethod.Kill);
        throw; // In case TerminateProcess didn't do it's job
      }
      Log.Message("Successfully started the virtualization engine.");
    }

    /// <summary>
    /// Terminates the current process.
    /// </summary>
    /// <param name="exitCode">The exit code to return to the operating system.</param>
    /// <param name="exitMethod">The method(s) to use for termination.</param>
    /// <returns>True if the current process' termination code is invoked, false otherwise.</returns>
    public static bool TerminateProcess(int exitCode, ExitMethod exitMethod)
    {
      Log.Message("Terminating process with exit code " + exitCode + ".");
      _syncBus.Flush();
      if ((exitMethod & ExitMethod.Request) == ExitMethod.Request
          && RaiseExitRequest(exitCode))
        return true;
      return (exitMethod & ExitMethod.Kill) == ExitMethod.Kill && KillGuestProcess();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Raises the <see cref="ExitRequestRaised"/> event.
    /// The main purpose of raising this request is to kill the wrapper process while also providing an exit code.
    /// 
    /// </summary>
    /// <param name="exitCode"></param>
    private static bool RaiseExitRequest(int exitCode)
    {
      // Make a copy of the eventhandlers before raising them.
      IEnumerable<ExitRequestEventHandler> eventHandlers;
      lock (_exitRequestEventLock)
      {
        eventHandlers = _exitRequestEventHandlersCollection == null
                          ? new ExitRequestEventHandler[0]
                          : _exitRequestEventHandlersCollection.ToArray();
      }
      // Raise all events.
      bool exitRequestHandled = true;
      foreach (var eventHandler in eventHandlers)
        exitRequestHandled = eventHandler(exitCode) ? exitRequestHandled : false;
      // Write log message according to the result.
      if (exitRequestHandled)
        Log.Debug("Exit procedure is invoked.");
      else
        Log.Warning("Exit procedure invocation FAILED.");
      return exitRequestHandled;
    }

    /// <summary>
    /// Attempts to immediately stop the guest process with reduced risk of loosing cached data.
    /// </summary>
    /// <returns></returns>
    private static bool KillGuestProcess()
    {
      Log.Debug("Sending kill signal to process...");
      try
      {
        Process.GetCurrentProcess().Kill();
        return true;
      }
      catch (Exception e)
      {
        Log.Critical("Failed to kill the process.", e);
        return false;
      }
    }

    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      _syncBus.Flush();
    }

    #endregion

  }
}
