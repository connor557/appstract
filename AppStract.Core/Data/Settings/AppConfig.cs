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

namespace AppStract.Core.Data.Settings
{
  [Serializable]
  public class AppConfig : IConfigurationObject
  {

    #region Properties

    /// <summary>
    /// Gets or sets the default location of the application data file.
    /// </summary>
    public string DefaultApplicationDataFile
    {
      get; set;
    }

    /// <summary>
    /// Gets or sets the library to inject.
    /// </summary>
    public string LibtoInject
    {
      get; set;
    }

    /// <summary>
    /// Gets or sets all libraries to register with EasyHook.
    /// </summary>
    public List<string> LibsToRegister
    {
      get; set;
    }

    #endregion

    #region IConfigurationObject Members

    public void LoadDefaults()
    {
      DefaultApplicationDataFile = "ApplicationData.xml";
      LibtoInject = "AppStract.Inject.dll";
      LibsToRegister = new List<string>(
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
    }

    #endregion

  }
}
