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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using AppStract.Core.System.IPC;
using AppStract.Server;
using AppStract.Utilities.Helpers;
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
      // Name the current thread.
      if (Thread.CurrentThread.Name == null)
        Thread.CurrentThread.Name = "Initializer";
      // Connect to server.
      IProcessSynchronizer sync = RemoteHooking.IpcConnectClient<ProcessSynchronizerInterface>(inChannelName).ProcessSynchronizer;
      // Initialize the guest's core.
      GuestCore.Initialize(sync);
      // Validate connection.
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
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Run method for the injected library, invoked by EasyHook.
    /// </summary>
    /// <remarks>
    /// The GC won't unload the library as long as this method doesn't return.
    /// <br />
    /// The Run() method should return if you want the injected library needs to be unloaded.
    /// Unhandled exceptions ARE NOT redirected automatically.
    /// As the connection to the host is established in <see cref="GuestCore.Initialize"/>,
    /// errors should be reported using <see cref="GuestCore.Log"/>.
    /// </remarks>
    /// <param name="inContext">Information about the environment in which the library main method has been invoked, used by the EasyHook library.</param>
    /// <param name="channelName">The name of the inter-process communication channel to connect to, used by the EasyHook library.</param>
    public void Run(RemoteHooking.IContext inContext, string channelName)
    {
      try
      {
        InitializeForRun();
        // Start the injected process.
        RemoteHooking.WakeUpProcess();
        // Block the current thread until the host becomes unreachable,
        // this keeps the GC from collecting the current EntryPoint.
        BlockThread();
      }
      catch (Exception e)
      {
        GuestCore.Log.Critical("An unexpected exception occured.", e);
        // Exit code 1067 = ERROR_PROCESS_ABORTED "The process terminated unexpectedly."
        if (!GuestCore.TerminateProcess(1067, ExitMethod.Request | ExitMethod.Kill))
          throw new ApplicationException("An unexpected fatal exception occured.", e);
      }
    }

    /// <summary>
    /// Run method for the injected library, invoked by EasyHook when injecting a wrapper process.
    /// </summary>
    /// <remarks>
    /// The GC won't unload the library as long as this method doesn't return.
    /// <br />
    /// The Run() method should return if you want the injected library needs to be unloaded.
    /// Unhandled exceptions ARE NOT redirected automatically.
    /// As the connection to the host is established in <see cref="GuestCore.Initialize"/>,
    /// errors should be reported using <see cref="GuestCore.Log"/>.
    /// </remarks>
    /// <param name="inContext">Information about the environment in which the library main method has been invoked, used by the EasyHook library.</param>
    /// <param name="channelName">The name of the inter-process communication channel to connect to, used by the EasyHook library.</param>
    /// <param name="wrappedProcessExecutable">The executable containing the main method of the guest process.</param>
    /// <param name="args">Optional arguments to pass to the guest's main method.</param>
    public void Run(RemoteHooking.IContext inContext, string channelName, string wrappedProcessExecutable, string args)
    {
      try
      {
        InitializeForRun();
        // Set the working directory to the one expected by the executable.
        Directory.SetCurrentDirectory(Path.GetDirectoryName(wrappedProcessExecutable));
        // Run the main method of the wrapped process.
        string[] arguments = args.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        GuestCore.Log.Debug("Invoking main method of targeted guest... using #{0} method parameters{1}",
                            arguments.Length, arguments.Length == 0 ? "" : ": " + args);
        Debugger.Break();
        System.Reflection.MethodInfo entrypoint;
        using (Server.Hooking.HookManager.ACL.GetHookingExclusion())
        {
          var assembly = System.Reflection.Assembly.LoadFrom(wrappedProcessExecutable);
          entrypoint = assembly.EntryPoint;
        }
        entrypoint.Invoke(null, null);
        var exitCode = AssemblyHelper.RunMainMethod(wrappedProcessExecutable, arguments.Length == 0 ? null : arguments);
        GuestCore.Log.Message("Target main method returned exitcode " + exitCode);
        // First attempt a clean shutdown, then try a forced shutdown.
        GuestCore.TerminateProcess(exitCode, ExitMethod.Request | ExitMethod.Kill);
      }

      catch (Exception e)
      {
#if DEBUG
        if (Debugger.IsAttached)
          Debugger.Break();
#endif
        GuestCore.Log.Critical("An unexpected exception occured.", e);
        // Exit code 1067 = ERROR_PROCESS_ABORTED "The process terminated unexpectedly."
        if (!GuestCore.TerminateProcess(1067, ExitMethod.Request | ExitMethod.Kill))
          throw new ApplicationException("An unexpected fatal exception occured.", e);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Contains the actions that both <see cref="Run"/> methods must execute
    /// before executing any of the more specific code.
    /// </summary>
    private void InitializeForRun()
    {
#if DEBUG
      if (MessageBox.Show("Do you want to attach a debugger to the current process?", "Attach Debugger?",
                          MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
          == DialogResult.Yes)
        Debugger.Launch();
#endif
      // Name the current thread.
      if (Thread.CurrentThread.Name == null)
        Thread.CurrentThread.Name = "EntryPoint";
      // Validate the connection.
      if (!GuestCore.Connected)
        return; // Return silently, can't log
      // Install all hooks.
      GuestCore.InstallHooks(this);
    }

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
