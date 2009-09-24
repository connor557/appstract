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

namespace AppStract.Utilities.Observables
{
  /// <summary>
  /// Observable wrapper class for <see cref="IList{T}"/>.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ObservableList<T> : IList<T>, IObservableCollection<T>
  {

    #region Variables

    private readonly IList<T> _list;
    private NotifyCollectionItem<T> _added;
    private NotifyCollectionItem<T> _changed;
    private NotifyCollectionItem<T> _removed;
    private readonly object _addLock;
    private readonly object _changeLock;
    private readonly object _removeLock;
    private readonly bool _itemTypeIsObservable;

    #endregion

    #region Constructors

    public ObservableList()
    {
      _list = new List<T>();
      _addLock = new object();
      _changeLock = new object();
      _removeLock = new object();
      _itemTypeIsObservable = IsItemTypeObservable();
    }

    public ObservableList(int capacity)
    {
      _list = new List<T>(capacity);
      _addLock = new object();
      _changeLock = new object();
      _removeLock = new object();
      _itemTypeIsObservable = IsItemTypeObservable();
    }

    public ObservableList(IEnumerable<T> collection)
    {
      _list = new List<T>(collection);
      _addLock = new object();
      _changeLock = new object();
      _removeLock = new object();
      _itemTypeIsObservable = IsItemTypeObservable();
    }

    #endregion

    #region Private Methods

    private static bool IsItemTypeObservable()
    {
      return typeof(IObservableItem<T>).IsAssignableFrom(typeof(T));
    }

    private void AttachEvents(T item)
    {
      if (_itemTypeIsObservable)
        ((IObservableItem<T>)item).Changed += Item_Changed;
    }

    private void DettachEvents(T item)
    {
      if (_itemTypeIsObservable)
        ((IObservableItem<T>)item).Changed -= Item_Changed;
    }

    private void Item_Changed(T item)
    {
      new NotifyCollectionItemEventRaiser<T>(_changed, this, item, _changeLock).RaiseAsync();
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
      new NotifyCollectionItemEventRaiser<T>(_added, this, item, _addLock).RaiseAsync();
    }

    public void RemoveAt(int index)
    {
      var item = _list[index];
      _list.RemoveAt(index);
      DettachEvents(item);
      new NotifyCollectionItemEventRaiser<T>(_removed, this, item, _removeLock).RaiseAsync();
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
        DettachEvents(oldValue);
        AttachEvents(value);
      }
    }

    #endregion

    #region ICollection<T> Members

    public void Add(T item)
    {
      _list.Add(item);
      AttachEvents(item);
      new NotifyCollectionItemEventRaiser<T>(_added, this, item, _addLock).RaiseAsync();
    }

    public void Clear()
    {
      var array = new T[_list.Count];
      _list.CopyTo(array, 0);
      _list.Clear();
      foreach (var item in array)
        new NotifyCollectionItemEventRaiser<T>(_removed, this, item, _removeLock).RaiseAsync();
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
      DettachEvents(item);
      new NotifyCollectionItemEventRaiser<T>(_removed, this, item, _removeLock).RaiseAsync();
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

    public event NotifyCollectionItem<T> ItemAdded
    {
      add { lock (_addLock) _added += value; }
      remove { lock (_addLock) _added -= value; }
    }

    public event NotifyCollectionItem<T> ItemChanged
    {
      add { lock (_changeLock) _changed += value; }
      remove { lock (_changeLock) _changed -= value; }
    }

    public event NotifyCollectionItem<T> ItemRemoved
    {
      add { lock (_removeLock) _removed += value; }
      remove { lock (_removeLock) _removed -= value; }
    }

    #endregion

    #region IObservableItem<T> Members

    public event NotifyItem<T> Changed;

    #endregion

  }
}
