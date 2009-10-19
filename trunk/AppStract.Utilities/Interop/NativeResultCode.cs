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

namespace Microsoft.Win32.Interop
{
  /// <summary>
  /// Static error codes resulting from native API calls.
  /// </summary>
  public enum NativeResultCode
  {
    /// <summary>
    /// The operation completed successfully.
    /// </summary>
    Succes = 0,
    /// <summary>
    /// The handle is invalid.
    /// </summary>
    InvalidHandle = 1,
    /// <summary>
    /// Access is denied.
    /// </summary>
    AccessDenied = 2,
    /// <summary>
    /// The system can not find the specified resource.
    /// </summary>
    NotFound = 3,
    /// <summary>
    /// The system can not find the specified file.
    /// </summary>
    FileNotFound = 4
  }
}
