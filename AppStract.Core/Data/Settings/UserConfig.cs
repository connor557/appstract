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
using System.IO;
using AppStract.Utilities.Helpers;

namespace AppStract.Core.Data.Settings
{
  [Serializable]
  public class UserConfig
  {

    #region Properties

    public string LogFile
    {
      get;
      set;
    }

    #endregion

    #region Static Methods

    public static UserConfig LoadFrom(string filename)
    {
      try
      {
        if (File.Exists(filename))
          return XmlSerializationHelper.Deserialize<UserConfig>(filename);
      }
      catch (Exception ex)
      {
        CoreBus.Log.Error("Could not load the user configuration.", ex);
      }
      var r = new UserConfig();
      r.LoadDefaults();
      return r;
    }

    public static bool SaveTo(UserConfig cnf, string filename)
    {
      return XmlSerializationHelper.TrySerialize(filename, cnf);
    }

    #endregion

    #region Private Methods

    private void LoadDefaults()
    {
      LogFile = "AppStract.log";
    }

    #endregion

  }
}
