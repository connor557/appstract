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
using System.Diagnostics;
using System.Threading;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Registry;
using AppStract.Core.Virtualization.Synchronization;
using AppStract.Server;
using AppStract.Server.FileSystem;
using AppStract.Server.Hooking;
using AppStract.Server.Registry;
using EasyHook;

namespace AppStract.Inject
{
  /// <summary>
  /// The entrypoint for the injected library.
  /// </summary>
  /// <remarks>
  /// EasyHook looks for a class implementing <see cref="IEntryPoint"/>.
  /// If such class is found, EasyHook instantiates it
  /// with the variables provided in <see cref="RemoteHooking.CreateAndInject"/>
  /// and then calls the <see cref="Run"/> method.
  /// </remarks>
  public class ProcessEntryPoint : IEntryPoint
  {

    #region Variables

    private readonly IServerReporter _serverReporter;
    private readonly IFileSystemLoader _fileSystemSynchronizer;
    private readonly IRegistryLoader _registrySynchronizer;
    private readonly CommunicationBus _commBus;
    private readonly HookImplementations _hookImplementations;
    private readonly VirtualFileSystem _fileSystem;
    private readonly RegistryProvider _registry;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ProcessEntryPoint"/>.
    /// </summary>
    /// <remarks>
    /// This call should only focus on initializing the variables and connecting to the host application.
    /// All business logic should be invoked in the Run() method.
    /// Unhandled exception are redirected to the host application automatically.
    /// </remarks>
    /// <param name="inContext">Information about the environment in which the library main method has been invoked.</param>
    /// <param name="inChannelName">The name of the inter-process communication channel to connect to.</param>
    /// <param name="resourceSynchronizer"></param>
    public ProcessEntryPoint(RemoteHooking.IContext inContext, string inChannelName, ProcessSynchronizer resourceSynchronizer)
    {
      /// Connect to server.
      RemoteHooking.IpcConnectClient<ProcessSynchronizer>(inChannelName);
      /// Initialize variables.
      _serverReporter = resourceSynchronizer;
      _commBus = new CommunicationBus(resourceSynchronizer, resourceSynchronizer);
      _fileSystemSynchronizer = _commBus;
      _registrySynchronizer = _commBus;
      _hookImplementations = new HookImplementations(_fileSystem, _registry);
      /// Validate connection.
      _serverReporter.Ping();
      int processId = RemoteHooking.GetCurrentProcessId();
      _serverReporter.ReportMessage("Process [PID" + processId + "] is initialized and validated.");
      /// Load resources.
      _serverReporter.ReportMessage("Process [PID" + processId + "] will now load the file system and registry data.");
      _fileSystem = new VirtualFileSystem(null);  /// BUG: Add a new variable to the constructor to get the root folder!
      _fileSystem.LoadFileTable(_fileSystemSynchronizer);
      _serverReporter.ReportMessage("Process [PID" + processId + "] succesfully loaded the file system.");
      _registry = new RegistryProvider();
      _registry.LoadRegistry(_registrySynchronizer);
      _serverReporter.ReportMessage("Process [PID" + processId + "] succesfully loaded the registry.");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The run code for the injected library.
    /// The GC won't unload the library as long as this method doesn't return.
    /// </summary>
    /// <remarks>
    /// Run(IContext, params) should return if you want the injected library needs to be unloaded.
    /// Unhandled exceptions ARE NOT redirected automatically. As the connection to the host is made in Initialize().
    /// Errors should be reported using ProcessHook (MarshalByRefObj).
    /// </remarks>
    /// <param name="inContext">Information about the environment in which the library main method has been invoked.</param>
    /// <param name="channelName">The name of the inter-process communication channel to connect to.</param>
    /// <param name="resourceSynchronizer"></param>
    public void Run(RemoteHooking.IContext inContext, String channelName, ProcessSynchronizer resourceSynchronizer)
    {
      /// Name the current thread.
      Thread.CurrentThread.Name = string.Format("{0} (PID {1})",
        Process.GetCurrentProcess().ProcessName, RemoteHooking.GetCurrentProcessId());
      /// Validate the connection.
      _serverReporter.Ping();
      /// Install all hooks.
      HookManager.Initialize(this, _hookImplementations);
      HookManager.InstallHooks();
      _serverReporter.ReportMessage("Process [PID" + RemoteHooking.GetCurrentProcessId() + "] is hooked.");
      _serverReporter.ReportMessage("Process [PID" + RemoteHooking.GetCurrentProcessId() + "] is waking up.");
      /// Start the injected process.
      RemoteHooking.WakeUpProcess();
      /// Block the current thread until the host becomes unreachable,
      /// this keeps the GC from collecting the current EntryPoint.
      BlockThread();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Doesn't return as long as the host is reachable.
    /// </summary>
    private void BlockThread()
    {
      while (true)
      {
        Thread.Sleep(500);
        if (!IsHostReachable())
          return;
      }
    }

    /// <summary>
    /// Returns whether the host is reachable.
    /// </summary>
    /// <returns>Is the host reachable?</returns>
    private bool IsHostReachable()
    {
      try
      {
        _serverReporter.Ping();
      }
      catch (Exception)
      {
        return false;
      }
      return true;
    }

    #endregion

  }
}
