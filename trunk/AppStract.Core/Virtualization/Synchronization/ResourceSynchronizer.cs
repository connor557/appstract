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
using AppStract.Core.Data.Application;
using AppStract.Core.Data.FileSystem;
using AppStract.Core.Data.Registry;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Observables;

namespace AppStract.Core.Virtualization.Synchronization
{
  public class ResourceSynchronizer : MarshalByRefObject, IFileSystemSynchronizer, IRegistrySynchronizer, IServerReporter
  {

    #region Variables

    private readonly FileSystemDatabase _fileSystemDatabase;
    private readonly RegistryDatabase _registryDatabase;

    #endregion

    #region Constructors

    public ResourceSynchronizer(ApplicationFile fileSystemDatabaseFile, ApplicationFile registryDatabaseFile)
    {
      throw new NotImplementedException();
    }

    public ResourceSynchronizer(FileSystemDatabase fileSystemDatabase, RegistryDatabase registryDatabase)
    {
      _fileSystemDatabase = fileSystemDatabase;
      _registryDatabase = registryDatabase;
    }

    #endregion

    #region Private Methods

    private void FileTable_ItemAdded(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      _fileSystemDatabase.EnqueueAction(new DatabaseAction<FileTableEntry>(new FileTableEntry(item, FileKind.Unspecified), DatabaseActionType.Set));
    }

    private void FileTable_ItemChanged(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      _fileSystemDatabase.EnqueueAction(new DatabaseAction<FileTableEntry>(new FileTableEntry(item, FileKind.Unspecified), DatabaseActionType.Update));
    }

    private void FileTable_ItemRemoved(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      _fileSystemDatabase.EnqueueAction(new DatabaseAction<FileTableEntry>(new FileTableEntry(item, FileKind.Unspecified), DatabaseActionType.Remove));
    }

    private void Registry_ItemAdded(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item)
    {
      _registryDatabase.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Set));
    }

    private void Registry_ItemChanged(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item)
    {
      _registryDatabase.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Update));
    }

    private void Registry_ItemARemoved(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item)
    {
      _registryDatabase.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Remove));
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

    public void LoadRegistryTo(ObservableDictionary<uint, VirtualRegistryKey> keyList)
    {
      if (keyList == null)
        throw new ArgumentNullException("keyList");
      IEnumerable<VirtualRegistryKey> keys = _registryDatabase.ReadAll();
      foreach (VirtualRegistryKey key in keys)
        keyList.Add(key.Handle, key);
      keyList.ItemAdded += Registry_ItemAdded;
      keyList.ItemChanged += Registry_ItemChanged;
      keyList.ItemRemoved += Registry_ItemARemoved;
    }

    public void SetRegistryKey(VirtualRegistryKey virtualRegistryKey)
    {
      SetRegistryKey(virtualRegistryKey, false);
    }

    public void SetRegistryKey(VirtualRegistryKey virtualRegistryKey, bool overwriteAllValues)
    {
      _registryDatabase.EnqueueAction(new DatabaseAction<VirtualRegistryKey>(virtualRegistryKey,
        overwriteAllValues ? DatabaseActionType.Set : DatabaseActionType.Update));
    }

    public void DeleteRegistryKey(uint keyIndex)
    {
      _registryDatabase.EnqueueAction(
        new DatabaseAction<VirtualRegistryKey>(new VirtualRegistryKey(keyIndex, null),
                                               DatabaseActionType.Remove));
    }

    #endregion

    #region IServerReporter Members

    public void Ping()
    {
      throw new NotImplementedException();
    }

    public void Ping(string message)
    {
      throw new NotImplementedException();
    }

    public void ReportException(Exception exception)
    {
      throw new NotImplementedException();
    }

    public void ReportException(Exception exception, string message)
    {
      throw new NotImplementedException();
    }

    public void ReportMessage(string message)
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
