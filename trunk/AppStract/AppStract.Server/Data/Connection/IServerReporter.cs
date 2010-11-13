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
using System.Collections.Generic;
using AppStract.Utilities.Logging;

namespace AppStract.Engine.Data.Connection
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
    /// Reports a message that must be logged, to the host process.
    /// </summary>
    /// <param name="message">Message to report.</param>
    void ReportMessage(LogMessage message);

    /// <summary>
    /// Reports a serie of messages that must be logged, to the host process.
    /// </summary>
    /// <param name="message">Message to report.</param>
    void ReportMessage(IEnumerable<LogMessage> message);

    /// <summary>
    /// Returns the maximum <see cref="LogLevel"/> needed by <see cref="ReportMessage"/>
    /// in order to report messages. Calling <see cref="ReportMessage"/> for <see cref="LogMessage"/>s
    /// with a higher <see cref="LogLevel"/> is a waste of resource.
    /// </summary>
    /// <returns></returns>
    LogLevel GetRequiredLogLevel();

  }
}
