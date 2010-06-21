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
using System.Runtime.Serialization;
using AppStract.Core;

namespace AppStract.Server
{
  /// <summary>
  /// Represents errors that occur during code execution in the guest's core.
  /// </summary>
  [Serializable]
  public class GuestException : CoreException
  {

    #region Constructors

    public GuestException()
    { }

    public GuestException(string message)
      : base(message)
    { }

    public GuestException(string message, Exception innerException)
      : base (message, innerException)
    { }

    protected GuestException(SerializationInfo info, StreamingContext ctxt)
      : base(info, ctxt) { }

    #endregion

  }
}
