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
using System.Text;

namespace AppStract.Utilities.Extensions
{
  public static class ExceptionExtensions
  {

    /// <summary>
    /// Formats the given <see cref="Exception"/> to a string.
    /// </summary>
    /// <param name="ex">The <see cref="Exception"/> to format.</param>
    /// <param name="includeStackTrace">
    /// Specifies if the stack trace has to be formatted into the resulting string.
    /// </param>
    /// <returns></returns>
    public static string ToFormattedString(this Exception ex, bool includeStackTrace)
    {
      var exceptionFormatter = new StringBuilder();
      exceptionFormatter.AppendLine("Exception: " + ex);
      exceptionFormatter.AppendLine("  Message: " + ex.Message);
      exceptionFormatter.AppendLine("  Site   : " + ex.TargetSite);
      exceptionFormatter.AppendLine("  Source : " + ex.Source);
      var inEx = ex.InnerException;
      while (inEx != null)
      {
        exceptionFormatter.AppendLine("Inner Exception:");
        exceptionFormatter.AppendLine("\t" + inEx);
        exceptionFormatter.AppendLine("\t Message: " + inEx.Message);
        inEx = inEx.InnerException;
      }
      if (includeStackTrace)
      {
        exceptionFormatter.AppendLine("Stack Trace:");
        exceptionFormatter.AppendLine(ex.StackTrace);
      }
      return exceptionFormatter.ToString();
    }

  }
}
