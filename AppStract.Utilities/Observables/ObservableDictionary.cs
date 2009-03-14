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
  /// <summary>
  /// Observable wrapper class for <see cref="Dictionary{TKey,TValue}"/>.
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
  {

    #region Variables

    private readonly IDictionary<TKey, TValue> _dictionary;
    private NotifyCollectionItem<KeyValuePair<TKey, TValue>> _added;
    private NotifyCollectionItem<KeyValuePair<TKey, TValue>> _changed;
    private NotifyCollectionItem<KeyValuePair<TKey, TValue>> _removed;
    private ReaderWriterLockSlim _addedSync;
    private ReaderWriterLockSlim _changedSync;
    private ReaderWriterLockSlim _removedSync;

    #endregion

    #region Events

    public event NotifyCollectionItem<KeyValuePair<TKey, TValue>> ItemAdded
    {
      add
      {
        _addedSync.EnterWriteLock();
        try
        {
          _added += value;
        }
        finally
        {
          _addedSync.ExitWriteLock();
        }
      }
      remove
      {
        _addedSync.EnterWriteLock();
        try
        {
          _added -= value;
        }
        finally
        {
          _addedSync.ExitWriteLock();
        }
      }
    }

    public event NotifyCollectionItem<KeyValuePair<TKey, TValue>> ItemChanged
    {
      add
      {
        _changedSync.EnterWriteLock();
        try
        {
          _changed += value;
        }
        finally
        {
          _changedSync.ExitWriteLock();
        }
      }
      remove
      {
        _changedSync.EnterWriteLock();
        try
        {
          _changed -= value;
        }
        finally
        {
          _changedSync.ExitWriteLock();
        }
      }
    }

    public event NotifyCollectionItem<KeyValuePair<TKey, TValue>> ItemRemoved
    {
      add
      {
        _removedSync.EnterWriteLock();
        try
        {
          _removed += value;
        }
        finally
        {
          _removedSync.ExitWriteLock();
        }
      }
      remove
      {
        _removedSync.EnterWriteLock();
        try
        {
          _removed -= value;
        }
        finally
        {
          _removedSync.ExitWriteLock();
        }
      }
    }

    #endregion

    #region Constructors

    public ObservableDictionary()
    {
      InitializeGlobalSyncVariables();
      _dictionary = new Dictionary<TKey, TValue>();
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
    {
      InitializeGlobalSyncVariables();
      _dictionary = new Dictionary<TKey, TValue>(dictionary);
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
    {
      InitializeGlobalSyncVariables();
      _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
    }

    public ObservableDictionary(IEqualityComparer<TKey> comparer)
    {
      InitializeGlobalSyncVariables();
      _dictionary = new Dictionary<TKey, TValue>(comparer);
    }

    public ObservableDictionary(int capacity)
    {
      InitializeGlobalSyncVariables();
      _dictionary = new Dictionary<TKey, TValue>(capacity);
    }

    public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
      InitializeGlobalSyncVariables();
      _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
    }

    #endregion

    #region Private Methods

    private void InitializeGlobalSyncVariables()
    {
      _addedSync = new ReaderWriterLockSlim();
      _changedSync = new ReaderWriterLockSlim();
      _removedSync = new ReaderWriterLockSlim();
    }

    private void RaiseAddedItemEvent(KeyValuePair<TKey, TValue> item)
    {
      _addedSync.EnterReadLock();
      try
      {
        if (_added != null)
          _added(this, item);
      }
      finally
      {
        _addedSync.ExitReadLock();
      }
    }

    private void RaiseChangedItemEvent(KeyValuePair<TKey, TValue> item)
    {
      _changedSync.EnterReadLock();
      try
      {
        if (_changed != null)
          _changed(this, item);
      }
      finally
      {
        _changedSync.ExitReadLock();
      }
    }

    private void RaiseRemovedItemEvent(KeyValuePair<TKey, TValue> item)
    {
      _removedSync.EnterReadLock();
      try
      {
        if (_removed != null)
          _removed(this, item);
      }
      finally
      {
        _removedSync.ExitReadLock();
      }
    }

    #endregion

    #region IDictionary<TKey,TValue> Members

    /// <summary>
    /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the dictionary.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
    {
      KeyValuePair<TKey, TValue> item = new KeyValuePair<TKey, TValue>(key, value);
      Add(item);
    }

    /// <summary>
    /// Determines whether the <see cref="ObservableDictionary{TKey,TValue}"/> contains the specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(TKey key)
    {
      return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// Gets a collection containing the keys in the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// Changes in this collection are not observable.
    /// Readd the key if the action needs to be observable.
    /// </summary>
    public ICollection<TKey> Keys
    {
      get { return _dictionary.Keys; }
    }

    /// <summary>
    /// Removes the value with the specified key from the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Remove(TKey key)
    {
      if (!_dictionary.ContainsKey(key))
        return false;
      KeyValuePair<TKey, TValue> item
        = new KeyValuePair<TKey, TValue>(key, _dictionary[key]);
      return Remove(item);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
      return _dictionary.TryGetValue(key, out value);
    }

    /// <summary>
    /// Gets a collection containing the values in the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// Changes in this collection are not observable.
    /// Use the indexer property if the action needs to be observable.
    /// </summary>
    public ICollection<TValue> Values
    {
      get { return _dictionary.Values; }
    }

    /// <summary>
    /// Gets or sets the value associated with the specified key.
    /// Actions using the setter are observable.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue this[TKey key]
    {
      get { return _dictionary[key]; }
      set
      {
        _dictionary[key] = value;
        RaiseChangedItemEvent(new KeyValuePair<TKey, TValue>(key, value));
      }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    /// <summary>
    /// Adds the specified <see cref="KeyValuePair{TKey,TValue}"/> to the dictionary.
    /// </summary>
    /// <param name="item"></param>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
      _dictionary.Add(item);
      RaiseAddedItemEvent(item);
    }

    /// <summary>
    /// Removes all keys and values from the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    public void Clear()
    {
      _dictionary.Clear();
    }

    /// <summary>
    /// Determines whether the <see cref="ObservableDictionary{TKey,TValue}"/>
    /// contains the specified <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
      return _dictionary.Contains(item);
    }

    /// <summary>
    /// Copies the elements of the <see cref="ObservableDictionary{TKey,TValue}"/> to an array,
    /// starting at the specified array index.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
      _dictionary.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Gets the number of <see cref="KeyValuePair{TKey,TValue}"/>s contained
    /// in the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    public int Count
    {
      get { return _dictionary.Count; }
    }

    /// <summary>
    /// Gets a value indicating whether the <see cref="ObservableDictionary{TKey,TValue}"/> is read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return _dictionary.IsReadOnly; }
    }

    /// <summary>
    /// Removes the specified <see cref="KeyValuePair{TKey,TValue}"/>
    /// from the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
      if (!_dictionary.Remove(item))
        return false;
      RaiseRemovedItemEvent(item);
      return true;
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
      return _dictionary.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    /// <summary>
    /// Returns an enumerator that iterates through the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <returns></returns>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _dictionary.GetEnumerator();
    }

    #endregion

  }
}
