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
using AppStract.Core.Logging;
using AppStract.Core.Virtualization.Synchronization;
using AppStract.Server;
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
    public ProcessEntryPoint(RemoteHooking.IContext inContext, string inChannelName)
    {
      /// Connect to server.
      IProcessSynchronizer sync = RemoteHooking.IpcConnectClient<ProcessSynchronizerInterface>(inChannelName).ProcessSynchronizer;
      /// Initialize the guest's core.
      GuestCore.Initialize(sync);
      /// Validate connection.
      if (!GuestCore.ValidConnection)
        throw new GuestException("Failed to validate the inter-process connection while initializing the guest's process.");
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
    public void Run(RemoteHooking.IContext inContext, string channelName)
    {
      /// Name the current thread.
      Thread.CurrentThread.Name = string.Format("{0} (PID {1}) Run method",
        Process.GetCurrentProcess().ProcessName, RemoteHooking.GetCurrentProcessId());
      /// Validate the connection.
      if (!GuestCore.ValidConnection)
        return; /// Return silently, can't log
      GuestCore.Log(new LogMessage(
        LogLevel.Information, "Guest process [{0}] entered the Run method.", GuestCore.ProcessId));
      /// Install all hooks.
      GuestCore.InstallHooks(this);
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
    private static void BlockThread()
    {
      while (true)
      {
        Thread.Sleep(500);
        if (!GuestCore.ValidConnection)
          return;
      }
    }

    #endregion

  }
}
