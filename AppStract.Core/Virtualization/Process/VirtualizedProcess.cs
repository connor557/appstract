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
using AppStract.Core.Data.Application;
using AppStract.Core.Virtualization.Synchronization;
using EasyHook;
using SystemProcess = System.Diagnostics.Process;

namespace AppStract.Core.Virtualization.Process
{
  public class VirtualizedProcess : IDisposable
  {

    #region Variables

    /// <summary>
    /// All data related to the application run in the current <see cref="VirtualizedProcess"/>.
    /// </summary>
    private readonly VirtualProcessStartInfo _startInfo;
    /// <summary>
    /// The object responsible for the synchronization
    /// between the current process and the virtualized process.
    /// </summary>
    private readonly ResourceSynchronizer _resourceSynchronizer;
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
    /// The object to lock while initializing EasyHook.
    /// </summary>
    private readonly object _easyHookSyncRoot;

    #endregion

    #region Constructors

    private VirtualizedProcess(VirtualProcessStartInfo startInfo)
    {
      _easyHookSyncRoot = new object();
      _startInfo = startInfo;
      _resourceSynchronizer = new ResourceSynchronizer(startInfo.DatabaseFileSystem,
                                                       startInfo.DatabaseRegistry);
    }

    #endregion

    #region Public Methods

    public static VirtualizedProcess StartProcess(VirtualProcessStartInfo startInfo)
    {
      /// Create an instance of VirtualizedProcess.
      VirtualizedProcess process = new VirtualizedProcess(startInfo);
      /// Initializes the underlying resources.
      process.InitEasyHook();
      /// Start the process.
      if (startInfo.Executable.Type == FileType.Assembly_Native)
        process.CreateAndInject();
      else if (startInfo.Executable.Type == FileType.Assembly_Managed)
        process.WrapAndInject();
      else  /// This should never happen.
        throw new VirtualProcessException("FileType " + startInfo.Executable.Type +
                                          " can't be used to start a process with.");
      return process;
    }

    public void KillProcess()
    {
      
    }

    #endregion

    #region Private Methods

    private void InitEasyHook()
    {
      lock (_easyHookSyncRoot)
      {
        if (_iniEasyHook)
          return;
        Config.Register("AppStract", ServiceCore.Configuration.AppConfig.LibsToRegister.ToArray());
        RemoteHooking.IpcCreateServer<ResourceSynchronizer>(ref _channelName, WellKnownObjectMode.SingleCall);
        _iniEasyHook = true;
      }
    }

    private void CreateAndInject()
    {
      int processId;
      /// Get the location of the library to inject
      string libraryLocation = ServiceCore.Configuration.AppConfig.LibtoInject;
      RemoteHooking.CreateAndInject(
        Path.Combine(ServiceCore.Configuration.DynConfig.Root, _startInfo.Executable.File),
        /// Optional command line parameters for process creation
        "",
        /// ProcessCreationFlags, no conditions are set on the created process.
        0,
        /// Absolute paths of the libraries to inject, we use the same one for 32bit and 64bit
        libraryLocation, libraryLocation,
        /// The process ID of the newly created process
        out processId,
        /// Extra parameters being passed to the injected library entry points Run() and Initialize()
        _channelName, _resourceSynchronizer);
      /// The process has been created, set the _process variable.
      _process = SystemProcess.GetProcessById(processId, SystemProcess.GetCurrentProcess().MachineName);
      _process.EnableRaisingEvents = true;
      _process.Exited += Process_Exited;
      ServiceCore.Log.Message("A virtualized process with PID {0} has been succesfully created for {1}.",
                              processId, _startInfo.Executable.File);
    }

    private void WrapAndInject()
    {
      /// Start wrapper.
      /// Inject wrapper.
      /// Let wrapper load the assembly.
      /// Let wrapper install the hooks.
      /// Let wrapper call Main()
      throw new NotImplementedException("The wrapper has not been implemented yet.");
    }

    private void Process_Exited(object sender, EventArgs e)
    {
      //if (!_process.HasExited)
      //  return;
      //if (_process.ExitCode == 0)
      //  SetStateChange(ProcessState.Exited);
      //else
      //  SetStateChange(ProcessState.Failed);
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
