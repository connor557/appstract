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
using AppStract.Engine.Data.Connection;
using AppStract.Engine.Virtualization;
using AppStract.Utilities.Logging;
using EasyHook;

namespace AppStract.Engine
{
  /// <summary>
  /// The static core class of the guest's process.
  /// </summary>
  public static class EngineCore
  {

    #region Variables

    /// <summary>
    /// Indicates whether the <see cref="EngineCore"/> is initialized.
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
    /// Communicates log queries to the server.
    /// </summary>
    private static LogBus _logBus;
    /// <summary>
    /// The virtualization engine supporting the current process.
    /// </summary>
    private static VirtualizationEngine _engine;
    /// <summary>
    /// Eventhandlers to call when a process exit is detected.
    /// </summary>
    private static EventHandler _onProcessExit;
    /// <summary>
    /// Eventhandlers to call when requesting a process exit.
    /// </summary>
    private static readonly List<ExitRequestEventHandler> _exitRequestEventHandlersCollection = new List<ExitRequestEventHandler>(1);
    /// <summary>
    /// The object to lock when initializing the <see cref="EngineCore"/> and/or changing any of the class events.
    /// </summary>
    private static readonly object _syncRoot = new object();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the virtualization engine supporting the current process.
    /// </summary>
    public static VirtualizationEngine Engine
    {
      get { return _engine; }
    }

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

    #endregion

    #region Events

    /// <summary>
    /// Occurs before <see cref="EngineCore"/> tries to kill the current process,
    /// and/or when <see cref="EngineCore"/> detects that the current process is terminating.
    /// </summary>
    /// <remarks>
    /// It's possible that this event is raised multiple times during process shutdown.
    /// </remarks>
    public static event EventHandler OnProcessExit
    {
      add { lock (_syncRoot) _onProcessExit += value; }
      remove { lock (_syncRoot) _onProcessExit -= value; }
    }

    /// <summary>
    /// Occurs when a module requests the current process to exit.
    /// This event should be subscribed by all classes able to close the process in a clean manner.
    /// </summary>
    public static event ExitRequestEventHandler ExitRequestRaised
    {
      add { lock (_syncRoot) _exitRequestEventHandlersCollection.Add(value); }
      remove { lock (_syncRoot) _exitRequestEventHandlersCollection.Remove(value); }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the <see cref="EngineCore"/>.
    /// </summary>
    /// <param name="processSynchronizer">
    /// The <see cref="IProcessSynchronizer"/> to use for communication with the host.
    /// </param>
    public static void Initialize(IProcessSynchronizer processSynchronizer)
    {
      lock (_syncRoot)
      {
        if (_initialized) return;
        // Initialize variables.
        _currentProcessId = RemoteHooking.GetCurrentProcessId();
        _serverReporter = processSynchronizer;
        _logBus = new LogBus(processSynchronizer);
#if !SYNCLOG
        _logBus.Enabled = true;
#endif
        _engine = VirtualizationEngine.InitializeEngine(processSynchronizer, processSynchronizer);
        // Attach ProcessExit event handler.
        AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
        // Mark instance as initialized.
        _initialized = true;
        Log.Message("Successfully initialized core components.");
      }
    }

    /// <summary>
    /// Installs all hooks, <see cref="Initialize"/> must be called before.
    /// </summary>
    /// <exception cref="EngineException">
    /// In case it's not possible to start the virtualization engine the following actions are taken:
    /// - <see cref="EngineCore"/> signals the termination of the current process, using <see cref="TerminateProcess"/>.
    /// - An <see cref="EngineException"/> is thrown if the process can't be terminated.
    /// </exception>
    /// <exception cref="EngineException">
    /// A <see cref="EngineException"/> is thrown if <see cref="Initialize"/> hasn't been called
    /// before installing the hooks.
    /// </exception>
    public static void StartVirtualizationEngine()
    {
      lock (_syncRoot)
        if (!_initialized)
          throw new EngineException("EngineCore must be initialized before the virtualization engine can be started.");
      try
      {
        _engine.StartEngine();
      }
      catch (EngineException e)
      {
        Log.Critical("Failed to start the virtualization engine.", e);
        TerminateProcess(-1, ExitMethod.Kill);
        throw;
      }
      Log.Message("Virtualization Engine is started and running.");
    }

    /// <summary>
    /// Terminates the current process while reducing the risk of loosing any cached data.
    /// </summary>
    /// <remarks>
    /// Terminating the current process involves taking the following steps:
    /// - <see cref="OnProcessExit"/> is raised.
    /// - In case <see cref="ExitMethod.Request"/> is specified, <see cref="ExitRequestRaised"/> is raised.
    /// - In case <see cref="ExitMethod.Kill"/> is specified, the process is killed.
    /// </remarks>
    /// <param name="exitCode">The exit code to return to the operating system.</param>
    /// <param name="exitMethod">The method(s) to use for termination.</param>
    /// <returns>True if the current process' termination code is invoked, false otherwise.</returns>
    public static bool TerminateProcess(int exitCode, ExitMethod exitMethod)
    {
      Log.Message("Terminating process with exit code " + exitCode + ".");
      lock (_syncRoot)
        _onProcessExit(null, new EventArgs());
      if ((exitMethod & ExitMethod.Request) == ExitMethod.Request
          && RaiseExitRequest(exitCode))
        return true;
      return (exitMethod & ExitMethod.Kill) == ExitMethod.Kill && KillProcess();
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
      lock (_syncRoot)
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
    /// Attempts to immediately stop the current proces.
    /// </summary>
    /// <returns></returns>
    private static bool KillProcess()
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

    /// <summary>
    /// Eventhandler for the <see cref="AppDomain.ProcessExit"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      lock (_syncRoot)
        _onProcessExit(null, new EventArgs());
    }

    #endregion

  }
}
