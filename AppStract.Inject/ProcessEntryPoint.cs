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
using AppStract.Core.Virtualization.Synchronization;
using AppStract.Server.FileSystem;
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
    private readonly IFileSystemSynchronizer _fileSystemSynchronizer;
    private readonly IRegistrySynchronizer _registrySynchronizer;
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
    public ProcessEntryPoint(RemoteHooking.IContext inContext, string inChannelName, ResourceSynchronizer resourceSynchronizer)
    {
      /// Connect to server.
      RemoteHooking.IpcConnectClient<ResourceSynchronizer>(inChannelName);
      
      _serverReporter = resourceSynchronizer;
      _fileSystemSynchronizer = resourceSynchronizer;
      _registrySynchronizer = resourceSynchronizer;
      /// Initialize variables.
      /// BUG: Add a new variable to the constructor to get the root folder!
      _fileSystem = new VirtualFileSystem(null, _fileSystemSynchronizer);
      _registry = new RegistryProvider(_registrySynchronizer);
      _hookImplementations = new HookImplementations(_fileSystem, _registry);
      /// Validate connection.
      _serverReporter.Ping("Process [PID" + RemoteHooking.GetCurrentProcessId() + "] is initialized and validated.");
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
    public void Run(RemoteHooking.IContext inContext, String channelName, ResourceSynchronizer resourceSynchronizer)
    {
      /// Name the current thread.
      Thread.CurrentThread.Name = string.Format("{0} (PID {1})",
        Process.GetCurrentProcess().ProcessName, RemoteHooking.GetCurrentProcessId());
      /// Validate the connection.
      _serverReporter.Ping("Process [PID" + RemoteHooking.GetCurrentProcessId() + "] is running.");
      /// Install all hooks.
      HookManager.Initialize(this, _hookImplementations);
      HookManager.InstallHooks();
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
