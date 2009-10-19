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
using AppStract.Core.System.Logging;

namespace AppStract.Core.System.Synchronization
{
  /// <summary>
  /// Enables different processes to test connectivity
  /// and to send simple messages from the guest to the host.
  /// </summary>
  public interface IServerReporter
  {

    /// <summary>
    /// Pings the host process. If the ping fails, an exception is thrown.
    /// </summary>
    /// <exception cref="Exception">
    /// Ping failed.
    /// </exception>
    void Ping();

    /// <summary>
    /// Reports an <see cref="Exception"/> to the host process.
    /// </summary>
    /// <param name="exception"><see cref="Exception"/> to report.</param>
    void ReportException(Exception exception);

    /// <summary>
    /// Reports an <see cref="Exception"/> to the host process.
    /// </summary>
    /// <param name="exception"><see cref="Exception"/> to report.</param>
    /// <param name="message">An associated message.</param>
    void ReportException(Exception exception, string message);

    /// <summary>
    /// Reports a message that must be logged, to the host process.
    /// </summary>
    /// <param name="message">Message to report.</param>
    void ReportMessage(LogMessage message);

    /// <summary>
    /// Returns the maximum <see cref="LogLevel"/> needed by <see cref="ReportMessage"/>
    /// in order to report messages. Calling <see cref="ReportMessage"/> for <see cref="LogMessage"/>s
    /// with a higher <see cref="LogLevel"/> is a waste of resource.
    /// </summary>
    /// <returns></returns>
    LogLevel GetRequiredLogLevel();

  }
}
