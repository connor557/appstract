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
using System.Collections.Generic;

namespace AppStract.Utilities.Observables
{

  /// <summary>
  /// Represents the method that will handle a change in a collection.
  /// </summary>
  /// <typeparam name="T">The type of items held in the collection.</typeparam>
  /// <param name="sender">The collection reporting the change.</param>
  /// <param name="item">The item causing the change in the collection.</param>
  /// <param name="args">The event data related to the change.</param>
  public delegate void CollectionChangedEventHandler<T>(ICollection<T> sender, T item, EventArgs args);

}
