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
using System.Windows.Forms;
using AppStract.Host;
using AppStract.Utilities.Helpers;
using AppStract.Utilities.Logging;

namespace AppStract.Manager
{
  /// <summary>
  /// The entry class for AppStract.Manager,
  /// which is the application responsible for packaging an application
  /// so it can be run in a virtual environment using AppStract.Host.
  /// </summary>
  static class Program
  {

    /// <summary>
    /// The main entry point for the host.
    /// </summary>
    /// <param name="args"></param>
    [STAThread]
    static void Main(string[] args)
    {
      System.Threading.Thread.CurrentThread.Name = "Main";
      var parser = new CommandlineParser(args);
      CoreManager.InitializeCore();
#if !DEBUG
      try
      {
#endif
      if (parser.HasDefinitions)
      {
        ConfigureFromArgs(parser);
        CoreManager.StartProcess(parser.IsDefined(CommandlineOption.ApplicationDataFile)
                                   ? parser.GetOption(CommandlineOption.ApplicationDataFile)
                                   : CoreBus.Configuration.Application.DefaultApplicationDataFile);
      }
      else
      {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FrmManager());
      }
#if !DEBUG
      }
      catch(Exception ex)
      {
      // ToDo: Refactor following code to use WinForm in stead of console window
        CoreBus.Log.Critical("A fatal exception occured.", ex);
        ProcessHelper.SetWindowState(WindowShowStyle.ShowNormal);
        Console.WriteLine("\r\n\r\n\r\n\r\n\r\n\r\n");
        Console.WriteLine("############################################################\r\n");
        Console.WriteLine(" A fatal exception occured, see below for more information.\r\n");
        Console.WriteLine("############################################################\r\n");
        Console.WriteLine(ex.GetType());
        Console.WriteLine(" -> " + ex.Message);
      }
#endif
      Console.WriteLine("\r\n\r\nPress any key to exit.\r\n");
      Console.ReadLine();
    }

    /// <summary>
    /// Configures the application with settings extracted from <paramref name="argParser"/>.
    /// </summary>
    /// <param name="argParser"></param>
    private static void ConfigureFromArgs(CommandlineParser argParser)
    {
      if (argParser.IsDefined(CommandlineOption.LogOutput))
      {
        LogType type;
        if (ParserHelper.TryParseEnum(argParser.GetOption(CommandlineOption.LogOutput), out type))
        {
          if (argParser.IsDefined(CommandlineOption.LogFile))
            CoreBus.Configuration.SetLogOutput(type, argParser.GetOption(CommandlineOption.LogFile));
          else
            CoreBus.Configuration.SetLogOutput(type);
        }
      }
      else if (argParser.IsDefined(CommandlineOption.LogFile)
               && CoreBus.Log.Type == LogType.File)
      {
        CoreBus.Configuration.SetLogOutput(LogType.File, argParser.GetOption(CommandlineOption.LogFile));
      }
      if (argParser.IsDefined(CommandlineOption.LogLevel))
      {
        LogLevel logLevel;
        if (ParserHelper.TryParseEnum(argParser.GetOption(CommandlineOption.LogLevel), out logLevel))
          CoreBus.Configuration.SetLogLevel(logLevel);
      }
      if (argParser.IsDefined(CommandlineOption.ShowWindow))
      {
        var showWindow = argParser.GetOption(CommandlineOption.ShowWindow);
        if (showWindow != "1" && showWindow.ToUpperInvariant() != "TRUE")
          ProcessHelper.SetWindowState(WindowShowStyle.Hide);
      }
    }

  }
}
