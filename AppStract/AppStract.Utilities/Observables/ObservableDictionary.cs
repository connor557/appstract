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
using System.Linq;

namespace AppStract.Utilities.Observables
{
  /// <summary>
  /// Observable wrapper class for <see cref="IDictionary{TKey,TValue}"/>,
  /// all events are raised in an asynchronous way.
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IObservableCollection<KeyValuePair<TKey, TValue>>, IObservableItem
  {

    #region Variables

    private readonly IDictionary<TKey, TValue> _dictionary;
    private EventHandler _changed;
    private CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _itemAdded;
    private CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _itemChanged;
    private CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> _itemRemoved;
    private readonly object _changeLock;
    private readonly object _itemAddLock;
    private readonly object _itemChangeLock;
    private readonly object _itemRemoveLock;
    private readonly bool _keyTypeIsObservable;
    private readonly bool _valueTypeIsObservable;

    #endregion

    #region Constructors

    public ObservableDictionary()
    {
      _dictionary = new Dictionary<TKey, TValue>();
      _changeLock = new object();
      _itemAddLock = new object();
      _itemChangeLock = new object();
      _itemRemoveLock = new object();
      _keyTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TKey));
      _valueTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TValue));
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
    {
      _dictionary = new Dictionary<TKey, TValue>(dictionary);
      _changeLock = new object();
      _itemAddLock = new object();
      _itemChangeLock = new object();
      _itemRemoveLock = new object();
      _keyTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TKey));
      _valueTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TValue));
    }

    public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
    {
      _dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
      _changeLock = new object();
      _itemAddLock = new object();
      _itemChangeLock = new object();
      _itemRemoveLock = new object();
      _keyTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TKey));
      _valueTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TValue));
    }

    public ObservableDictionary(IEqualityComparer<TKey> comparer)
    {
      _dictionary = new Dictionary<TKey, TValue>(comparer);
      _changeLock = new object();
      _itemAddLock = new object();
      _itemChangeLock = new object();
      _itemRemoveLock = new object();
      _keyTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TKey));
      _valueTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TValue));
    }

    public ObservableDictionary(int capacity)
    {
      _dictionary = new Dictionary<TKey, TValue>(capacity);
      _changeLock = new object();
      _itemAddLock = new object();
      _itemChangeLock = new object();
      _itemRemoveLock = new object();
      _keyTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TKey));
      _valueTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TValue));
    }

    public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
    {
      _dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
      _changeLock = new object();
      _itemAddLock = new object();
      _itemChangeLock = new object();
      _itemRemoveLock = new object();
      _keyTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TKey));
      _valueTypeIsObservable = typeof(IObservableItem).IsAssignableFrom(typeof(TValue));
    }

    #endregion

    #region Private Methods

    private void AttachEvents(TKey key)
    {
      if (_keyTypeIsObservable)
        ((IObservableItem)key).Changed += Key_Changed;
    }

    private void AttachEvents(TValue value)
    {
      if (_valueTypeIsObservable)
        ((IObservableItem)value).Changed += Value_Changed;
    }

    private void DettachEvents(TKey key)
    {
      if (_keyTypeIsObservable)
        ((IObservableItem)key).Changed -= Key_Changed;
    }

    private void DettachEvents(TValue value)
    {
      if (_valueTypeIsObservable)
        ((IObservableItem)value).Changed -= Value_Changed;
    }

    private void Key_Changed(object sender, EventArgs args)
    {
      if (!(sender is TKey)) return;
      var item = (TKey) sender;
      TValue value;
      // Check if the key exists, if not: detach the event and return quietly.
      if (!_dictionary.TryGetValue(item, out value))
      {
        ((IObservableItem) item).Changed -= Key_Changed;
        return;
      }
      new CollectionChangedEventRaiser<KeyValuePair<TKey, TValue>>
        (_itemChanged, this, new KeyValuePair<TKey, TValue>(item, value), new EventArgs(), _itemChangeLock).RaiseAsync();
      // Changing a single item changes the whole collection, raise the event for this change
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changeLock).RaiseAsync();
    }

    private void Value_Changed(object sender, EventArgs args)
    {
      if (!(sender is TValue)) return;
      var item = (TValue)sender;
      // Find the key.
      var key = from pair in _dictionary
                where pair.Value.Equals(item)
                select pair.Key;
      // Check if the key exists, if not: detach the event and return quietly.
      if (key.Count() == 0)
      {
        ((IObservableItem)item).Changed -= Value_Changed;
        return;
      }
      // Raise the event.
      new CollectionChangedEventRaiser<KeyValuePair<TKey, TValue>>
        (_itemChanged, this, new KeyValuePair<TKey, TValue>(key.First(), item), new EventArgs(), _itemChangeLock).RaiseAsync();
      // Changing a single item changes the whole collection, raise the event for this change
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changeLock).RaiseAsync();
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
      // Pass this item to the ICollection implementation of Add()
      Add(new KeyValuePair<TKey, TValue>(key, value));
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
        new CollectionChangedEventRaiser<KeyValuePair<TKey, TValue>>
          (_itemChanged, this, new KeyValuePair<TKey, TValue>(key, value), new EventArgs(), _itemChangeLock).RaiseAsync();
        // Changing a single item changes the whole collection, raise the event for this change
        new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changeLock).RaiseAsync();
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
      new CollectionChangedEventRaiser<KeyValuePair<TKey, TValue>>(_itemAdded, this, item, new EventArgs(), _itemAddLock).RaiseAsync();
      // Changing a single item changes the whole collection, raise the event for this change
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changeLock).RaiseAsync();
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
        new CollectionChangedEventRaiser<KeyValuePair<TKey, TValue>>
          (_itemRemoved, this, item, new EventArgs(), _itemRemoveLock).RaiseAsync();
      // Changing a single item changes the whole collection, raise the event for this change
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changeLock).RaiseAsync();
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
      new CollectionChangedEventRaiser<KeyValuePair<TKey, TValue>>(_itemRemoved, this, item, new EventArgs(), _itemRemoveLock).RaiseAsync();
      // Changing a single item changes the whole collection, raise the event for this change
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changeLock).RaiseAsync();
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

    public event EventHandler Changed
    {
      add { lock (_changeLock) _changed += value; }
      remove { lock (_changeLock) _changed -= value; }
    }

    #endregion

    #region IObservableCollection<KeyValuePair<TKey,TValue>> Members

    public event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> ItemAdded
    {
      add { lock(_itemAddLock) _itemAdded += value; }
      remove { lock (_itemAddLock) _itemAdded -= value; }
    }

    public event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> ItemChanged
    {
      add { lock (_itemChangeLock) _itemChanged += value; }
      remove { lock (_itemChangeLock) _itemChanged -= value; }
    }

    public event CollectionChangedEventHandler<KeyValuePair<TKey, TValue>> ItemRemoved
    {
      add { lock (_itemRemoveLock) _itemRemoved += value; }
      remove { lock (_itemRemoveLock) _itemRemoved -= value; }
    }

    #endregion

  }
}
