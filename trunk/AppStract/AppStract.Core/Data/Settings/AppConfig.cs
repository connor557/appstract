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
using System.IO;
using AppStract.Utilities.Helpers;
using AppStract.Utilities.ManagedFusion;

namespace AppStract.Host.Data.Settings
{
  [Serializable]
  public class AppConfig
  {

    #region Constants

    private const SerializerType _SerializerType = SerializerType.XML;

    #endregion

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
    /// Gets or sets the executable of the watcher to use for cleaning the GAC after process termination.
    /// </summary>
    public string WatcherExecutable
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
    /// Gets or sets the folder containing the 'GAC-CleanUp-Insurances'.
    /// </summary>
    public string GacCleanUpInsuranceFolder
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the registry key containing the 'GAC-CleanUp-Insurances'.
    /// </summary>
    /// <remarks>
    /// The specified key is always interpreted as a subkey of the CurrentUser rootkey.
    /// </remarks>
    public string GacCleanUpInsuranceRegistryKey
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
          return SerializationHelper.Deserialize<AppConfig>(filename, _SerializerType);
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
      return SerializationHelper.TrySerialize(filename, cnf, _SerializerType);
    }

    #endregion

    #region Private Methods

    private void LoadDefaults()
    {
      DefaultApplicationDataFile = "ApplicationData.xml";
      LibtoInject = "AppStract.Inject.dll";
      WatcherExecutable = "AppStract.Watcher.exe";
      WrapperExecutable = "Appstract.Wrapper.exe";
      LibsToShare = new List<string>(
        new[]
          {
            "EasyHook.dll",
            "AppStract.Host.dll",
            "AppStract.Inject.dll",
            "AppStract.Manager.exe",
            "AppStract.Engine.dll",
            "AppStract.Utilities.dll"
          });
      GacCleanUpInsuranceFolder = CoreBus.Runtime.StartUpDirectory + @"\GAC";
      GacCleanUpInsuranceRegistryKey = @"Software\AppStract";
      GacInstallerDescription
        = InstallerDescription.CreateForFile("AppStract Server", CoreBus.Runtime.RunningExecutable);
    }

    #endregion

  }
}
