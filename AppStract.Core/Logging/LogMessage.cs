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

namespace AppStract.Core.Logging
{
  /// <summary>
  /// Represents a message that needs to be written to the log.
  /// </summary>
  [Serializable]
  public struct LogMessage
  {

    #region Variables

    private readonly string _message;
    private readonly LogLevel _level;
    private readonly Exception _exception;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the <see cref="LogLevel"/> of the current <see cref="LogMessage"/>.
    /// </summary>
    public LogLevel Level
    {
      get { return _level; }
    }

    /// <summary>
    /// Gets the message to log.
    /// </summary>
    public string Message
    {
      get { return _message; }
    }

    /// <summary>
    /// Gets the associated <see cref="Exception"/>, if any.
    /// </summary>
    public Exception Exception
    {
      get { return _exception; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="LogMessage"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
    /// <param name="format">The message to log, as a format.</param>
    /// <param name="args">The arguments to format the <paramref name="format"/> parameter with.</param>
    public LogMessage(LogLevel logLevel, string format, params object[] args)
    {
      _message = string.Format(format, args);
      _level = logLevel;
      _exception = null;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LogMessage"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
    /// <param name="format">The message to log, as a format.</param>
    /// <param name="args">The arguments to format the <paramref name="format"/> parameter with.</param>
    /// <param name="exception">The associated <see cref="Exception"/>.</param>
    public LogMessage(LogLevel logLevel, string format, Exception exception, params object[] args)
    {
      _message = string.Format(format, args);
      _level = logLevel;
      _exception = exception;
    }

    #endregion

  }
}
