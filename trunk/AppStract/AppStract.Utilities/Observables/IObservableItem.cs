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

namespace AppStract.Utilities.Observables
{
  /// <summary>
  /// Interfaces a class that's observable for changes.
  /// </summary>
  public interface IObservableItem
  {

    /// <summary>
    /// Occurs when the state of the item is changed.
    /// </summary>
    event EventHandler Changed;

  }

  /// <summary>
  /// Interfaces a class that's observable for changes, and reports those changed with event data of type <see cref="EventArgsType"/>.
  /// </summary>
  /// <typeparam name="EventArgsType">The type of class wrapping the event data.</typeparam>
  public interface IObservableItem<EventArgsType> where EventArgsType : EventArgs
  {

    /// <summary>
    /// Occurs when the state of the item is changed.
    /// </summary>
    event EventHandler<EventArgsType> Changed;

  }

}
