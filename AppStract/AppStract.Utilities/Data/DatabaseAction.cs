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

namespace AppStract.Utilities.Data
{
  /// <summary>
  /// Represents an action that needs to be performed on a <see cref="Database{T}"/>.
  /// </summary>
  /// <typeparam name="T">The type providing data for the specified action.</typeparam>
  [Serializable]
  public sealed class DatabaseAction<T> : EventArgs
  {

    #region Variables

    private readonly DatabaseActionType _actionType;
    private readonly T _item;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the type of the required action.
    /// </summary>
    public DatabaseActionType ActionType
    {
      get { return _actionType; }
    }

    /// <summary>
    /// Gets the item to perform the required action with.
    /// </summary>
    public T Item
    {
      get { return _item; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="DatabaseAction{T}"/>.
    /// </summary>
    /// <param name="item">The item to perform the required action with.</param>
    /// <param name="actionType">The type of action to perform.</param>
    public DatabaseAction(T item, DatabaseActionType actionType)
    {
      _item = item;
      _actionType = actionType;
    }

    #endregion

  }
}
