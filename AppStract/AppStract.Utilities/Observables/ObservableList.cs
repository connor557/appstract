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
  /// Observable wrapper class for <see cref="IList{T}"/>.
  /// </summary>
  /// <typeparam name="T">The type of elements in the collection.</typeparam>
  public class ObservableList<T> : IList<T>, IObservableCollection<T>, IObservableItem
  {

    #region Variables

    private readonly IList<T> _list;
    private EventHandler _changed;
    private CollectionChangedEventHandler<T> _itemAdded;
    private CollectionChangedEventHandler<T> _itemChanged;
    private CollectionChangedEventHandler<T> _itemRemoved;
    private readonly object _changedEventLock;
    private readonly object _itemAddedEventLock;
    private readonly object _itemChangedEventLock;
    private readonly object _itemRemovedEventLock;
    private readonly bool _itemTypeIsObservable;

    #endregion

    #region Constructors

    public ObservableList()
    {
      _list = new List<T>();
      _changedEventLock = new object();
      _itemAddedEventLock = new object();
      _itemChangedEventLock = new object();
      _itemRemovedEventLock = new object();
      _itemTypeIsObservable = IsItemTypeObservable();
    }

    public ObservableList(int capacity)
    {
      _list = new List<T>(capacity);
      _changedEventLock = new object();
      _itemAddedEventLock = new object();
      _itemChangedEventLock = new object();
      _itemRemovedEventLock = new object();
      _itemTypeIsObservable = IsItemTypeObservable();
    }

    public ObservableList(IEnumerable<T> collection)
    {
      _list = new List<T>(collection);
      _changedEventLock = new object();
      _itemAddedEventLock = new object();
      _itemChangedEventLock = new object();
      _itemRemovedEventLock = new object();
      _itemTypeIsObservable = IsItemTypeObservable();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Determines whether <see cref="T"/> implements <see cref="IObservableItem"/>.
    /// </summary>
    /// <returns></returns>
    private static bool IsItemTypeObservable()
    {
      return typeof(IObservableItem).IsAssignableFrom(typeof(T));
    }

    /// <summary>
    /// Attaches <see cref="Item_Changed"/> to <paramref name="item"/> if <see cref="_itemTypeIsObservable"/> is true.
    /// </summary>
    /// <param name="item"></param>
    private void AttachEvents(T item)
    {
      if (_itemTypeIsObservable)
        ((IObservableItem)item).Changed += Item_Changed;
    }

    /// <summary>
    /// Detaches <see cref="Item_Changed"/> from <paramref name="item"/> if <see cref="_itemTypeIsObservable"/> is true.
    /// </summary>
    /// <param name="item"></param>
    private void DetachEvents(T item)
    {
      if (_itemTypeIsObservable)
        ((IObservableItem)item).Changed -= Item_Changed;
    }

    /// <summary>
    /// Eventhandler for reported changes in items of type T, if <see cref="T"/> implements the <see cref="IObservableItem"/> interface.
    /// This eventhandler is attached and detached by <see cref="AttachEvents"/> and <see cref="DetachEvents"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void Item_Changed(object sender, EventArgs args)
    {
      if (!(sender is T))
        return;
      var item = (T)sender;
      if(!_list.Contains(item))
        return;
      // Raise the change-event for the single item
      new CollectionChangedEventRaiser<T>(_itemChanged, this, item, new EventArgs(), _itemChangedEventLock).RaiseAsync();
      // Changing a single item changes the whole collection, raise the event for this change
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changedEventLock).RaiseAsync();
    }

    #endregion

    #region IList<T> Members

    public int IndexOf(T item)
    {
      return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
      _list.Insert(index, item);
      AttachEvents(item);
      new CollectionChangedEventRaiser<T>(_itemAdded, this, item, new EventArgs(), _itemAddedEventLock).RaiseAsync();
    }

    public void RemoveAt(int index)
    {
      var item = _list[index];
      _list.RemoveAt(index);
      DetachEvents(item);
      new CollectionChangedEventRaiser<T>(_itemRemoved, this, item, new EventArgs(), _itemRemovedEventLock).RaiseAsync();
      // The collection is changed, raise the event
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changedEventLock).RaiseAsync();
    }

    public T this[int index]
    {
      get
      {
        return _list[index];
      }
      set
      {
        var oldValue = _list[index];
        if (oldValue.Equals(value))
          return;
        _list[index] = value;
        DetachEvents(oldValue);
        AttachEvents(value);
        new CollectionChangedEventRaiser<T>(_itemChanged, this, value, new EventArgs(), _itemAddedEventLock).RaiseAsync();
        // The collection is changed, raise the event
        new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changedEventLock).RaiseAsync();
      }
    }

    #endregion

    #region ICollection<T> Members

    public void Add(T item)
    {
      _list.Add(item);
      AttachEvents(item);
      new CollectionChangedEventRaiser<T>(_itemAdded, this, item, new EventArgs(), _itemAddedEventLock).RaiseAsync();
      // The collection is changed, raise the event
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changedEventLock).RaiseAsync();
    }

    public void Clear()
    {
      var array = new T[_list.Count];
      _list.CopyTo(array, 0);
      _list.Clear();
      foreach (var item in array)
        new CollectionChangedEventRaiser<T>(_itemRemoved, this, item, new EventArgs(), _itemRemovedEventLock).RaiseAsync();
      // The collection is changed, raise the event
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changedEventLock).RaiseAsync();
    }

    public bool Contains(T item)
    {
      return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      _list.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return _list.Count; }
    }

    public bool IsReadOnly
    {
      get { return _list.IsReadOnly; }
    }

    public bool Remove(T item)
    {
      if (!_list.Remove(item))
        return false;
      DetachEvents(item);
      new CollectionChangedEventRaiser<T>(_itemRemoved, this, item, new EventArgs(), _itemRemovedEventLock).RaiseAsync();
      // The collection is changed, raise the event
      new ItemChangedEventRaiser(_changed, this, new EventArgs(), _changedEventLock).RaiseAsync();
      return true;
    }

    #endregion

    #region IEnumerable<T> Members

    public IEnumerator<T> GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _list.GetEnumerator();
    }

    #endregion

    #region IObservableCollection<T> Members

    public event CollectionChangedEventHandler<T> ItemAdded
    {
      add { lock (_itemAddedEventLock) _itemAdded += value; }
      remove { lock (_itemAddedEventLock) _itemAdded -= value; }
    }

    public event CollectionChangedEventHandler<T> ItemChanged
    {
      add { lock (_itemChangedEventLock) _itemChanged += value; }
      remove { lock (_itemChangedEventLock) _itemChanged -= value; }
    }

    public event CollectionChangedEventHandler<T> ItemRemoved
    {
      add { lock (_itemRemovedEventLock) _itemRemoved += value; }
      remove { lock (_itemRemovedEventLock) _itemRemoved -= value; }
    }

    #endregion

    #region IObservableItem Members

    public event EventHandler Changed
    {
      add { lock (_changedEventLock) _changed += value; }
      remove { lock (_changedEventLock) _changed -= value; }
    }

    #endregion

  }
}
