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

namespace AppStract.Engine.Data.Connection
{
  /// <summary>
  /// Represents errors caused by any configuration data of the engine connection.
  /// </summary>
  [Serializable]
  public class ConfigurationDataException : EngineException
  {
    /// <summary>
    /// Gets the data causing the exception.
    /// </summary>
    public ConfigurationDataType ConfigurationDataType
    {
      get; private set;
    }

    public ConfigurationDataException(ConfigurationDataType resourceType)
    {
      ConfigurationDataType = resourceType;
    }

    public ConfigurationDataException(ConfigurationDataType resourceType, string message)
      : base(message)
    {
      ConfigurationDataType = resourceType;
    }

    public ConfigurationDataException(ConfigurationDataType resourceType, string message, Exception innerException)
      : base(message, innerException)
    {
      ConfigurationDataType = resourceType;
    }

  }
}
