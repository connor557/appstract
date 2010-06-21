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

namespace AppStract.Core.Virtualization.Engine.FileSystem
{
  /// <summary>
  /// An action to take on a file or device that exists or does not exist. 
  /// For devices other than files, this parameter is usually set to OPEN_EXISTING.
  /// </summary>
  public enum FileCreationDisposition
  {
    /// <summary>
    /// The required action is not specified.
    /// </summary>
    UNSPECIFIED = 0,
    /// <summary>
    /// Creates a new file, only if it does not already exist.
    /// If the specified file exists, the function fails.
    /// If the specified file does not exist and is a valid path to a writable location, a new file is created.
    /// </summary>
    CREATE_NEW = 1,
    /// <summary>
    /// Creates a new file, always.
    /// If the specified file exists and is writable, the function overwrites the file.
    /// If the specified file does not exist and is a valid path, a new file is created..
    /// </summary>
    CREATE_ALWAYS = 2,
    /// <summary>
    /// Opens a file or device, only if it exists.
    /// If the specified file or device does not exist, the function fails.
    /// </summary>
    OPEN_EXISTING = 3,
    /// <summary>
    /// Opens a file, always.
    /// If the specified file exists, the function succeeds.
    /// If the specified file does not exist and is a valid path to a writable location, the function creates a file.
    /// </summary>
    OPEN_ALWAYS = 4,
    /// <summary>
    /// Opens a file and truncates it so that its size is zero bytes, only if it exists.
    /// If the specified file does not exist, the function fails.
    /// The calling process must open the file with the GENERIC_WRITE bit set as part of the dwDesiredAccess parameter.
    /// </summary>
    TRUNCATE_EXISTING = 5
  }
}
