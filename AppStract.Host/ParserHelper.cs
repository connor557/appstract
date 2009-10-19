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

namespace AppStract.Host
{
  public static class ParserHelper
  {

    public static bool TryParseLogLevel(object logLevel, out LogLevel rLogLevel)
    {
      rLogLevel = LogLevel.None;
      var logLevelType = typeof(LogLevel);
      if (!Enum.IsDefined(logLevelType, logLevel))
        return false;
      rLogLevel = (LogLevel)Enum.Parse(logLevelType, logLevel.ToString());
      return true;
    }

    public static bool TryParseLogType(object logType, out LogType rLogType)
    {
      rLogType = LogType.Null;
      var logTypeType = typeof(LogType);
      if (!Enum.IsDefined(logTypeType, logType))
        return false;
      rLogType = (LogType)Enum.Parse(logTypeType, logType.ToString());
      return true;
    }

  }
}
