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
using System.IO;
using System.Threading;
using AppStract.Utilities.Extensions;

namespace AppStract.Utilities.Logging
{
  /// <summary>
  /// Provides the common implementation of a log service.
  /// This class is abstract.
  /// </summary>
  public abstract class Logger
  {

    #region Variables

    protected object _syncRoot;
    protected TextWriter _writer;
    protected LogLevel _level;
    protected LogType _logType;

    #endregion

    #region Properties

    public LogLevel LogLevel
    {
      get { return _level; }
      set { _level = value; }
    }

    public LogType Type
    {
      get { return _logType; }
    }

    #endregion

    #region Constructors

    protected Logger() { }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="logType"></param>
    /// <param name="logLevel"></param>
    /// <param name="writer"></param>
    protected Logger(LogType logType, LogLevel logLevel, TextWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      _level = logLevel;
      _logType = logType;
      _writer = writer;
      _syncRoot = new object();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Verifies if the specified <paramref name="logMessage"/> should be logged,
    /// and if so, writes the message to the log.
    /// </summary>
    /// <remarks>
    /// <paramref name="logMessage"/> is written to the log by calling <see cref="Write(LogMessage)"/>.
    /// </remarks>
    /// <param name="logMessage"></param>
    public virtual void Log(LogMessage logMessage)
    {
      if (logMessage.Level <= _level)
        Write(logMessage);
    }

    public void Message(string format, params object[] args)
    {
      Log(new LogMessage(LogLevel.Information, format, args));
    }

    public void Message(string format, Exception exception, params object[] args)
    {
      Log(new LogMessage(LogLevel.Information, format, exception, args));
    }

    public void Warning(string format, params object[] args)
    {
      Log(new LogMessage(LogLevel.Warning, format, args));
    }

    public void Warning(string format, Exception exception, params object[] args)
    {
      Log(new LogMessage(LogLevel.Warning, format, exception, args));
    }

    public void Error(string format, params object[] args)
    {
      Log(new LogMessage(LogLevel.Error, format, args));
    }

    public void Error(string format, Exception exception, params object[] args)
    {
      Log(new LogMessage(LogLevel.Error, format, exception, args));
    }

    public void Critical(string format, params object[] args)
    {
      Log(new LogMessage(LogLevel.Critical, format, args));
    }

    public void Critical(string format, Exception exception, params object[] args)
    {
      Log(new LogMessage(LogLevel.Critical, format, exception, args));
    }

    public void Debug(string format, params object[] args)
    {
      Log(new LogMessage(LogLevel.Debug, format, args));
    }

    public void Debug(string format, Exception exception, params object[] args)
    {
      Log(new LogMessage(LogLevel.Debug, format, exception, args));
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Formats the specified <see cref="LogMessage"/> to a <see cref="string"/> using <see cref="FormatLogMessage"/>.
    /// The message is then passed to <see cref="Write(string)"/> using a thread from <see cref="ThreadPool"/>.
    /// </summary>
    /// <param name="message"></param>
    protected virtual void Write(LogMessage message)
    {
      // Note: When guest sends a batch of messages, this method consumes all ThreadPool threads
      ThreadPool.QueueUserWorkItem(CallWriteForString, FormatLogMessage(message));
    }

    /// <summary>
    /// Writes the given <paramref name="message"/> to <see cref="_writer"/>.
    /// </summary>
    /// <param name="message"></param>
    protected virtual void Write(string message)
    {
      lock (_syncRoot)
      {
        _writer.WriteLine(message);
        _writer.Flush();
      }
    }

    /// <summary>
    /// Formats the given <see cref="LogMessage"/> to a string.
    /// </summary>
    /// <param name="message">The <see cref="LogMessage"/> to format to a <see cref="string"/>.</param>
    /// <returns></returns>
    protected virtual string FormatLogMessage(LogMessage message)
    {
      var formattedMessage
        = string.Format("{0} [{1}]{2} [{3}] {4}",
                        message.DateTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                        message.Level,
                        message.Prefix != null ? " [" + message.Prefix + "]" : "",
                        message.SendingThread,
                        message.Message)
          + (message.Exception != null
               ? "\r\n" + message.Exception.ToFormattedString(MustIncludeStackTrace(message.Level))
               : "");
      return formattedMessage;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Can be used as a <see cref="WaitCallback"/>.
    /// Calls <see cref="Write(string)"/>, with <paramref name="message"/>.ToString() as parameter.
    /// </summary>
    /// <param name="message"></param>
    private void CallWriteForString(object message)
    {
      Write(message.ToString());
    }

    /// <summary>
    /// Returns whether or not the stacktrace should be included when formatting
    /// an <see cref="Exception"/> for the given <see cref="LogLevel"/>.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns></returns>
    private static bool MustIncludeStackTrace(LogLevel logLevel)
    {
      return logLevel == LogLevel.Debug;
    }

    #endregion

  }
}
