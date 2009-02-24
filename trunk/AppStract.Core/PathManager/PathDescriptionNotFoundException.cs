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


namespace AppStract.Core.Paths
{

  /// <summary>
  /// This class represents an Exception caused by a request for an unknown path.
  /// </summary>
  public class PathDescriptionNotFoundException : Exception
  {

    #region Variables

    /// <summary>
    /// The path description causing the exception.
    /// </summary>
    protected string _pathDescription;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the PathDescription causing the exception.
    /// </summary>
    public string PathDescription
    {
      get { return _pathDescription; }
    }

    #endregion

    #region Constructors

    public PathDescriptionNotFoundException(string pathDescription)
    {
      _pathDescription = pathDescription;
    }

    public PathDescriptionNotFoundException(string pathDescription, string message)
      : base(message)
    {
      _pathDescription = pathDescription;
    }

    public PathDescriptionNotFoundException(string pathDescription, string message, Exception innerException)
      : base(message, innerException)
    {
      _pathDescription = pathDescription;
    }

    public PathDescriptionNotFoundException(string pathDescription, string format, params object[] args)
      : base(String.Format(format, args))
    {
      _pathDescription = pathDescription;
    }

    public PathDescriptionNotFoundException(string pathDescription, string format, Exception innerException, params object[] args)
      : base(String.Format(format, args), innerException)
    {
      _pathDescription = pathDescription;
    }

    #endregion

  }

}
