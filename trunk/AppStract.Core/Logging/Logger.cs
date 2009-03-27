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
using System.Text;
using System.Threading;

namespace AppStract.Core.Logging
{
  public abstract class Logger
  {

    #region Variables

    protected object _syncRoot;
    protected TextWriter _writer;
    protected LogLevel _level;

    #endregion

    #region Properties

    public LogLevel LogLevel
    {
      get { return _level; }
      set { _level = value; }
    }

    #endregion

    #region Constructors

    protected Logger() { }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="logLevel"></param>
    /// <param name="writer"></param>
    protected Logger(LogLevel logLevel, TextWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException("writer");
      _level = logLevel;
      _writer = writer;
      _syncRoot = new object();
    }

    #endregion

    #region Public Methods

    public virtual void Warning(string format, params object[] args)
    {
      if (LogLevel.Warning < _level)
        Write(FormatLogMessage(LogLevel.Warning, string.Format(format, args)));
    }

    public virtual void Warning(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Warning < _level)
        Write(FormatLogMessage(LogLevel.Warning, string.Format(format, args), exception));
    }

    public virtual void Message(string format, params object[] args)
    {
      if (LogLevel.Information < _level)
        Write(FormatLogMessage(LogLevel.Information, string.Format(format, args)));
    }

    public virtual void Message(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Information < _level)
        Write(FormatLogMessage(LogLevel.Information, string.Format(format, args), exception));
    }

    public virtual void Error(string format, params object[] args)
    {
      if (LogLevel.Error < _level)
        Write(FormatLogMessage(LogLevel.Error, string.Format(format, args)));
    }

    public virtual void Error(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Error < _level)
        Write(FormatLogMessage(LogLevel.Error, string.Format(format, args), exception));
    }

    public virtual void Critical(string format, params object[] args)
    {
      if (LogLevel.Critical < _level)
        Write(FormatLogMessage(LogLevel.Critical, string.Format(format, args)));
    }

    public virtual void Critical(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Critical < _level)
        Write(FormatLogMessage(LogLevel.Critical, string.Format(format, args), exception));
    }

    public virtual void Debug(string format, params object[] args)
    {
      if (LogLevel.Debug < _level)
        Write(FormatLogMessage(LogLevel.Debug, string.Format(format, args)));
    }

    public virtual void Debug(string format, Exception exception, params object[] args)
    {
      if (LogLevel.Debug < _level)
        Write(FormatLogMessage(LogLevel.Debug, string.Format(format, args), exception));
    }

    #endregion

    #region Protected Methods

    protected virtual string FormatLogMessage(LogLevel logLevel, string message)
    {
      return string.Format("{0} [{1}] [{2}] {3}",
        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
        logLevel,
        Thread.CurrentThread.Name,
        message);
    }

    protected virtual string FormatLogMessage(LogLevel logLevel, string message, Exception exception)
    {
      string formattedLogMessage = FormatLogMessage(logLevel, message);
      string formattedException = FormatException(exception);
      return formattedLogMessage + "\r\n" + formattedException;
    }

    protected virtual void Write(string message)
    {
      Monitor.Enter(_syncRoot);
      try
      {
        _writer.WriteLine(message);
        _writer.Flush();
      }
      finally
      {
        Monitor.Exit(_syncRoot);
      }
    }

    protected static string FormatException(Exception ex)
    {
      StringBuilder exceptionFormatter = new StringBuilder();
      exceptionFormatter.AppendLine("Exception: " + ex);
      exceptionFormatter.AppendLine("  Message: " + ex.Message);
      exceptionFormatter.AppendLine("  Site   : " + ex.TargetSite);
      exceptionFormatter.AppendLine("  Source : " + ex.Source);
      Exception innerException = ex.InnerException;
      while (innerException != null)
      {
        exceptionFormatter.AppendLine("Inner Exception:");
        exceptionFormatter.AppendLine("\t" + innerException);
        exceptionFormatter.AppendLine("\t Message: " + innerException.Message);
        innerException = innerException.InnerException;
      }
      exceptionFormatter.AppendLine("Stack Trace:");
      exceptionFormatter.AppendLine(ex.StackTrace);
      return exceptionFormatter.ToString();
    }

    #endregion

  }
}
