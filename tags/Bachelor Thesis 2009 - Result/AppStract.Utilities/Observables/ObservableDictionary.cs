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
using System.Linq;

namespace AppStract.Utilities.Observables
{
  /// <summary>
  /// Observable wrapper class for <see cref="IDictionary{TKey,TValue}"/>,
  /// all events are raised in an asynchronous way.
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IObservableCollection<KeyValuePair<TKey, TValue>>
  {

    #region Variables

    private readonly IDictionary<TKey, TValue> _dictionary;
    private NotifyCollectionItem<KeyValuePair<TKey, TValue>> _added;
    private NotifyCollectionItem<KeyValuePair<TKey, TValue>> _changed;
    private NotifyCollectionItem<KeyValuePair<TKey, TValue>> _removed;
    private object _addLock;
    private object _changeLock;
    private object _removeLock;
    private bool _keyTypeIsObservable;
    private bool _valueTypeIsObservable;

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
      _addLock = new object();
      _changeLock = new object();
      _removeLock = new object();
      _keyTypeIsObservable = typeof(IObservableItem<TKey>).IsAssignableFrom(typeof(TKey));
      _valueTypeIsObservable = typeof(IObservableItem<TValue>).IsAssignableFrom(typeof(TValue));
    }

    private void AttachEvents(TKey key)
    {
      if (_keyTypeIsObservable)
        ((IObservableItem<TKey>)key).Changed += Key_Changed;
    }

    private void AttachEvents(TValue value)
    {
      if (_valueTypeIsObservable)
        ((IObservableItem<TValue>)value).Changed += Value_Changed;
    }

    private void DettachEvents(TKey key)
    {
      if (_keyTypeIsObservable)
        ((IObservableItem<TKey>)key).Changed -= Key_Changed;
    }

    private void DettachEvents(TValue value)
    {
      if (_valueTypeIsObservable)
        ((IObservableItem<TValue>)value).Changed -= Value_Changed;
    }

    private void Key_Changed(TKey item)
    {
      TValue value;
      /// Check if the key exists, if not: detach the event and return quietly.
      if (!_dictionary.TryGetValue(item, out value))
        ((IObservableItem<TKey>) item).Changed -= Key_Changed;
      else
        new NotifyCollectionItemEventRaiser<KeyValuePair<TKey, TValue>>
          (_changed, this, new KeyValuePair<TKey, TValue>(item, value), _changeLock).RaiseAsync();
    }

    private void Value_Changed(TValue item)
    {
      /// Find the key.
      var key = from pair in _dictionary
                where pair.Value.Equals(item)
                select pair.Key;
      /// Check if the key exists, if not: detach the event and return quietly.
      if (key.Count() == 0)
      {
        ((IObservableItem<TValue>)item).Changed -= Value_Changed;
        return;
      }
      /// Raise the event.
      new NotifyCollectionItemEventRaiser<KeyValuePair<TKey, TValue>>
        (_changed, this, new KeyValuePair<TKey, TValue>(key.First(), item), _changeLock).RaiseAsync();
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
      /// Pass this item to the ICollection implementation of Add()
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
        TValue oldValue = _dictionary[key];
        if (oldValue.Equals(value))
          return;
        _dictionary[key] = value;
        DettachEvents(oldValue);
        AttachEvents(value);
        new NotifyCollectionItemEventRaiser<KeyValuePair<TKey, TValue>>
          (_changed, this, new KeyValuePair<TKey, TValue>(key, value), _changeLock).RaiseAsync();
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
      AttachEvents(item.Key);
      AttachEvents(item.Value);
      new NotifyCollectionItemEventRaiser<KeyValuePair<TKey, TValue>>(_added, this, item, _addLock).RaiseAsync();
    }

    /// <summary>
    /// Removes all keys and values from the <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    public void Clear()
    {
      var array = new KeyValuePair<TKey, TValue>[_dictionary.Count];
      _dictionary.CopyTo(array, 0);
      _dictionary.Clear();
      foreach (var item in array)
        new NotifyCollectionItemEventRaiser<KeyValuePair<TKey, TValue>>
          (_removed, this, item, _removeLock).RaiseAsync();
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
      DettachEvents(item.Key);
      DettachEvents(item.Value);
      new NotifyCollectionItemEventRaiser<KeyValuePair<TKey, TValue>>(_removed, this, item, _removeLock).RaiseAsync();
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

    #region IObservableItem<KeyValuePair<TKey,TValue>> Members

    public event NotifyItem<KeyValuePair<TKey, TValue>> Changed;

    #endregion

    #region IObservableCollection<KeyValuePair<TKey,TValue>> Members

    public event NotifyCollectionItem<KeyValuePair<TKey, TValue>> ItemAdded
    {
      add { lock(_addLock) _added += value; }
      remove { lock (_addLock) _added -= value; }
    }

    public event NotifyCollectionItem<KeyValuePair<TKey, TValue>> ItemChanged
    {
      add { lock (_changeLock) _changed += value; }
      remove { lock (_changeLock) _changed -= value; }
    }

    public event NotifyCollectionItem<KeyValuePair<TKey, TValue>> ItemRemoved
    {
      add { lock (_removeLock) _removed += value; }
      remove { lock (_removeLock) _removed -= value; }
    }

    #endregion

  }
}
