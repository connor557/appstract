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

using System.Collections.Generic;
using AppStract.Engine.Configuration;

namespace AppStract.Engine.Data.Connection
{
  /// <summary>
  /// Provides the configuration data needed by the virtualization engine in order to be initializable.
  /// </summary>
  public interface IConfigurationProvider
  {

    /// <summary>
    /// Gets the collection of connection strings associated to the available <see cref="DataResourceType"/> members.
    /// </summary>
    IDictionary<DataResourceType, string> ConnectionStrings
    {
      get;
    }

    /// <summary>
    /// Gets the engine rules to apply on the file system virtualization engine.
    /// </summary>
    /// <returns></returns>
    FileSystemRuleCollection GetFileSystemEngineRules();

    /// <summary>
    /// Gets the engine rules to apply on the registry virtualization engine.
    /// </summary>
    /// <returns></returns>
    RegistryRuleCollection GetRegistryEngineRules();

  }
}
