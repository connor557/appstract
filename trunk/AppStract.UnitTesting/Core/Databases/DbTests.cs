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
using System.IO;
using System.Linq;
using AppStract.Core;
using AppStract.Core.Data.Databases;
using NUnit.Framework;

namespace AppStract.UnitTesting.Core.Databases
{
  [TestFixture]
  public class DbTests
  {

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
      } catch { }
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void FaultyConnectionString()
    {
      new FileSystemDatabase("RandomConnectionString");
    }

    [Test]
    [ExpectedException(typeof(DatabaseException))]
    public void ReadAllFileBeforeInitialization()
    {
      FileSystemDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile).ReadAll();
    }

    [Test]
    [ExpectedException(typeof(DatabaseException))]
    public void ReadAllRegBeforeInitialization()
    {
      RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile).ReadAll();
    }

    [Test]
    public void ReadAllFileOnEmptyDatabase()
    {
      // Don't expect exceptions and expect the database to be empty
      var fsDb = FileSystemDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      fsDb.Initialize();
      var fsCnt = fsDb.ReadAll().Count();
      Assert.IsTrue(fsCnt == 0, "FsDb counted: " + fsCnt);
    }

    [Test]
    public void ReadAllRegOnEmptyDatabase()
    {
      // Don't expect exceptions and expect the database to be empty
      var regDb = RegistryDatabase.CreateDefaultDatabase(DbConstants.DatabaseFile);
      regDb.Initialize();
      var regCnt = regDb.ReadAll().Count();
      Assert.IsTrue(regCnt == 0, "   RegDb counted: " + regCnt);
    }

  }
}
