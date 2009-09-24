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
using AppStract.Utilities.Assembly;
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
    /// All business logic should be invoked in the <see cref="Run"/> method.
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
      if (!GuestCore.Connected)
        throw new GuestException("Failed to validate the inter-process connection while initializing the guest's virtual environment.");
    }

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
    /// <param name="wrappedProcessExecutable">The executable to start the Main() method of.</param>
    /// <param name="args">The arguments to pass to the main method of <paramref name="wrappedProcessExecutable"/>.</param>
    public ProcessEntryPoint(RemoteHooking.IContext inContext, string inChannelName, string wrappedProcessExecutable, string args)
      : this(inContext, inChannelName)
    {
      /// Install all hooks.
      GuestCore.InstallHooks(this);
      /// Block a thread until the host becomes unreachable,
      /// this should keep the GC from collecting the current EntryPoint.
      new Thread(BlockThread).Start();
      /// Run the main method of the wrapped process.
      GuestCore.Log(new LogMessage(LogLevel.Debug, "Invoking main method of virtualized process..."));
      AssemblyHelper.RunMainMethod(wrappedProcessExecutable,
                                   args.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The run code for the injected library.
    /// </summary>
    /// <remarks>
    /// The GC won't unload the library as long as this method doesn't return.
    /// <br />
    /// Run(IContext, params) should return if you want the injected library needs to be unloaded.
    /// Unhandled exceptions ARE NOT redirected automatically.
    /// As the connection to the host is established in <see cref="GuestCore.Initialize"/>,
    /// errors should be reported using the <see cref="GuestCore.Log(AppStract.Core.Logging.LogMessage)"/> function.
    /// </remarks>
    /// <param name="inContext">Information about the environment in which the library main method has been invoked, used by the EasyHook library.</param>
    /// <param name="channelName">The name of the inter-process communication channel to connect to, used by the EasyHook library.</param>
    public void Run(RemoteHooking.IContext inContext, string channelName)
    {
      try
      {
        /// Name the current thread.
        if (Thread.CurrentThread.Name == null)
            Thread.CurrentThread.Name = string.Format("{0} (PID {1}) Run method",
              Process.GetCurrentProcess().ProcessName, RemoteHooking.GetCurrentProcessId());
        /// Validate the connection.
        if (!GuestCore.Connected)
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
      catch (Exception e)
      {
        GuestCore.Log(new LogMessage(LogLevel.Critical, "An unexpected exception occured.", e),
                      false);
        Process.GetCurrentProcess().Kill();
      }
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
        if (!GuestCore.Connected)
          return;
      }
    }

    #endregion

  }
}
