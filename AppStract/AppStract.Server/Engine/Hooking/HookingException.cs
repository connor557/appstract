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

namespace AppStract.Server.Engine.Hooking
{
  /// <summary>
  /// Represents errors that occur during actions related to API hooking.
  /// </summary>
  [Serializable]
  public class HookingException : EngineException
  {

    #region Variables

    private HookData _apiHook;
    private string _apiHookDelegateName;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the data describing target function causing the exception.
    /// </summary>
    public HookData ApiHook
    {
      get { return _apiHook; }
      set
      {
        _apiHook = value;
        _apiHookDelegateName = _apiHook.Handler.Method.DeclaringType + " -> " + _apiHook.Handler.Method;
      }
    }

    #endregion

    #region Constructors

    public HookingException()
    { }

    public HookingException(string message)
      : base(message)
    { }

    public HookingException(string message, HookData hookData)
      : base(message)
    {
      ApiHook = hookData;
    }

    public HookingException(string message, Exception innerException)
      : base (message, innerException)
    { }

    public HookingException(string message, HookData hookData, Exception innerException)
      : base(message, innerException)
    {
      ApiHook = hookData;
    }

    protected HookingException(SerializationInfo info, StreamingContext ctxt)
      : base(info, ctxt)
    {
      try
      {
        _apiHook = new HookData(
          info.GetString("HookDescription"),
          info.GetString("HookTargetLibrary"),
          info.GetString("HookTargetSymbol"),
          null, null);
        _apiHookDelegateName = info.GetString("HookDelegate");
      }
      catch (SerializationException)
      {
      }
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
      var extraLine = "Hook target: " + _apiHook + "\r\nHook delegate: " + _apiHookDelegateName;
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
      info.AddValue("HookDescription", _apiHook.Description);
      info.AddValue("HookTargetLibrary", _apiHook.TargetLibrary);
      info.AddValue("HookTargetSymbol", _apiHook.TargetSymbol);
      info.AddValue("HookDelegate", _apiHookDelegateName);
      base.GetObjectData(info, context);
    }

    #endregion

  }
}
