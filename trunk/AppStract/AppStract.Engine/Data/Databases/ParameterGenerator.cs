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

namespace AppStract.Engine.Data.Databases
{
  /// <summary>
  /// Generates unique names for parameters.
  /// This class is intended to be used when building large queries
  /// by using different methods and/or threads.
  /// </summary>
  public sealed class ParameterGenerator
  {

    #region Variables

    /// <summary>
    /// The last assigned index.
    /// </summary>
    private uint _index;
    /// <summary>
    /// The object to lock when changing <see cref="_index"/>.
    /// </summary>
    private readonly object _indexLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ParameterGenerator"/>.
    /// </summary>
    public ParameterGenerator()
    {
      _indexLock = new object();
      _index = 0;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the next unique name for a parameter.
    /// </summary>
    /// <returns></returns>
    public string Next()
    {
      lock (_indexLock)
        return "@param" + ++_index;
    }

    #endregion

  }
}
