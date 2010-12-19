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

namespace AppStract.Engine.Data.Connection
{
  /// <summary>
  /// Represents a type of configuration data.
  /// </summary>
  public enum ConfigurationDataType
  {
    /// <summary>
    /// The root directory as used by the file system.
    /// </summary>
    /// <remarks>
    /// The file table only contains relative paths, while the file system should only provide absolute paths.
    /// These relative paths must be combined with the root directory, which results in usable absolute paths.
    /// </remarks>
    FileSystemRoot,
    /// <summary>
    /// The full connection string to the registry database.
    /// </summary>
    RegistryDatabase,
    /// <summary>
    /// The filename of the registry database,
    /// either an absolute path or a path relative to <see cref="FileSystemRoot"/>.
    /// </summary>
    RegistryDatabaseFile,
    /// <summary>
    /// The file to write all log messages to,
    /// either an absolute path or a path relative to <see cref="FileSystemRoot"/>.
    /// </summary>
    LogFile,
    /// <summary>
    /// The full connection string to the database to write all log messages to.
    /// </summary>
    LogDatabase
  }
}
