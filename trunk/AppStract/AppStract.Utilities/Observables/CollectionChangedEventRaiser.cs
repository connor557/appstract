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
  internal class CollectionChangedEventRaiser<T> : EventRaiser
  {

    #region Variables

    /// <summary>
    /// The delegate to call.
    /// </summary>
    private readonly CollectionChangedEventHandler<T> _delegate;
    /// <summary>
    /// The collection which owns <see cref="_itemToNotify"/>.
    /// </summary>
    private readonly ICollection<T> _collection;
    /// <summary>
    /// The item to use as parameter for the <see cref="CollectionChangedEventHandler{TItem}"/> delegate.
    /// </summary>
    private readonly T _itemToNotify;
    /// <summary>
    /// The event data.
    /// </summary>
    private readonly EventArgs _args;
    /// <summary>
    /// The object to lock when reading the delegate.
    /// </summary>
    private readonly object _syncLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="CollectionChangedEventRaiser{T}"/>.
    /// </summary>
    /// <param name="dlg">The <see cref="CollectionChangedEventHandler{TItem}"/> delegate to call.</param>
    /// <param name="notifyingCollection">The collection owning the item to notify.</param>
    /// <param name="itemToNotify">The item to use as parameter for the <see cref="CollectionChangedEventHandler{TItem}"/> delegate.</param>
    /// <param name="args">The event data.</param>
    public CollectionChangedEventRaiser(CollectionChangedEventHandler<T> dlg, ICollection<T> notifyingCollection, T itemToNotify, EventArgs args)
      : this (dlg, notifyingCollection, itemToNotify, args, new object())
    {
      
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CollectionChangedEventRaiser{T}"/>.
    /// </summary>
    /// <param name="dlg">The <see cref="CollectionChangedEventHandler{TItem}"/> delegate to call.</param>
    /// <param name="notifyingCollection">The collection owning the item to notify.</param>
    /// <param name="itemToNotify">The item to use as parameter for the <see cref="CollectionChangedEventHandler{TItem}"/> delegate.</param>
    /// <param name="args">The event data.</param>
    /// <param name="syncLock">The object to lock when reading the delegate.</param>
    public CollectionChangedEventRaiser(CollectionChangedEventHandler<T> dlg, ICollection<T> notifyingCollection, T itemToNotify, EventArgs args, object syncLock)
    {
      _delegate = dlg;
      _collection = notifyingCollection;
      _itemToNotify = itemToNotify;
      _args = args;
      _syncLock = syncLock;
    }

    #endregion

    #region Public Methods

    public override void Raise()
    {
      lock (_syncLock)
      {
        if (_delegate != null)
          _delegate(_collection, _itemToNotify, _args);
      }
    }

    #endregion

  }
}
