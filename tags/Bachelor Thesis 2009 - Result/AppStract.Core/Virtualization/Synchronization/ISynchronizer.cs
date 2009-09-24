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

using System.Collections.Generic;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Registry;

namespace AppStract.Core.Virtualization.Synchronization
{
  /// <summary>
  /// Interface for interprocess data synchronization.
  /// </summary>
  public interface ISynchronizer
  {

    /// <summary>
    /// Commits the specified <see cref="DatabaseAction{T}"/>s to the file system database.
    /// </summary>
    /// <param name="actions">Actions to commit.</param>
    void SyncFileSystemActions(IEnumerable<DatabaseAction<FileTableEntry>> actions);

    /// <summary>
    /// Commits the specified <see cref="DatabaseAction{T}"/>s to the registry database.
    /// </summary>
    /// <param name="actions">Actions to commit.</param>
    void SyncRegistryActions(IEnumerable<DatabaseAction<VirtualRegistryKey>> actions);

  }
}
