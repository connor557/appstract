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
using AppStract.Utilities.Helpers;
using AppStract.Utilities.ManagedFusion.Insuring;

namespace AppStract.Core.Data.Settings
{
  [Serializable]
  public class UserConfig
  {

    #region Constants

    private const SerializerType _SerializerType = SerializerType.XML;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the file to use as logfile.
    /// </summary>
    public string LogFile
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the method to use for insuring a clean GAC.
    /// </summary>
    public CleanUpInsuranceFlags GacCleanUpInsuranceFlags
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
          return SerializationHelper.Deserialize<UserConfig>(filename, _SerializerType);
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
      return SerializationHelper.TrySerialize(filename, cnf, _SerializerType);
    }

    #endregion

    #region Private Methods

    private void LoadDefaults()
    {
      LogFile = "AppStract.log";
      GacCleanUpInsuranceFlags = CleanUpInsuranceFlags.TrackByFile | CleanUpInsuranceFlags.ByWatchService;
    }

    #endregion

  }
}
