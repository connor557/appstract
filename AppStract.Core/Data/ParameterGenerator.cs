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

namespace AppStract.Core.Data
{
  public class ParameterGenerator
  {

    #region Variables

    private uint _index;
    private readonly object _indexLock;

    #endregion

    #region Constructors

    public ParameterGenerator()
    {
      _indexLock = new object();
      _index = 0;
    }

    #endregion

    #region Public Methods

    public string Next()
    {
      lock (_indexLock)
        _index++;
      return "*param" + _index;
    }

    #endregion

  }
}
