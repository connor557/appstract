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
using System.Collections.Generic;
using AppStract.Core.Data;
using AppStract.Core.Data.FileSystem;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Observables;

namespace AppStract.Core.Synchronization.Implementation
{
  public class ResourceSynchronizer : IFileSystemSynchronizer, IRegistrySynchronizer
  {

    #region Variables

    private readonly FileSystemDatabase _fileSystemDatabase;

    #endregion

    #region Constructors

    public ResourceSynchronizer()
    {
      throw new NotImplementedException();
    }

    public ResourceSynchronizer(FileSystemDatabase fileSystemDatabase)
    {
      _fileSystemDatabase = fileSystemDatabase;
    }

    #endregion

    #region Members of IFileSystemSynchronizer

    public void LoadFileSystemTableTo(ObservableDictionary<string, string> fileTable)
    {
      if (fileTable == null)
        throw new ArgumentNullException("fileTable");
      fileTable.Clear();
      IEnumerable<FileTableEntry> entries = _fileSystemDatabase.ReadAll();
      foreach (FileTableEntry entry in entries)
        fileTable.Add(entry.Key, entry.Value);
      /// Add the events after filling the table.
      fileTable.ItemAdded += FileTable_ItemAdded;
      fileTable.ItemChanged += FileTable_ItemChanged;
      fileTable.ItemRemoved += FileTable_ItemRemoved;
    }

    #endregion

    #region Members of IRegistrySynchronizer

    public void LoadRegistryTo(IDictionary<uint, VirtualRegistryKey> keyList)
    {
      throw new System.NotImplementedException();
    }

    public void SetRegistryKey(VirtualRegistryKey virtualRegistryKey)
    {
      throw new System.NotImplementedException();
    }

    public void SetRegistryKey(VirtualRegistryKey virtualRegistryKey, bool overwriteAllValues)
    {
      throw new System.NotImplementedException();
    }

    public void DeleteRegistryKey(uint keyIndex)
    {
      throw new System.NotImplementedException();
    }

    #endregion

    #region Private Methods

    private void FileTable_ItemAdded(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      _fileSystemDatabase.EnqueueAction(new DatabaseAction<FileTableEntry>(new FileTableEntry(item), DatabaseActionType.Add));
    }

    private void FileTable_ItemChanged(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      _fileSystemDatabase.EnqueueAction(new DatabaseAction<FileTableEntry>(new FileTableEntry(item), DatabaseActionType.Update));
    }

    private void FileTable_ItemRemoved(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      _fileSystemDatabase.EnqueueAction(new DatabaseAction<FileTableEntry>(new FileTableEntry(item), DatabaseActionType.Remove));
    }

    #endregion

  }
}
