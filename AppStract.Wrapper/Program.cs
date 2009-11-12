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
using System.Threading;
using AppStract.Core.System.Logging;
using AppStract.Server;

namespace AppStract.Wrapper
{
  class Program
  {

    #region Variables

    private static bool _exit;
    private static int _exitCode;

    #endregion

    #region Private Methods

    /// <summary>
    /// Entry point for the wrapper process.
    /// Hangs until the connection with the server process is lost
    /// or until the <see cref="_exit"/> flag is set to true.
    /// </summary>
    /// <param name="args"></param>
    private static int Main(string[] args)
    {
      AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
      GuestCore.ExitRequestRaised += ExitRequestEventHandler;
      while (!_exit)
      {
        Thread.Sleep(500);
        if (GuestCore.Initialized && !GuestCore.Connected)
          break;
      }
      GuestCore.Log(new LogMessage(LogLevel.Debug, "Main method returning with exit code " + _exitCode), false);
      return _exitCode;
    }

    /// <summary>
    /// Tries to log unhandled exceptions to the server process.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      GuestCore.Log(new LogMessage(LogLevel.Critical, "Target process threw an unhandled exception.", e.ExceptionObject), false);
      // Not sure if the following is necessary since the process is already dying from the unhandled exception
      _exitCode = -1;
      _exit = true;
    }

    /// <summary>
    /// Eventhandler for the <see cref="GuestCore.ExitRequestRaised"/> event.
    /// </summary>
    /// <param name="exitCode"></param>
    /// <returns></returns>
    private static bool ExitRequestEventHandler(int exitCode)
    {
      _exitCode = exitCode;
      _exit = true;
      return true;
    }

    #endregion

  }
}
