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
using AppStract.Core;
using AppStract.Core.Logging;

namespace AppStract.Host
{
  /// <summary>
  /// The entry class for AppStract.Host,
  /// which is a console application able to run packaged applications in a virtual environment.
  /// </summary>
  class ApplicationLauncher
  {

    #region Constants

    /// <summary>
    /// The title of the console window.
    /// </summary>
    private const string _ConsoleTitle = "AppStract - Host";

    #endregion

    #region Private Methods

    /// <summary>
    /// The main entry point for AppStract.Host.
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
      System.Threading.Thread.CurrentThread.Name = "Main";
      Console.Title = _ConsoleTitle;
      CoreManager.InitializeCore();
      var parser = new CommandlineParser(args);
      ConfigureFromArgs(parser);
#if !DEBUG
      try
      {
#endif
        if (parser.IsDefined(CommandlineOption.ApplicationDataFile))
          CoreManager.StartProcess(parser.GetOption(CommandlineOption.ApplicationDataFile).ToString());
        else
          CoreManager.StartProcess();
#if !DEBUG
      }
      catch(Exception ex)
      {
        CoreBus.Log.Critical("A fatal exception occured.", ex);
        CoreBus.Configuration.ShowWindow(true, _consoleTitle);
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
        if (ParserHelper.TryParseLogType(argParser.GetOption(CommandlineOption.LogOutput), out type))
        {
          if (argParser.IsDefined(CommandlineOption.LogFile))
            CoreBus.Configuration.SetLogOutput(type, argParser.GetOption(CommandlineOption.LogFile).ToString());
          else
            CoreBus.Configuration.SetLogOutput(type);
        }
      }
      else if (argParser.IsDefined(CommandlineOption.LogFile)
               && CoreBus.Log.Type == LogType.File)
      {
        CoreBus.Configuration.SetLogOutput(LogType.File, argParser.GetOption(CommandlineOption.LogFile).ToString());
      }
      if (argParser.IsDefined(CommandlineOption.LogLevel))
      {
        LogLevel logLevel;
        if (ParserHelper.TryParseLogLevel(argParser.GetOption(CommandlineOption.LogLevel), out logLevel))
          CoreBus.Configuration.SetLogLevel(logLevel);
      }
      if (argParser.IsDefined(CommandlineOption.ShowWindow))
      {
        object showWindow = argParser.GetOption(CommandlineOption.ShowWindow);
        bool sWindow = showWindow.ToString() == "1"
                       || showWindow.ToString().ToLowerInvariant() == "true";
        CoreBus.Configuration.ShowWindow(sWindow, _ConsoleTitle);
      }
    }

    #endregion

  }
}
