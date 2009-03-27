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
using System.Xml.Serialization;
using AppStract.Utilities.Assembly;

namespace AppStract.Core.Data.Settings
{
  /// <summary>
  /// Holds all configuration variables.
  /// </summary>
  public class Configuration
  {

    #region Constants

    /// <summary>
    /// The XML file for the application configuration.
    /// </summary>
    private const string _appConfigFile = "AppConfig.xml";
    /// <summary>
    /// The XML file for the user configuration.
    /// </summary>
    private const string _userConfigFile = "UserConfig.xml";

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the configuration of the application's constants.
    /// </summary>
    public AppConfig AppConfig
    {
      get; set;
    }

    /// <summary>
    /// Gets or sets the configuration variables defined by the user.
    /// </summary>
    public UserConfig UserConfig
    {
      get; set;
    }

    /// <summary>
    /// Gets or sets the configuration of the current application's instance.
    /// </summary>
    public DynamicConfig DynConfig
    {
      get; set;
    }

    #endregion

    #region Constructors

    private Configuration() { }

    #endregion

    #region Public Methods

    public static Configuration LoadConfiguration()
    {
      return new Configuration
               {
                 AppConfig = LoadAppConfig(),
                 UserConfig = LoadUserConfig(),
                 DynConfig = LoadDynConfig()
               };
    }

    #endregion

    #region Private Methods

    private static AppConfig LoadAppConfig()
    {
      try
      {
        return Deserialize<AppConfig>(_appConfigFile);
      }
      catch (ApplicationException ex)
      {
        ServiceCore.Log.Error("Could not load the application configuration.", ex);
        return null;
      }
    }

    private static UserConfig LoadUserConfig()
    {
      try
      {
        return Deserialize<UserConfig>(_userConfigFile);
      }
      catch (ApplicationException ex)
      {
        ServiceCore.Log.Error("Could not load the user configuration.", ex);
        return null;
      }
    }

    private static DynamicConfig LoadDynConfig()
    {
      var dynConfig = new DynamicConfig();
      /// Set the root directory of the current process.
      var entryAssemblyFile = AssemblyHelper.GetEntryAssembly().Location;
      if (entryAssemblyFile != null)
        dynConfig.Root = Path.GetDirectoryName(entryAssemblyFile);
      if (string.IsNullOrEmpty(dynConfig.Root))
        /// Don't use "else ...", Path.GetDirectoryName() might return an empty string.
        dynConfig.Root = Environment.CurrentDirectory;
      return dynConfig;
    }

    private static T Deserialize<T>(string filename)
    {
      var serializer = new XmlSerializer(typeof(T));
      try
      {
        using (var stream = new StreamReader(filename))
        {
          return (T)serializer.Deserialize(stream);
        }
      }
      catch (Exception e)
      {
        throw new ApplicationException("Can't deserialize an object of type " + typeof(T) + " from " + filename, e);
      }
    }

    #endregion

  }
}
