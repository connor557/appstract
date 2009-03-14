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

using System.Collections.Generic;
using System.Threading;

namespace AppStract.Utilities.Observables
{
  public class ObservableQueue<TItem>
  {

    #region Variables

    private readonly Queue<TItem> _queue;
    private readonly object _eventEnqueueLock;
    private readonly object _eventDequeueLock;
    private NotifyItem<TItem> _itemEnqueued;
    private NotifyItem<TItem> _itemDequeued;

    #endregion

    #region Events

    /// <summary>
    /// The <see cref="ItemEnqueued"/> event is raised right after an item is added to the queue.
    /// </summary>
    /// <remarks>
    /// This event is called asynchronously, so it's possible that the item is already dequeued when the event is raised.
    /// </remarks>
    public event NotifyItem<TItem> ItemEnqueued
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
    public event NotifyItem<TItem> ItemDequeued
    {
      add { lock (_eventDequeueLock) { _itemDequeued += value; } }
      remove { lock (_eventDequeueLock) { _itemDequeued -= value; } }
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

    public void Enqueue(TItem item)
    {
      _queue.Enqueue(item);
      RaiseEvent(_itemEnqueued, item, _eventEnqueueLock);
    }

    public TItem Dequeue()
    {
      TItem item = _queue.Dequeue();
      RaiseEvent(_itemDequeued, item, _eventDequeueLock);
      return item;
    }

    #endregion

    #region Private Methods

    private static void RaiseEvent(NotifyItem<TItem> dlg, TItem itemToNotify, object syncLock)
    {
      if (dlg == null)
        return;
      ThreadPool.QueueUserWorkItem(RaiseEvent, new EventData<TItem>(dlg, itemToNotify, syncLock));
    }

    public static void RaiseEvent(object eventData)
    {
      EventData<TItem> data = (EventData<TItem>)eventData;
      lock (data.SyncLock)
        data.Delegate(data.ItemToNotify);
    }

    #endregion

    #region Private Structs

    private struct EventData<TEventItem>
    {
      public NotifyItem<TEventItem> Delegate;
      public TEventItem ItemToNotify;
      public object SyncLock;

      public EventData(NotifyItem<TEventItem> dlg, TEventItem itemToNotify, object syncLock)
      {
        Delegate = dlg;
        ItemToNotify = itemToNotify;
        SyncLock = syncLock;
      }
    }

    #endregion

  }
}
