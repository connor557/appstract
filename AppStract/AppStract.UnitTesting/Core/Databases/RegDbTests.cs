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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AppStract.Core;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine.Registry;
using NUnit.Framework;
using ValueType = AppStract.Core.Virtualization.Engine.Registry.ValueType;

namespace AppStract.UnitTesting.Core.Databases
{
  [TestFixture]
  public class RegDbTests
  {
    private VirtualRegistryValue entryValue;
    private VirtualRegistryKey entryKey;
    private VirtualRegistryKey nonExistingEntryKey = new VirtualRegistryKey(987, "SomeWrongValue");

    public RegDbTests()
    {
      entryValue = new VirtualRegistryValue("myValue", new ASCIIEncoding().GetBytes("someData"), ValueType.REG_SZ);
      entryKey = new VirtualRegistryKey(456, @"HKEY_USERS\MyTestUser\TestEntry",
                                        new Dictionary<string, VirtualRegistryValue> { { entryValue.Name, entryValue } });
    }

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
      var db = RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      db.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(entryKey, DatabaseActionType.Set));
      Thread.Sleep(500);  // Give the database some time to write
      var items = db.ReadAll();
      var rEntry = items.First();
      Assert.IsTrue(rEntry.Handle == entryKey.Handle, "Handle doesn't match.");
      Assert.IsTrue(rEntry.Path == entryKey.Path, "Path doesn't match.");
      Assert.IsTrue(rEntry.Values.Count == entryKey.Values.Count, "Value count doesn't match.");
      var firstValue = rEntry.Values.First();
      Assert.IsTrue(firstValue.Value.Name == entryValue.Name, "Value's name doesn't match.");
      Assert.IsTrue(firstValue.Value.Type == entryValue.Type, "Value's type doesn't match.");
      // Note: Can't compare binary values like this! Verify if extension method is called when Data is of type byte[]
      string data = firstValue.Value.Data.AsString();
      Assert.IsTrue(data == entryValue.Data.AsString(), "Value's data doesn't match.");
    }

    [Test]
    public void ReInsertNewItem()
    {
      var db = RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      db.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(entryKey, DatabaseActionType.Set));
      Thread.Sleep(500);  // Give the database some time to write
      var itemCount = db.ReadAll().Count();
      Assert.IsTrue(itemCount == 1, "Item count is " + itemCount);
    }

    [Test]
    public void UpdateItemsValue()
    {
      var db = RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      entryValue.Data = new ASCIIEncoding().GetBytes("myUpdatedValue");
      entryKey.Values[entryValue.Name] = entryValue;
      db.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(entryKey, DatabaseActionType.Set));
      Thread.Sleep(500);  // Give the database some time to write
      var items = db.ReadAll();
      var rEntry = items.First();
      // Note: Can't compare binary values like this! Verify if extension method is called when Data is of type byte[]
      var data = rEntry.Values[entryValue.Name].Data.AsString();
      Assert.IsTrue(data == entryValue.Data.AsString(),
        "Input data is \"" + entryValue.Data.AsString() + "\"  while output data is \"" + data + "\"");
    }

    [Test]
    public void UpdateItemsKey()
    {
      var db = RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      entryKey = new VirtualRegistryKey(entryKey.Handle,
                                        @"HKEY_USERS\MyTestUser\UpdatedTestEntry",
                                        entryKey.Values);
      db.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(entryKey, DatabaseActionType.Set));
      Thread.Sleep(500);  // Give the database some time to write
      var items = db.ReadAll();
      var rEntry = items.First();
      Assert.IsTrue(rEntry.Path == entryKey.Path,
        "Inputted path is \"" + entryKey.Path + "\"  while outputted path is \"" + rEntry.Path + "\"");
    }

    [Test]
    public void DeleteInsertedItem()
    {
      var db = RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      db.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(entryKey, DatabaseActionType.Remove));
      Thread.Sleep(500);  // Give the database some time to write
      var cnt = db.ReadAll().Count();
      Assert.IsTrue(cnt == 0, "Counted " + cnt + " objects after removing.");
    }

    [Test]
    public void DeleteNonExistingItem()
    {
      var db = RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      db.Initialize();
      var cntFirst = db.ReadAll().Count();
      db.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(nonExistingEntryKey, DatabaseActionType.Remove));
      Thread.Sleep(500);  // Give the database some time to write
      var cntNext = db.ReadAll().Count();
      Assert.IsTrue(cntFirst == cntNext, "Counted " + cntNext + " while " + cntFirst + " was expected.");
    }

  }

  static class Extensions
  {
    public static string AsString(this IEnumerable<byte> array)
    {
      string result = "";
      foreach (var b in array)
        result += b + " ";
      return result;
    }
  }

}
