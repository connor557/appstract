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
using AppStract.Utilities.Observables;
using NUnit.Framework;

namespace AppStract.UnitTesting.Utilities.Observables
{
  [TestFixture]
  public class ObservableListTests
  {

    private static EventWaitHandle _handle;

    [Test]
    public void AddItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableList<string>();
      test.ItemAdded += List_ItemEvent;
      test.Add("myValue");
      Assert.IsTrue(_handle.WaitOne(10));
    }

    [Test]
    public void RemoveItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableList<string>();
      test.Add("myValue");
      test.ItemRemoved += List_ItemEvent;
      test.Remove("myValue");
      Assert.IsTrue(_handle.WaitOne(10));
    }

    [Test]
    public void ChangeItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableList<string>();
      test.Add("myValue");
      test.ItemChanged += List_ItemEvent;
      test[test.IndexOf("myValue")] = "myNewValue";
      Assert.IsTrue(_handle.WaitOne(10));
    }

    [Test]
    public void ChangeCollection()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableList<string>();
      test.Changed += List_Changed;
      test.Add("myValue");
      Assert.IsTrue(_handle.WaitOne(10), "Add() is not recognized as a change");
      _handle.Reset();
      test[0] = "newValue";
      Assert.IsTrue(_handle.WaitOne(10), "this[] is not recognized as a change");
      _handle.Reset();
      test.RemoveAt(0);
      Assert.IsTrue(_handle.WaitOne(10), "RemoveAt() is not recognized as a change");
      _handle.Reset();
      test.Add("myValue");
      Assert.IsTrue(_handle.WaitOne(10), "Add() is not recognized as a change");
      _handle.Reset();
      test.Remove("myValue");
      Assert.IsTrue(_handle.WaitOne(10), "Remove() is not recognized as a change");
    }

    [Test]
    public void NestedObservables()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableList<ObservableItem>();
      test.ItemChanged += ObservableItemChanged;
      var item = new ObservableItem();
      test.Add(item);
      item.ReportChange();
      Assert.IsTrue(_handle.WaitOne(10));
    }

    static void List_ItemEvent(ICollection<string> sender, string item, EventArgs args)
    {
      _handle.Set();
    }

    static void List_Changed(object sender, EventArgs args)
    {
      _handle.Set();
    }

    static void ObservableItemChanged(ICollection<ObservableItem> sender, ObservableItem item, EventArgs args)
    {
      _handle.Set();
    }

  }
}
