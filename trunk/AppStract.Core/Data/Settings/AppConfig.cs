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

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.GAC;
using AppStract.Utilities.Helpers;

namespace AppStract.Core.Data.Settings
{
  [Serializable]
  public class AppConfig
  {

    #region Properties

    /// <summary>
    /// Gets or sets the default location of the application data file.
    /// </summary>
    public string DefaultApplicationDataFile
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the library to inject.
    /// </summary>
    public string LibtoInject
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the executable of the wrapper to use for managed targets.
    /// </summary>
    public string WrapperExecutable
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets all libraries that must be shared between the current process and the other/guest process.
    /// </summary>
    public List<string> LibsToShare
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the <see cref="InstallerDescription"/> to use for manipulating the global assembly cache.
    /// </summary>
    public InstallerDescription GacInstallerDescription
    {
      get;
      set;
    }

    #endregion

    #region Static Methods

    public static AppConfig LoadFrom(string filename)
    {
      try
      {
        if (File.Exists(filename))
          return XmlSerializationHelper.Deserialize<AppConfig>(filename);
      }
      catch (Exception ex)
      {
        CoreBus.Log.Error("Could not load the application configuration.", ex);
      }
      var r = new AppConfig();
      r.LoadDefaults();
      return r;
    }

    public static bool SaveTo(AppConfig cnf, string filename)
    {
      return XmlSerializationHelper.TrySerialize(filename, cnf);
    }

    #endregion

    #region Private Methods

    private void LoadDefaults()
    {
      DefaultApplicationDataFile = "ApplicationData.xml";
      LibtoInject = "AppStract.Inject.dll";
      WrapperExecutable = "Appstract.Wrapper.exe";
      LibsToShare = new List<string>(
        new[]
          {
            "EasyHook.dll",
            "AppStract.Core.dll",
            "AppStract.Host.exe",
            "AppStract.Inject.dll",
            "AppStract.Manager.exe",
            "AppStract.Server.dll",
            "AppStract.Utilities.dll"
          });
      GacInstallerDescription
        = InstallerDescription.CreateForFile("AppStract Server", CoreBus.Runtime.RunningExecutable);
    }

    #endregion

  }
}
