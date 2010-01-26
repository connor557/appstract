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
using System.Runtime.Serialization;
using System.Xml.Serialization;
using AppStract.Utilities.Helpers;

namespace AppStract.Core.Data.Application
{
  [Serializable]
  public sealed class ApplicationData : ISerializable
  {

    #region Variables

    private ApplicationSettings _settings;
    private ApplicationFiles _files;

    #endregion

    #region Properties

    [XmlElement]
    public ApplicationSettings Settings
    {
      get { return _settings; }
      // Setter should be removed without making serialization to fail.
      // When this setter is used, other code might be using a cached reference to the old _settings.
      set { _settings = value; }
    }

    [XmlElement]
    public ApplicationFiles Files
    {
      get { return _files; }
      // Setter should be removed without making serialization to fail.
      // When this setter is used, other code might be using a cached reference to the old _files.
      set { _files = value; }
    }

    #endregion

    #region Constructors

    public ApplicationData()
    {
      _settings = new ApplicationSettings();
      _files = new ApplicationFiles();
    }

    public ApplicationData(SerializationInfo info, StreamingContext context)
    {
      _settings = (ApplicationSettings)info.GetValue("Settings", typeof(ApplicationSettings));
      _files = (ApplicationFiles)info.GetValue("Files", typeof(ApplicationFiles));
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Saves an instance of <see cref="ApplicationData"/> to the specified <paramref name="filename"/>.
    /// </summary>
    /// <param name="applicationData">The data to save.</param>
    /// <param name="filename">File containing the data to deserialize.</param>
    /// <returns>True if the data is successfully save; otherwise, false.</returns>
    public static bool Save(ApplicationData applicationData, string filename)
    {
      try
      {
        XmlSerializationHelper.Serialize(filename, applicationData);
        return true;
      }
      catch (Exception e)
      {
        CoreBus.Log.Warning("Failed to save instance of ApplicationData to " + filename, e);
        return false;
      }
    }

    /// <summary>
    /// Initializes and returns an instance of <see cref="ApplicationData"/> from the specified <paramref name="filename"/>.
    /// Returns null if the loading failed.
    /// </summary>
    /// <param name="filename">File containing the data to deserialize.</param>
    /// <returns>The <see cref="ApplicationData"/>, or null if deserialization failed.</returns>
    public static ApplicationData Load(string filename)
    {
      try
      {
        return XmlSerializationHelper.Deserialize<ApplicationData>(filename);
      }
      catch (Exception e)
      {
        CoreBus.Log.Warning("Failed to load an instance of ApplicationData from " + filename, e);
        return null;
      }
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("Settings", _settings, typeof(ApplicationSettings));
      info.AddValue("Files", _files, typeof(ApplicationFiles));
    }

    #endregion

  }
}
