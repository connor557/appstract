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

namespace AppStract.Utilities.Observables
{
  /// <summary>
  /// Interfaces a dynamic data collection that provides notifications when items get added, removed, or changed.
  /// </summary>
  /// <typeparam name="T">The type of elements in the collection.</typeparam>
  public interface IObservableCollection<T>
  {
    
    /// <summary>
    /// Occurs when an item is added.
    /// </summary>
    event CollectionChangedEventHandler<T> ItemAdded;

    /// <summary>
    /// Occurs when an item is changed.
    /// </summary>
    event CollectionChangedEventHandler<T> ItemChanged;

    /// <summary>
    /// Occurs when an item is removed.
    /// </summary>
    event CollectionChangedEventHandler<T> ItemRemoved;

  }
}
