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
using System.Runtime.InteropServices;
using AppStract.Core.Logging;
using AppStract.Utilities.Serialization;

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
      get; private set;
    }

    /// <summary>
    /// Gets or sets the configuration variables defined by the user.
    /// </summary>
    public UserConfig UserConfig
    {
      get; private set;
    }

    /// <summary>
    /// Gets or sets the configuration of the current application's instance.
    /// </summary>
    public DynamicConfig DynConfig
    {
      get; private set;
    }

    #endregion

    #region Constructors

    public Configuration(AppConfig appConfig, UserConfig userConfig)
    {
      AppConfig = appConfig;
      UserConfig = userConfig;
      DynConfig = new DynamicConfig();
      DynConfig.LoadDefaults();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a fully loaded configuration.
    /// </summary>
    /// <returns></returns>
    public static Configuration LoadConfiguration()
    {
      var appConfig = LoadAppConfig();
      var userConfig = LoadUserConfig();
      return new Configuration(appConfig, userConfig);
    }

    /// <summary>
    /// Saves the <see cref="Configuration"/> specified to the default configuration-files.
    /// </summary>
    /// <param name="configuration"></param>
    public static void SaveConfiguration(Configuration configuration)
    {
      if (!Serializer.TrySerialize(_appConfigFile, configuration.AppConfig))
        CoreBus.Log.Warning("Unable to save the application configuration.");
      if (!Serializer.TrySerialize(_userConfigFile, configuration.UserConfig))
        CoreBus.Log.Warning("Unable to save the user configuration.");
    }

    /// <summary>
    /// Configures the log service to use the <see cref="LogLevel"/> specified.
    /// </summary>
    /// <param name="logLevel"></param>
    public void SetLogLevel(LogLevel logLevel)
    {
      CoreBus.Log.LogLevel = logLevel;
    }

    /// <summary>
    /// Configures the log service to use the <see cref="LogType"/> specified.
    /// </summary>
    /// <param name="logType"></param>
    public void SetLogOutput(LogType logType)
    {
      SetLogOutput(logType, null);
    }

    /// <summary>
    /// Configures the log service to use the <see cref="LogType"/> specified.
    /// If the log service is a <see cref="FileLogger"/>, the filename specified is used as the destination file.
    /// </summary>
    /// <param name="logType"></param>
    /// <param name="filename"></param>
    public void SetLogOutput(LogType logType, string filename)
    {
      if (CoreBus.Log.Type == logType)
        return;
      if (logType == LogType.Null)
        CoreBus.Log = new NullLogger();
      else if (logType == LogType.Console)
        CoreBus.Log = new ConsoleLogger(CoreBus.Log.LogLevel);
      else if (logType == LogType.File)
        CoreBus.Log
          = filename == null
              ? FileLogger.CreateLogService(CoreBus.Log.LogLevel)
              : FileLogger.CreateLogService(CoreBus.Log.LogLevel, filename);
    }

    /// <summary>
    /// Shows or hides the window with the windowname specified.
    /// </summary>
    /// <param name="showWindow">True if the window must be shown; false, otherwise.</param>
    /// <param name="windowName">Name of the window to show or hide.</param>
    public void ShowWindow(bool showWindow, string windowName)
    {
      bool visible = showWindow.ToString() == "1"
                     || showWindow.ToString().ToLowerInvariant() == "true";
      if (visible)
        return;
      /// Hide the window.
      IntPtr hWnd = FindWindow(null, windowName);
      if (hWnd != IntPtr.Zero)
        ShowWindow(hWnd, 0); /// 0 = SW_HIDE
      /// 1 = SW_SHOWNORMA  
      /// Else, do nothing. There is not always a console window.
    }

    #endregion

    #region Private Methods

    private static AppConfig LoadAppConfig()
    {
      try
      {
        if (File.Exists(_appConfigFile))
          return Serializer.Deserialize<AppConfig>(_appConfigFile);
      }
      catch (SerializationException ex)
      {
        CoreBus.Log.Error("Could not load the application configuration.", ex);
      }
      var r = new AppConfig();
      r.LoadDefaults();
      return r;
    }

    private static UserConfig LoadUserConfig()
    {
      try
      {
        if (File.Exists(_appConfigFile))
          return Serializer.Deserialize<UserConfig>(_userConfigFile);
      }
      catch (SerializationException ex)
      {
        CoreBus.Log.Error("Could not load the user configuration.", ex);
      }
      var r = new UserConfig();
      r.LoadDefaults();
      return r;
    }

    #endregion

    #region DLL Imports

    /// <summary>
    /// The FindWindow function retrieves a handle to the top-level window whose class name
    /// and window name match the specified strings. This function does not search child windows.
    /// This function does not perform a case-sensitive search.
    /// </summary>
    /// <param name="lpClassName"></param>
    /// <param name="lpWindowName"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    /// <summary>
    /// The ShowWindow function sets the specified window's show state.
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="nCmdShow"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    #endregion

  }
}
