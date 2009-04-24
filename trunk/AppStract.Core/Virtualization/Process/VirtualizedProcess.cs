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
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using Microsoft.Win32.Interop;
using AppStract.Core.Data.Application;
using AppStract.Core.Virtualization.Synchronization;
using EasyHook;
using SystemProcess = System.Diagnostics.Process;

namespace AppStract.Core.Virtualization.Process
{
  /// <summary>
  /// Provides access to local virtualized processes.
  /// </summary>
  public class VirtualizedProcess : IDisposable
  {

    #region Variables

    /// <summary>
    /// All data related to the application run in the current <see cref="VirtualizedProcess"/>.
    /// </summary>
    protected readonly VirtualProcessStartInfo _startInfo;
    /// <summary>
    /// The object responsible for the synchronization
    /// between the current process and the virtualized process.
    /// </summary>
    protected readonly ProcessSynchronizer _processSynchronizer;
    /// <summary>
    /// Name of the remoting-channel, created by RemoteHooking.IpcCreateServer
    /// </summary>
    private string _channelName;
    /// <summary>
    /// The virtualized local system process.
    /// </summary>
    private SystemProcess _process;
    /// <summary>
    /// True if EasyHook has been initialized already.
    /// </summary>
    private bool _iniEasyHook;
    /// <summary>
    /// Whether the current <see cref="VirtualizedProcess"/> has been terminated.
    /// </summary>
    private bool _hasExited;
    /// <summary>
    /// Delegates to call when the process has exited.
    /// </summary>
    private ProcessExitEventHandler _exited;
    /// <summary>
    /// The object to lock while initializing EasyHook.
    /// </summary>
    private readonly object _easyHookSyncRoot;
    /// <summary>
    /// The object to lock when calling <see cref="_exited"/>.
    /// </summary>
    private readonly object _exitEventSyncRoot;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when the current process exited.
    /// </summary>
    public event ProcessExitEventHandler Exited
    {
      add { lock (_exitEventSyncRoot) _exited += value; }
      remove { lock (_exitEventSyncRoot) _exited -= value; }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value whether the associated <see cref="VirtualizedProcess"/> has been terminated.
    /// </summary>
    public bool HasExited
    {
      get { return _hasExited; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="VirtualizedProcess"/>,
    /// using a default <see cref="ProcessSynchronizer"/> based on the specified <see cref="VirtualProcessStartInfo"/>.
    /// </summary>
    /// <param name="startInfo">
    /// The <see cref="VirtualProcessStartInfo"/> containing the information used to start the process with.
    /// </param>
    protected VirtualizedProcess(VirtualProcessStartInfo startInfo)
    {
      _easyHookSyncRoot = new object();
      _exitEventSyncRoot = new object();
      _startInfo = startInfo;
      _processSynchronizer = new ProcessSynchronizer(startInfo.Files.DatabaseFileSystem,
                                                      startInfo.Files.RootDirectory,
                                                      startInfo.Files.DatabaseRegistry);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VirtualizedProcess"/>,
    /// using a default <see cref="ProcessSynchronizer"/> based on the specified <see cref="VirtualProcessStartInfo"/>.
    /// </summary>
    /// <param name="startInfo">
    /// The <see cref="VirtualProcessStartInfo"/> containing the information used to start the process with.
    /// </param>
    /// <param name="processSynchronizer">
    /// The <see cref="ProcessSynchronizer"/> to use for data synchronization with the <see cref="VirtualizedProcess"/>.
    /// </param>
    protected VirtualizedProcess(VirtualProcessStartInfo startInfo, ProcessSynchronizer processSynchronizer)
    {
      _easyHookSyncRoot = new object();
      _exitEventSyncRoot = new object();
      _startInfo = startInfo;
      _processSynchronizer = processSynchronizer;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts a new <see cref="VirtualizedProcess"/> from the <see cref="VirtualProcessStartInfo"/> specified.
    /// </summary>
    /// <param name="startInfo">
    /// The <see cref="VirtualProcessStartInfo"/> containing the information used to start the process with.
    /// </param>
    /// <returns>
    /// A new <see cref="VirtualizedProcess"/> component that is associated with the process resource.
    /// </returns>
    public static VirtualizedProcess Start(VirtualProcessStartInfo startInfo)
    {
      /// Create an instance of VirtualizedProcess.
      var process = new VirtualizedProcess(startInfo);
      process.Start();
      return process;
    }

    /// <summary>
    /// Immediately stops the associated process.
    /// A final synchronization cycle is not guaranteed.
    /// </summary>
    public void Kill()
    {
      _process.Kill();
      _process.Dispose();
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Starts the process using the information of the class's variables.
    /// </summary>
    protected void Start()
    {
      /// Initialize the underlying resources.
      InitEasyHook();
      _hasExited = false;
      /// Start the process.
      switch (_startInfo.Files.Executable.Type)
      {
        case FileType.Assembly_Native:
          CreateAndInject();
          break;
        case FileType.Assembly_Managed:
          WrapAndInject();
          break;
        default:  /// Should never happen!
          throw new VirtualProcessException("FileType " + _startInfo.Files.Executable.Type +
                                            " can't be used to start a process with.");
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Initializes all components related to <see cref="EasyHook"/>.
    /// </summary>
    private void InitEasyHook()
    {
      lock (_easyHookSyncRoot)
      {
        if (_iniEasyHook)
          return;
        Config.Register("AppStract", CoreBus.Configuration.AppConfig.LibsToRegister.ToArray());
        ProcessSynchronizerInterface.SProcessSynchronizer = _processSynchronizer;
        RemoteHooking.IpcCreateServer<ProcessSynchronizerInterface>(ref _channelName, WellKnownObjectMode.Singleton);
        _iniEasyHook = true;
      }
    }

    /// <summary>
    /// Creates and injects the current <see cref="VirtualizedProcess"/>,
    /// and sets the created process component to the <see cref="_process"/> variable.
    /// </summary>
    private void CreateAndInject()
    {
      int processId;
      /// Get the location of the library to inject
      string libraryLocation = CoreBus.Configuration.AppConfig.LibtoInject;
      RemoteHooking.CreateAndInject(
        Path.Combine(_startInfo.WorkingDirectory.File, _startInfo.Files.Executable.File),
        /// Optional command line parameters for process creation
        _startInfo.Arguments,
        /// ProcessCreationFlags, no conditions are set on the created process.
        0,
        /// Absolute paths of the libraries to inject, we use the same one for 32bit and 64bit
        libraryLocation, libraryLocation,
        /// The process ID of the newly created process
        out processId,
        /// Extra parameters being passed to the injected library entry points Run() and Initialize()
        _channelName);
      /// The process has been created, set the _process variable.
      _process = SystemProcess.GetProcessById(processId, SystemProcess.GetCurrentProcess().MachineName);
      _process.EnableRaisingEvents = true;
      _process.Exited += Process_Exited;
      CoreBus.Log.Message("A virtualized process with PID {0} has been succesfully created for {1}.",
                              processId, _startInfo.Files.Executable.File);
    }

    /// <summary>
    /// [Not Implemented] Wraps and injects a process, used for .NET applications.
    /// </summary>
    private void WrapAndInject()
    {
      /// Start wrapper.
      /// Inject wrapper.
      /// Let wrapper load the assembly.
      /// Let wrapper install the hooks.
      /// Let wrapper call Main()
      throw new NotImplementedException("The wrapper has not been implemented yet.");
    }

    /// <summary>
    /// Handles the <see cref="_process"/>.Exited event;
    /// Sets <see cref="_hasExited"/> and raises <see cref="_exited"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Process_Exited(object sender, EventArgs e)
    {
      if (Thread.CurrentThread.Name == null)
        Thread.CurrentThread.Name = "Process Finalizer";
      CoreBus.Log.Message("Process Exited event is called");
      if (!_process.HasExited)
        return;
      _hasExited = true;
      lock (_exitEventSyncRoot)
      {
        if (WinError.Succeeded(_process.ExitCode))
          _exited(this, ExitCode.Success);
        else
        {
          CoreBus.Log.Error("Process exited with ExitCode [{0}] {1} and message {2}",
            _process.ExitCode, WinError.GetErrorName((uint)_process.ExitCode), _process.StandardError.ReadToEnd());
          _exited(this, ExitCode.Error);
        }
      }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
