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

namespace AppStract.Core.Data.Databases
{
  /// <summary>
  /// Defines a range of indices.
  /// </summary>
  public class IndexRange
  {

    #region Variables

    private uint _start;
    private uint _end;

    #endregion

    #region Properties

    /// <summary>
    /// The start of the range.
    /// </summary>
    public uint Start
    {
      get { return _start; }
      set { _start = value; }
    }

    /// <summary>
    /// The end of the range.
    /// </summary>
    public uint End
    {
      get { return _end; }
      set { _end = value; }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Instantiates a new <see cref="IndexRange"/> based on the values specified for
    /// <paramref name="start"/> and <paramref name="end"/>.
    /// </summary>
    /// <param name="start">The start of the range.</param>
    /// <param name="end">The end of the range.</param>
    public IndexRange(uint start, uint end)
    {
      if (end < start)
        throw new ArgumentException("Parameter \"end\" must be greater then \"start\"", "end");
      _start = start;
      _end = end;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns whether the <paramref name="value"/> specified falls within range of the current <see cref="IndexRange"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool IsInRange(uint value)
    {
      return value >= _start
             && value <= _end;
    }

    #endregion

  }
}
