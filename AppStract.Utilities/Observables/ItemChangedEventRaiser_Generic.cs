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

namespace AppStract.Utilities.Observables
{
  /// <summary>
  /// Raises an <see cref="EventHandler"/> synchronously or asynchronously.
  /// </summary>
  internal class ItemChangedEventRaiser<TEventArgs> : EventRaiser where TEventArgs : EventArgs
  {

    #region Variables

    /// <summary>
    /// The delegate to call.
    /// </summary>
    private readonly EventHandler<TEventArgs> _delegate;
    /// <summary>
    /// The item to use as sender parameter for the <see cref="EventHandler"/> delegate.
    /// </summary>
    private readonly object _itemToNotify;
    /// <summary>
    /// The event data.
    /// </summary>
    private readonly TEventArgs _args;
    /// <summary>
    /// The object to lock when reading the delegate.
    /// </summary>
    private readonly object _syncLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ItemChangedEventRaiser"/>.
    /// </summary>
    /// <param name="dlg">The <see cref="EventHandler"/> delegate to call.</param>
    /// <param name="itemToNotify">The item to use as sender parameter for the <see cref="EventHandler"/> delegate.</param>
    /// <param name="args">The event data.</param>
    public ItemChangedEventRaiser(EventHandler<TEventArgs> dlg, object itemToNotify, TEventArgs args)
      : this(dlg, itemToNotify, args, new object())
    {

    }

    /// <summary>
    /// Initializes a new instance of <see cref="ItemChangedEventRaiser"/>.
    /// </summary>
    /// <param name="dlg">The <see cref="EventHandler"/> delegate to call.</param>
    /// <param name="itemToNotify">The item to use as parameter for the <see cref="EventHandler"/> delegate.</param>
    /// <param name="args">The event data.</param>
    /// <param name="syncLock">The object to lock when reading the delegate.</param>
    public ItemChangedEventRaiser(EventHandler<TEventArgs> dlg, object itemToNotify, TEventArgs args, object syncLock)
    {
      _delegate = dlg;
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
          _delegate(_itemToNotify, _args);
      }
    }

    #endregion

  }
}
