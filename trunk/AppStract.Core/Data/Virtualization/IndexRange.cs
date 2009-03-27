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

namespace AppStract.Core.Data.Virtualization
{
  public class IndexRange
  {

    #region Variables

    private uint _start;
    private uint _end;

    #endregion

    #region Properties

    public uint Start
    {
      get { return _start; }
      set { _start = value; }
    }

    public uint End
    {
      get { return _end; }
      set { _end = value; }
    }

    #endregion

    #region Constructor

    public IndexRange(uint start, uint end)
    {
      if (end < start)
        throw new ArgumentException("Parameter \"end\" must be greater then \"start\"", "end");
      _start = start;
      _end = end;
    }

    #endregion

    #region Public Methods

    public bool IsInRange(uint value)
    {
      return value >= _start
             && value <= _end;
    }

    #endregion

  }
}
