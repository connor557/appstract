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
using System.Runtime.Serialization;
using System.Text;

namespace AppStract.Server.Hooking
{
  /// <summary>
  /// Represents errors that occur during actions related to API hooking.
  /// </summary>
  [Serializable]
  public class HookingException : GuestException
  {

    #region Properties

    /// <summary>
    /// Gets or sets the name of the library containing the target function causing the exception.
    /// </summary>
    public string HookedLibraryName
    {
      get; set;
    }

    /// <summary>
    /// Gets or sets the exported symbol name of the target function causing the exception.
    /// </summary>
    public string HookedSymbolName
    {
      get; set;
    }

    #endregion

    #region Constructors

    public HookingException()
    { }

    public HookingException(string message)
      : base(message)
    { }

    public HookingException(string message, string hookedLibrary, string hookedSymbol)
      : base(message)
    {
      HookedLibraryName = hookedLibrary;
      HookedSymbolName = hookedSymbol;
    }

    public HookingException(string message, Exception innerException)
      : base (message, innerException)
    { }

    public HookingException(string message, string hookedLibrary, string hookedSymbol, Exception innerException)
      : base(message, innerException)
    {
      HookedLibraryName = hookedLibrary;
      HookedSymbolName = hookedSymbol;
    }

    protected HookingException(SerializationInfo info, StreamingContext ctxt)
      : base(info, ctxt)
    {
      HookedLibraryName = info.GetString("HookedLibraryName");
      HookedSymbolName = info.GetString("HookedSymbolName");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates and returns a <see cref="string"/> representation of the current <see cref="HookingException"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      // Return base.ToString() with hook target inserted on second line
      var lines = new List<string>(base.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None));
      var extraLine = "Hook target: " + (HookedLibraryName ?? "?") + "." + (HookedSymbolName ?? "?");
      if (lines.Count > 1)
        lines.Insert(1, extraLine);
      else
        lines.Add(extraLine);
      var strBldr = new StringBuilder();
      foreach (var line in lines)
        strBldr.AppendLine(line);
      return strBldr.ToString();
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("HookedLibraryName", HookedLibraryName);
      info.AddValue("HookedSymbolName", HookedSymbolName);
      base.GetObjectData(info, context);
    }

    #endregion

  }
}
