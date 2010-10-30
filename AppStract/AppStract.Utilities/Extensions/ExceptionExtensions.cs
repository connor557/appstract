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
using System.Linq;
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
      exceptionFormatter.AppendLine("Exception: " + ex.GetType());
      exceptionFormatter.AppendLine("  Message: " + ex.Message);
      exceptionFormatter.AppendLine("  Source : " + ex.GetTargetSite("AppStract"));
      if (ex.InnerException != null)
        exceptionFormatter.AppendLine("Inner " + ex.InnerException.ToFormattedString(false));
      if (includeStackTrace)
      {
        exceptionFormatter.AppendLine("\r\nStack Trace:");
        exceptionFormatter.AppendLine(ex.StackTrace);
      }
      return exceptionFormatter.ToString();
    }

    private static string GetTargetSite(this Exception ex, string typeRootNamespace)
    {
      if (ex == null)
        throw new NullReferenceException();
      if (string.IsNullOrEmpty(typeRootNamespace))
        throw new ArgumentNullException("typeRootNamespace");
      typeRootNamespace = " " + typeRootNamespace + ".";
      var stack = ex.StackTrace.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
      var targetSite = stack.FirstOrDefault(stackItem => stackItem.Contains(typeRootNamespace));
      if (string.IsNullOrEmpty(targetSite))
        return ex.Source;
      targetSite = targetSite.Substring(targetSite.IndexOf(typeRootNamespace) + 1);
      var closingCharIndex = targetSite.IndexOf(')');
      targetSite = closingCharIndex != -1
                     ? targetSite.Substring(0, closingCharIndex + 1)
                     : targetSite;
      return targetSite;
    }
  }
}
