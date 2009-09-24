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
  public class NotifyItemEventRaiser<T> : EventRaiser
  {

    #region Variables

    /// <summary>
    /// The delegate to call.
    /// </summary>
    private readonly NotifyItem<T> _delegate;
    /// <summary>
    /// The item to use as parameter for the <see cref="NotifyItem{TItem}"/> delegate.
    /// </summary>
    private readonly T _itemToNotify;
    /// <summary>
    /// The object to lock when reading the delegate.
    /// </summary>
    private readonly object _syncLock;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="NotifyItemEventRaiser{T}"/>.
    /// </summary>
    /// <param name="dlg">The <see cref="NotifyItem{TItem}"/> delegate to call.</param>
    /// <param name="itemToNotify">The item to use as parameter for the <see cref="NotifyItem{TItem}"/> delegate.</param>
    public NotifyItemEventRaiser(NotifyItem<T> dlg, T itemToNotify)
      : this (dlg, itemToNotify, new object())
    {
      
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NotifyItemEventRaiser{T}"/>.
    /// </summary>
    /// <param name="dlg">The <see cref="NotifyItem{TItem}"/> delegate to call.</param>
    /// <param name="itemToNotify">The item to use as parameter for the <see cref="NotifyItem{TItem}"/> delegate.</param>
    /// <param name="syncLock">The object to lock when reading the delegate.</param>
    public NotifyItemEventRaiser(NotifyItem<T> dlg, T itemToNotify, object syncLock)
    {
      _delegate = dlg;
      _itemToNotify = itemToNotify;
      _syncLock = syncLock;
    }

    #endregion

    #region Public Methods

    public override void Raise()
    {

      lock (_syncLock)
      {
        if (_delegate != null)
          _delegate(_itemToNotify);
      }
    }

    #endregion

  }
}
