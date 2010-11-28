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
using System.Threading;
using AppStract.Utilities.Logging;
using AppStract.Utilities.Observables;

namespace AppStract.Utilities.Data
{
  /// <summary>
  /// Base class for database interfaces using SQLite.
  /// </summary>
  /// <typeparam name="T">The type of objects stored and accessed by the current <see cref="Database{T}"/>.</typeparam>
  public abstract class Database<T> : IDisposable
  {

    #region Private Types

    private class QueueEmptier<TItem> :IEnumerator<TItem>
    {
      #region Variables

      private readonly ObservableQueue<TItem> _queue;
      private readonly IList<TItem> _dequeuedItems;

      #endregion

      #region Constructors

      public QueueEmptier(ObservableQueue<TItem> queue)
      {
        _queue = queue;
        _dequeuedItems = new List<TItem>();
      }

      #endregion

      #region Public Methods

      public IEnumerable<TItem> GetDequeuedItems()
      {
        return _dequeuedItems;
      }

      #endregion

      #region IEnumerator<T> Members

      public TItem Current { get; private set; }

      #endregion

      #region IDisposable Members

      public void Dispose()
      {
        
      }

      #endregion

      #region IEnumerator Members

      object System.Collections.IEnumerator.Current
      {
        get { return Current; }
      }

      public bool MoveNext()
      {
        if (_queue.Count == 0)
          return false;
        Current = _queue.Dequeue();
        _dequeuedItems.Add(Current);
        return true;
      }

      public void Reset()
      {
        throw new NotSupportedException();
      }

      #endregion
    }

    #endregion

    #region Variables

    /// <summary>
    /// Queue for the <see cref="DatabaseAction{T}"/>s waiting to be commited.
    /// </summary>
    private readonly ObservableQueue<DatabaseAction<T>> _actionQueue;
    /// <summary>
    /// Object to acquire a write-lock on when flushing <see cref="_actionQueue"/>.
    /// </summary>
    private readonly ReaderWriterLockSlim _flushLock;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a new item is added to the queue.
    /// </summary>
    /// <remarks>
    /// This event is synchronously called and is not thread safe.
    /// Inheritors should never unsubscribe, in order to avoid exceptions while raising the event.
    /// Inheritors should also process the event async if processing takes a long time.
    /// If an inheritor exposes this event, the inheritor should implement it's own functionality to make it thread safe.
    /// </remarks>
    protected event EventHandler<DatabaseAction<T>> ItemEnqueued;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the log service.
    /// </summary>
    protected Logger Log
    {
      get; private set;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="Database{T}"/>.
    /// </summary>
    protected Database()
    {
      _flushLock = new ReaderWriterLockSlim();
      _actionQueue = new ObservableQueue<DatabaseAction<T>>();
      _actionQueue.ItemEnqueued += OnActionEnqueued;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the database.
    /// Must be called before being able to use any other functionality.
    /// </summary>
    /// <exception cref="DatabaseException">
    /// A <see cref="DatabaseException"/> is thrown if the connectionstring is invalid.
    /// -0R-
    /// A <see cref="DatabaseException"/> is thrown if initialization failed.
    /// </exception>
    public void Initialize()
    {
      Log = GetLogger() ?? new NullLogger();
      DoInitialize();
    }

    /// <summary>
    /// Reads the complete database to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<T> ReadAll();

    /// <summary>
    /// Enqueues an action to be committed to the database.
    /// </summary>
    /// <param name="databaseAction"></param>
    public void EnqueueAction(DatabaseAction<T> databaseAction)
    {
      // Don't acquire a write lock, this class can handle the enqueueing of new items while flushing.
      // A lock is only necessary when dequeueing items.
      _actionQueue.Enqueue(databaseAction);
    }

    /// <summary>
    /// Enqueues multiple actions to be committed to the database.
    /// </summary>
    /// <param name="databaseActions"></param>
    public void EnqueueAction(IEnumerable<DatabaseAction<T>> databaseActions)
    {
      // Don't acquire a write lock, this class can handle the enqueueing of new items while flushing.
      // A lock is only necessary when dequeueing items.
      _actionQueue.Enqueue(databaseActions, false);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Returns the <see cref="Logger"/> to write any log messages to.
    /// </summary>
    /// <returns></returns>
    protected abstract Logger GetLogger();

    /// <summary>
    /// Initializes the database.
    /// </summary>
    protected abstract void DoInitialize();

    /// <summary>
    /// Returns whether the specified <paramref name="item"/> exists in the database.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract bool ItemExists(T item);

    /// <summary>
    /// Writes the given items to the database.
    /// </summary>
    /// <remarks>
    /// When implementing this method, bear in mind that every call <see cref="IEnumerator{T}.MoveNext"/>
    /// dequeues an item and is therefor irreversible.
    /// </remarks>
    /// <exception cref="DatabaseException">
    /// A <see cref="DatabaseException"/> is thrown if anything goes wrong during the writing operation.
    /// </exception>
    /// <param name="items"></param>
    protected abstract void Write(IEnumerator<DatabaseAction<T>> items);

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles newly enqueued <see cref="DatabaseAction{T}"/>s.
    /// </summary>
    private void OnActionEnqueued(object sender, QueueChangedEventArgs<DatabaseAction<T>> e)
    {
      if (ItemEnqueued != null)
        ItemEnqueued(this, e.Data);
      try
      {
        Flush(false);
      }
      catch (DatabaseException ex)
      {
        Log.Error("[Database] Failed to flush", ex);
      }
    }

    /// <summary>
    /// Flushes all queued <see cref="DatabaseAction{TItem}"/>s to the physical database.
    /// It's not guaranteed that it's the current thread that will perform the flushing.
    /// </summary>
    /// <exception cref="DatabaseException">
    /// An <see cref="DatabaseException"/> is thrown if flushing failes.
    /// </exception>
    /// <exception cref="TimeoutException">
    /// A <see cref="TimeoutException"/> is thrown if <paramref name="awaitRunningSequence"/> is set to true
    /// and if the current flush sequence can't start within 2000ms.
    /// </exception>
    /// <param name="awaitRunningSequence">
    /// Specifies, in case a flush sequence is already running, whether or not this method needs to wait before returning until the other sequence ended.
    /// If waiting takes more then 2000ms, a <see cref="TimeoutException"/> is thrown.
    /// </param>
    private void Flush(bool awaitRunningSequence)
    {
      try
      {
        if (!_flushLock.TryEnterWriteLock(2))
        {
          // Already flushing, wait for flushing to end before return statement?
          if (awaitRunningSequence)
          {
            if (!_flushLock.TryEnterWriteLock(2000))
              throw new TimeoutException("Failed to wait for database-flushing to end.");
            _flushLock.ExitWriteLock();
          }
          return;
        }
        var items = new QueueEmptier<DatabaseAction<T>>(_actionQueue);
        try
        {
          Write(items);
        }
        catch
        {
          _actionQueue.Enqueue(items.GetDequeuedItems(), false);
          throw;
        }
      }
      finally
      {
        if (_flushLock.IsWriteLockHeld)
          _flushLock.ExitWriteLock();
      }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      try
      {
        Flush(true);
      }
      catch (Exception e)
      {
        Log.Error("Failed to flush to database during Dispose", e);
      }
    }

    #endregion

  }
}
