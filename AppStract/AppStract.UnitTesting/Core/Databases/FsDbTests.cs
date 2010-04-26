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

using System.IO;
using System.Linq;
using System.Threading;
using AppStract.Core;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine.FileSystem;
using NUnit.Framework;

namespace AppStract.UnitTesting.Core.Databases
{

  [TestFixture]
  public class FsDbTests
  {

    FileTableEntry entry = new FileTableEntry("someKey", "someValue", FileKind.File);
    FileTableEntry nonExistingEntry = new FileTableEntry("noKey", "noValue", FileKind.Unspecified);

    [SetUp]
    public void SetUp()
    {
      try
      {
        File.Delete(DbConstants.DatabaseFile);
      }
      catch { }
      CoreManager.InitializeCore();
    }

    [TearDown]
    public void CleanUp()
    {
      try
      {
        File.Delete(DbConstants.DatabaseFile);
      }
      catch { }
    }

    [Test]
    public void InsertNewItem()
    {
      var db = FileSystemDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      db.EnqueueAction(new DatabaseAction<FileTableEntry>(entry, DatabaseActionType.Set));
      Thread.Sleep(500);  // Give the database some time to write
      var items = db.ReadAll();
      var rEntry = items.First();
      bool equals = entry.Key == rEntry.Key;
      equals = entry.Value == rEntry.Value ? equals : false;
      equals = entry.FileKind == rEntry.FileKind ? equals : false;
      Assert.IsTrue(equals);
    }

    [Test]
    public void ReInsertItem()
    {
      var db = FileSystemDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      db.EnqueueAction(new DatabaseAction<FileTableEntry>(entry, DatabaseActionType.Set));
      Thread.Sleep(500);  // Give the database some time to write
      var itemCount = db.ReadAll().Count();
      Assert.IsTrue(itemCount == 1);
    }

    [Test]
    public void UpdateItem()
    {
      var db = FileSystemDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      entry.Value = "someUpdatedValue";
      db.EnqueueAction(new DatabaseAction<FileTableEntry>(entry, DatabaseActionType.Set));
      Thread.Sleep(500);  // Give the database some time to write
      var items = db.ReadAll();
      var cnt = items.Count();
      Assert.IsTrue(cnt == 1, "Counted " + cnt + " objects after updating.");
      var rEntry = items.First();
      bool equals = entry.Key == rEntry.Key;
      equals = entry.Value == rEntry.Value ? equals : false;
      equals = entry.FileKind == rEntry.FileKind ? equals : false;
      Assert.IsTrue(equals);
    }

    [Test]
    public void DeleteInsertedItem()
    {
      var db = FileSystemDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      db.EnqueueAction(new DatabaseAction<FileTableEntry>(entry, DatabaseActionType.Remove));
      Thread.Sleep(500);  // Give the database some time to write
      var cnt = db.ReadAll().Count();
      Assert.IsTrue(cnt == 0, "Counted " + cnt + " objects after removing.");
    }

    [Test]
    public void DeleteNonExistingItem()
    {
      var db = FileSystemDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      var cntFirst = db.ReadAll().Count();
      db.EnqueueAction(new DatabaseAction<FileTableEntry>(nonExistingEntry, DatabaseActionType.Remove));
      Thread.Sleep(500);  // Give the database some time to write
      var cntNext = db.ReadAll().Count();
      Assert.IsTrue(cntFirst == cntNext, "Counted " + cntNext + " while " + cntFirst + " was expected.");
    }

  }
}
