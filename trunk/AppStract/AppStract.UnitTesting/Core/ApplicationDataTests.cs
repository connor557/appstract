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
using AppStract.Host;
using AppStract.Host.Data.Application;
using NUnit.Framework;

namespace AppStract.UnitTesting.Core
{
  [TestFixture]
  public class ApplicationDataTests
  {

    private const string ApplicationDataXmlFile = "unitTestApplicationData.xml";
    private ApplicationData applicationData;

    [TestFixtureSetUp]
    public void SetUp()
    {
      try
      {
        File.Delete(ApplicationDataXmlFile);
      } catch { }
      CoreManager.InitializeCore();
    }

    [TestFixtureTearDown]
    public void TearDown()
    {
      try
      {
        File.Delete(ApplicationDataXmlFile);
      }
      catch { }
    }

    [Test]
    [ExpectedException(typeof(FileNotFoundException))]
    public void ApplicationDataCreateInitial()
    {
      applicationData = new ApplicationData();
      applicationData.Files.RegistryDatabase = new ApplicationFile("myTestDir\\testRegDatabase.db3");
      applicationData.Files.RootDirectory = new ApplicationFile("myTestDir\\");
      applicationData.Files.Executable = new ApplicationFile("myTestDir\\DoesntExist.exe");
    }

    [Test]
    public void ApplicationDataSerialize()
    {
      Assert.IsTrue(ApplicationData.Save(applicationData, ApplicationDataXmlFile));
    }

    [Test]
    public void ApplicationDataDeserialize()
    {
      var appData = ApplicationData.Load(ApplicationDataXmlFile);
      Assert.IsTrue(applicationData.Files.RegistryDatabase.ToString() == appData.Files.RegistryDatabase.ToString(),
                    "DatabaseRegistry doesn't match");
      Assert.IsTrue(applicationData.Files.Executable == appData.Files.Executable
                    || applicationData.Files.Executable.ToString() == appData.Files.Executable.ToString(),
                    "Executable doesn't match");
      Assert.IsTrue(applicationData.Files.RootDirectory.ToString() == appData.Files.RootDirectory.ToString(),
                    "RootDirectory doesn't match");
    }

    [Test]
    public void ArePathsRelative()
    {
      if (applicationData.Files.RegistryDatabase != null)
        Assert.IsFalse(Path.IsPathRooted(applicationData.Files.RegistryDatabase.FileName), "DatabaseRegistry.FileName is not relative");
      if (applicationData.Files.Executable != null)
        Assert.IsFalse(Path.IsPathRooted(applicationData.Files.Executable.FileName), "Executable.FileName is not relative");
      if (applicationData.Files.RootDirectory != null)
        Assert.IsFalse(Path.IsPathRooted(applicationData.Files.RootDirectory.FileName), "RootDirectory.FileName is not relative");
    }

  }
}
