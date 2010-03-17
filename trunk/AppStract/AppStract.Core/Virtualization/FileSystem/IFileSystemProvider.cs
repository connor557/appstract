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

namespace AppStract.Core.Virtualization.FileSystem
{
  /// <summary>
  /// Provides access to the virtual file system.
  /// </summary>
  public interface IFileSystemProvider
  {

    /// <summary>
    /// Returns a <see cref="FileTableEntry"/> for the specified <see cref="FileRequest"/>.
    /// If the file doesn't exist yet, it will be added to the filetable without creating the file on disc.
    /// </summary>
    /// <param name="fileRequest"></param>
    /// <returns></returns>
    FileTableEntry GetFile(FileRequest fileRequest);

    /// <summary>
    /// Deletes the <see cref="FileTableEntry"/> specified from the file table.
    /// </summary>
    /// <param name="fileTableEntry">The entry to delete from the file table.</param>
    void DeleteFile(FileTableEntry fileTableEntry);

  }
}
