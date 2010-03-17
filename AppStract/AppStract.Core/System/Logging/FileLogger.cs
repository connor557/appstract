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

using System.IO;

namespace AppStract.Core.System.Logging
{
  /// <summary>
  /// A <see cref="Logger"/> which writes all received messages to a file.
  /// </summary>
  public class FileLogger : Logger
  {

    #region Constructors

    private FileLogger(LogLevel logLevel, TextWriter textWriter)
      : base (LogType.File, logLevel, textWriter)
    { }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Creates a LogService which logs all message fitting the specified LogLevel
    /// to the specified log file.
    /// </summary>
    /// <param name="logLevel">Minimum level of log-entries to log.</param>
    /// <param name="logLocation">Path to the file to write log-entries to.</param>
    /// <returns></returns>
    public static FileLogger CreateLogService(LogLevel logLevel, string logLocation)
    {
      string directory = Path.GetDirectoryName(logLocation);
      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);
      if (File.Exists(logLocation))
        File.Delete(logLocation);
      return new FileLogger(logLevel, new StreamWriter(logLocation));
    }

    #endregion

  }
}
