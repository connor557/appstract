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
using System.Runtime.InteropServices;
using System.Threading;
using AppStract.Core;
using AppStract.Core.Data.Settings;
using AppStract.Core.Logging;

namespace AppStract.Host
{
  /// <summary>
  /// The entry class for AppStract.Host,
  /// which is a console application able to run packaged applications in a virtual environment.
  /// </summary>
  public class ApplicationLauncher
  {

    #region Constants

    /// <summary>
    /// The title of the console window.
    /// </summary>
    private const string _consoleTitle = "AppStract - Host";

    #endregion

    #region Main

    static void Main(string[] args)
    {
      Thread.CurrentThread.Name = "Main";
      Console.Title = _consoleTitle;
      InitializeCore();
      ConfigureFromArgs(args);
      /// Start application...
#if DEBUG
      Console.WriteLine("\r\n\r\nLogic starts here...");
      Console.ReadLine();
#endif
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the <see cref="ServiceCore"/> 
    /// and it's <see cref="ServiceCore.Configuration"/> and <see cref="ServiceCore.Log"/>.
    /// </summary>
    public static void InitializeCore()
    {
#if DEBUG
      ServiceCore.Log = new ConsoleLogger();
#else
      /// How to initialize the log service without configuration?
      /// How to initialize the configuration without logservice?
      throw new NotImplementedException();
#endif
      ServiceCore.Configuration = Configuration.LoadConfiguration();
    }

    /// <summary>
    /// Configures the application with settings specified in <paramref name="args"/>.
    /// </summary>
    /// <param name="args"></param>
    public static void ConfigureFromArgs(string[] args)
    {
      var parser = new CommandlineParser(args);
      parser.Parse();
      if (parser.IsDefined(CommandlineOption.LogOutput))
      {
        if (parser.IsDefined(CommandlineOption.LogFile))
          TrySetLogOutput(parser.GetOption(CommandlineOption.LogOutput),
                          parser.GetOption(CommandlineOption.LogFile));
        else
          TrySetLogOutput(parser.GetOption(CommandlineOption.LogOutput));
      }
      else if (parser.IsDefined(CommandlineOption.LogFile)
        && ServiceCore.Log.Type == LogType.File)
        TrySetLogOutput(LogType.File, parser.GetOption(CommandlineOption.LogFile));
      if (parser.IsDefined(CommandlineOption.LogLevel))
        TrySetLogLevel(parser.GetOption(CommandlineOption.LogLevel));
      if (parser.IsDefined(CommandlineOption.ShowWindow))
        SetWindowState(parser.GetOption(CommandlineOption.ShowWindow));
      if (parser.IsDefined(CommandlineOption.ApplicationDataFile))
        throw new NotImplementedException();
    }

    #endregion

    #region Private Methods

    private static void TrySetLogLevel(object logLevel)
    {
      var logLevelType = typeof (LogLevel);
      if (!Enum.IsDefined(logLevelType, logLevel))
        return;
      var level = (LogLevel) Enum.Parse(logLevelType, logLevel.ToString());
      ServiceCore.Log.LogLevel = level;
    }

    private static void TrySetLogOutput(object logType)
    {
      TrySetLogOutput(logType, null);
    }

    private static void TrySetLogOutput(object logType, object file)
    {
      var logTypeType = typeof (LogType);
      if (!Enum.IsDefined(logTypeType, logType))
        return;
      var type = (LogType) Enum.Parse(logTypeType, logType.ToString());
      if (ServiceCore.Log.Type == type)
        return;
      if (type == LogType.Null)
        ServiceCore.Log = new NullLogger();
      else if (type == LogType.Console)
        ServiceCore.Log = new ConsoleLogger(ServiceCore.Log.LogLevel);
      else if (type == LogType.File)
        ServiceCore.Log
          = file == null
              ? FileLogger.CreateLogService(ServiceCore.Log.LogLevel)
              : FileLogger.CreateLogService(ServiceCore.Log.LogLevel, file.ToString());
    }

    private static void SetWindowState(object showWindow)
    {
      bool visible = showWindow.ToString() == "1"
                     || showWindow.ToString().ToLowerInvariant() == "true";
      if (visible)
        return;
      /// Hide the window.
      IntPtr hWnd = FindWindow(null, _consoleTitle);
      if (hWnd != IntPtr.Zero)
        ShowWindow(hWnd, 0); /// 0 = SW_HIDE
                             /// 1 = SW_SHOWNORMA  
      /// Else, do nothing.
      /// There is no console window when this code is called from AppStract.Packager.
    }

    #endregion

    #region DLL Imports

    /// <summary>
    /// The FindWindow function retrieves a handle to the top-level window whose class name
    /// and window name match the specified strings. This function does not search child windows.
    /// This function does not perform a case-sensitive search.
    /// </summary>
    /// <param name="lpClassName"></param>
    /// <param name="lpWindowName"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    /// <summary>
    /// The ShowWindow function sets the specified window's show state.
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="nCmdShow"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    #endregion

  }
}
