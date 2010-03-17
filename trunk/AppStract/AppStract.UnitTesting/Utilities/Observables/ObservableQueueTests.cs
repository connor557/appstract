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

using System.Threading;
using AppStract.Utilities.Observables;
using NUnit.Framework;

namespace AppStract.UnitTesting.Utilities.Observables
{
  [TestFixture]
  public class ObservableQueueTests
  {

    private static EventWaitHandle _handle;

    [Test]
    public void EnqueueItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableQueue<string>();
      test.ItemEnqueued += Queue_ItemEnqueued;
      test.Enqueue("test");
      Assert.IsTrue(_handle.WaitOne(10));
    }

    [Test]
    public void DequeueItem()
    {
      _handle = new EventWaitHandle(false, EventResetMode.ManualReset);
      var test = new ObservableQueue<string>();
      test.Enqueue("test");
      test.ItemDequeued += Queue_ItemDequeued;
      test.Dequeue();
      Assert.IsTrue(_handle.WaitOne(10));
    }

    static void Queue_ItemEnqueued(object sender, QueueChangedEventArgs<string> e)
    {
      _handle.Set();
    }

    static void Queue_ItemDequeued(object sender, QueueChangedEventArgs<string> e)
    {
      _handle.Set();
    }

  }
}
