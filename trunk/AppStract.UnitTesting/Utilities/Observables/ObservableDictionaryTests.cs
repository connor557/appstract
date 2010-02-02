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

using System.Collections.Generic;
using System.Threading;
using AppStract.Utilities.Observables;
using NUnit.Framework;

namespace AppStract.UnitTesting.Utilities.Observables
{
  [TestFixture]
  public class ObservableDictionaryTests
  {

    private static EventWaitHandle _handle;

    [Test]
    public void AddItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableDictionary<int, string>();
      test.ItemAdded += Dictionary_ItemEvent;
      test.Add(0, "myValue");
      Assert.IsTrue(_handle.WaitOne(10));
    }

    [Test]
    public void ChangeItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableDictionary<int, string>();
      test.Add(0, "myValue");
      test.ItemChanged += Dictionary_ItemEvent;
      test[0] = "test";
      Assert.IsTrue(_handle.WaitOne(10));
    }

    [Test]
    public void DeleteItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableDictionary<int, string>();
      test.ItemRemoved += Dictionary_ItemEvent;
      test.Add(0, "myValue");
      test.Remove(0);
      Assert.IsTrue(_handle.WaitOne(10));
    }

    [Test]
    public void ChangeCollection()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableDictionary<int, string>();
      test.Changed += Dictionary_Changed;
      test.Add(0, "myValue");
      Assert.IsTrue(_handle.WaitOne(10), "Add() is not recognized as a change");
      _handle.Reset();
      test[0] = "newValue";
      Assert.IsTrue(_handle.WaitOne(10), "this[] is not recognized as a change");
      _handle.Reset();
      test.Remove(0);
      Assert.IsTrue(_handle.WaitOne(10), "Remove() is not recognized as a change");
    }

    [Test]
    public void NestedObservables()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableDictionary<ObservableItem, ObservableItem>();
      test.ItemChanged += ObservableItemChanged;
      var key = new ObservableItem();
      var value = new ObservableItem();
      test.Add(key, value);
      key.ReportChange();
      Assert.IsTrue(_handle.WaitOne(10), "The key is not detected as an observable");
      _handle.Reset();
      value.ReportChange();
      Assert.IsTrue(_handle.WaitOne(10), "The value is not detected as an observable");
    }

    static void Dictionary_ItemEvent(ICollection<KeyValuePair<int, string>> sender, KeyValuePair<int, string> item)
    {
      _handle.Set();
    }

    static void Dictionary_Changed(KeyValuePair<int, string> item)
    {
      _handle.Set();
    }

    static void ObservableItemChanged(ICollection<KeyValuePair<ObservableItem, ObservableItem>> sender, KeyValuePair<ObservableItem, ObservableItem> item)
    {
      _handle.Set();
    }

  }
}
