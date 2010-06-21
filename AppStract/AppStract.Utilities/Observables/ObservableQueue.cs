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
using System.Collections.Generic;

namespace AppStract.Utilities.Observables
{
  /// <summary>
  /// A queue that provides notifications when items get added or removed.
  /// </summary>
  /// <typeparam name="TItem"></typeparam>
  public class ObservableQueue<TItem> : IObservableItem<QueueChangedEventArgs<TItem>>
  {

    #region Variables

    private readonly Queue<TItem> _queue;
    private readonly object _eventEnqueueLock;
    private readonly object _eventDequeueLock;
    private EventHandler<QueueChangedEventArgs<TItem>> _itemEnqueued;
    private EventHandler<QueueChangedEventArgs<TItem>> _itemDequeued;

    #endregion

    #region Events

    /// <summary>
    /// The <see cref="ItemEnqueued"/> event is raised right after an item is added to the queue.
    /// </summary>
    /// <remarks>
    /// This event is called asynchronously, so it's possible that the item is already dequeued when the event is raised.
    /// </remarks>
    public event EventHandler<QueueChangedEventArgs<TItem>> ItemEnqueued
    {
      add { lock (_eventEnqueueLock) { _itemEnqueued += value; } }
      remove { lock (_eventEnqueueLock) { _itemEnqueued -= value; } }
    }

    /// <summary>
    /// The <see cref="ItemDequeued"/> event is raised when an item is removed from the queue.
    /// </summary>
    /// <remarks>
    /// This event is called asynchronously, so it's possible that the item is still in the queue when the event is raised.
    /// </remarks>
    public event EventHandler<QueueChangedEventArgs<TItem>> ItemDequeued
    {
      add { lock (_eventDequeueLock) { _itemDequeued += value; } }
      remove { lock (_eventDequeueLock) { _itemDequeued -= value; } }
    }

    #endregion

    #region Properties

    public int Count
    {
      get { return _queue.Count; }
    }

    #endregion

    #region Constructors

    public ObservableQueue()
    {
      _queue = new Queue<TItem>();
      _eventEnqueueLock = new object();
      _eventDequeueLock = new object();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds an object to the <see cref="ObservableQueue{TItem}"/>.
    /// </summary>
    /// <remarks>The value can be null for reference types.</remarks>
    /// <param name="item">The object to add to the <see cref="ObservableQueue{TItem}"/>.</param>
    public void Enqueue(TItem item)
    {
      _queue.Enqueue(item);
      RaiseEvent(_itemEnqueued, item, _eventEnqueueLock);
    }

    /// <summary>
    /// Adds a serie of objects to the <see cref="ObservableQueue{TItem}"/>.
    /// An event is raisen for each item.
    /// </summary>
    /// <param name="items">The objects to add to the <see cref="ObservableQueue{TItem}"/>.</param>
    /// <param name="immediateRaise">
    /// True if the <see cref="ItemEnqueued"/> must be raisen per enqueued item.
    /// False if all events must be raisen AFTER all items are enqueued.
    /// </param>
    public void Enqueue(IEnumerable<TItem> items, bool immediateRaise)
    {
      foreach (var item in items)
      {
        _queue.Enqueue(item);
        if (immediateRaise)
          RaiseEvent(_itemEnqueued, item, _eventEnqueueLock);
      }
      if (!immediateRaise)
        foreach (var item in items)
          RaiseEvent(_itemEnqueued, item, _eventEnqueueLock);
    }

    /// <summary>
    /// Removes and returns the object at the beginning of the <see cref="ObservableQueue{TItem}"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">The <see cref="ObservableQueue{TItem}"/> is empty.</exception>
    /// <returns>The object that is removed from the beginning of the <see cref="ObservableQueue{TItem}"/>.</returns>
    public TItem Dequeue()
    {
      var item = _queue.Dequeue();
      RaiseEvent(_itemDequeued, item, _eventDequeueLock);
      return item;
    }

    #endregion

    #region Private Methods

    private void RaiseEvent(EventHandler<QueueChangedEventArgs<TItem>> dlg, TItem itemToNotify, object syncLock)
    {
      if (dlg != null)
        new ItemChangedEventRaiser<QueueChangedEventArgs<TItem>>(dlg, this, new QueueChangedEventArgs<TItem>(itemToNotify), syncLock).RaiseAsync();
    }

    #endregion

    #region IObservableItem<QueueChangedEventArgs> Members

    public event EventHandler<QueueChangedEventArgs<TItem>> Changed;

    #endregion

  }
}
