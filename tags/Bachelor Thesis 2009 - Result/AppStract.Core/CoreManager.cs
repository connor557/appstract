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
using AppStract.Core.Data.Application;
using AppStract.Core.Data.Settings;
using AppStract.Core.Logging;
using AppStract.Core.Virtualization.Process;

namespace AppStract.Core
{
  public static class CoreManager
  {

    #region Variables

    private static VirtualizedProcess _process;

    #endregion

    #region Constructors

    static CoreManager()
    {
      /// Binding this event might cause a SecurityException.
      /// This exception is not catched because it indicates that the process is running with insufficient rights.
      AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Occurs when the OS is querying the current process to exit.
    /// Actions taken by an <see cref="EventHandler"/> must be handled as quick as possible.
    /// </summary>
    public static event EventHandler Exiting;

    /// <summary>
    /// Initializes the <see cref="CoreBus"/> 
    /// and it's <see cref="CoreBus.Configuration"/> and <see cref="CoreBus.Log"/>.
    /// </summary>
    public static void InitializeCore()
    {
#if DEBUG
      CoreBus.Log = new ConsoleLogger();
#else
      /// How to initialize the log service without configuration?
      /// How to initialize the configuration without logservice?
      throw new NotImplementedException();
#endif
      CoreBus.Configuration = Configuration.LoadConfiguration();
    }

    /// <summary>
    /// Starts a process from the applicationdata loaded from the default startup file.
    /// </summary>
    /// <exception cref="CoreException">
    /// A <see cref="CoreException"/> is thrown if the process can't be started.
    /// </exception>
    public static void StartProcess()
    {
      var appFile = CoreBus.Configuration.AppConfig.DefaultApplicationDataFile;
      StartProcess(appFile);
    }

    /// <summary>
    /// Starts a process from the applicationdata loaded from the filename specified.
    /// </summary>
    /// <exception cref="CoreException">
    /// A <see cref="CoreException"/> is thrown if the process can't be started.
    /// </exception>
    /// <param name="applicationDataFile">
    /// The file to load the <see cref="ApplicationData"/> from,
    /// representing the application to start.
    /// </param>
    public static void StartProcess(string applicationDataFile)
    {
      var data = ApplicationData.Load(applicationDataFile);
      if (data == null)
        throw new CoreException(applicationDataFile
                                + " could not be found or contains invalid data while trying"
                                + " to start a new process based on this file.");
      var workingDirectory = new ApplicationFile(Path.GetDirectoryName(applicationDataFile));
      var startInfo = new VirtualProcessStartInfo(data, workingDirectory);
      _process = VirtualizedProcess.Start(startInfo);
    }

    #endregion

    #region Private Methods

    private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
    {
      if (Exiting != null)
        Exiting(sender, e);
    }

    #endregion

  }
}
