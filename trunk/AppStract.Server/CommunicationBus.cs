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
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Registry;
using AppStract.Core.Virtualization.Synchronization;
using AppStract.Utilities.Observables;

namespace AppStract.Server
{
  /// <summary>
  /// Enqueues <see cref="DatabaseAction{T}"/>s 'till <see cref="Flush"/> is called.
  /// Provides an optimized interface for IPC with the <see cref="ProcessSynchronizer"/>.
  /// </summary>
  /// <remarks>
  /// Actions are enqueued for a maximum of 500ms.
  /// If the <see cref="CommunicationBus"/> detects that the process is queried to shut down,
  /// the queues are flushed to the <see cref="ProcessSynchronizer"/> of the host process.
  /// 
  /// Realize that in 500ms much actions can be enqueued,
  /// and all these actions will be lost if the process would be killed by the Windows task manager.
  /// </remarks>
  public class CommunicationBus : IFileSystemLoader, IRegistryLoader
  {

    #region Variables

    /// <summary>
    /// The <see cref="IResourceLoader"/>to use for loading the resources.
    /// </summary>
    private readonly IResourceLoader _loader;
    /// <summary>
    /// The <see cref="ISynchronizer"/>to use for synchronization
    /// between the current guest process and the host process.
    /// </summary>
    private readonly ISynchronizer _synchronizer;
    /// <summary>
    /// The <see cref="Queue{T}"/> containing all waiting <see cref="DatabaseAction{T}"/>s
    /// to send  to the file system database.
    /// </summary>
    private readonly Queue<DatabaseAction<FileTableEntry>> _fileSystemQueue;
    /// <summary>
    /// The <see cref="Queue{T}"/> containing all waiting <see cref="DatabaseAction{T}"/>s
    /// to send  to the registry database.
    /// </summary>
    private readonly Queue<DatabaseAction<VirtualRegistryKey>> _registryQueue;
    /// <summary>
    /// The object to lock when performing actions on <see cref="_fileSystemQueue"/>.
    /// </summary>
    private readonly object _fileSystemLock;
    /// <summary>
    /// The object to lock when performing actions on <see cref="_registryQueue"/>.
    /// </summary>
    private readonly object _registryLock;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of <see cref="CommunicationBus"/>.
    /// </summary>
    /// <param name="resourceSynchronizer">
    /// The <see cref="ISynchronizer"/> to use for synchronization
    /// between the current guest process and the host process.
    /// </param>
    /// <param name="resourceLoader">
    /// The <see cref="IResourceLoader"/> to use for loading the resources.
    /// </param>
    public CommunicationBus(ISynchronizer resourceSynchronizer, IResourceLoader resourceLoader)
    {
      _synchronizer = resourceSynchronizer;
      _loader = resourceLoader;
      _fileSystemQueue = new Queue<DatabaseAction<FileTableEntry>>();
      _registryQueue = new Queue<DatabaseAction<VirtualRegistryKey>>();
      _fileSystemLock = new object();
      _registryLock = new object();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Flushes all enqueued items to the <see cref="ProcessSynchronizer"/> specified during initialization.
    /// </summary>
    public void Flush()
    {
      IEnumerable<DatabaseAction<FileTableEntry>> fsActions;
      IEnumerable<DatabaseAction<VirtualRegistryKey>> regActions;
      lock (_fileSystemLock)
      {
        fsActions = _fileSystemQueue.ToArray();
        _fileSystemQueue.Clear();
      }
      lock (_registryLock)
      {
        regActions = _registryQueue.ToArray();
        _registryQueue.Clear();
      }
      _synchronizer.SyncFileSystemActions(fsActions);
      _synchronizer.SyncRegistryActions(regActions);
    }

    #endregion

    #region Private Methods

    private void FileTable_ItemAdded(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      lock (_fileSystemLock)
        _fileSystemQueue.Enqueue(new DatabaseAction<FileTableEntry>(new FileTableEntry(item, FileKind.Unspecified), DatabaseActionType.Set));
    }

    private void FileTable_ItemChanged(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      lock (_fileSystemLock)
        _fileSystemQueue.Enqueue(new DatabaseAction<FileTableEntry>(new FileTableEntry(item, FileKind.Unspecified), DatabaseActionType.Update));
    }

    private void FileTable_ItemRemoved(ICollection<KeyValuePair<string, string>> sender, KeyValuePair<string, string> item)
    {
      lock (_fileSystemLock)
        _fileSystemQueue.Enqueue(new DatabaseAction<FileTableEntry>(new FileTableEntry(item, FileKind.Unspecified), DatabaseActionType.Remove));
    }

    private void Registry_ItemAdded(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item)
    {
      lock (_registryLock)
        _registryQueue.Enqueue(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Set));
    }

    private void Registry_ItemChanged(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item)
    {
      lock (_registryLock)
        _registryQueue.Enqueue(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Update));
    }

    private void Registry_ItemRemoved(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item)
    {
      lock (_registryLock)
        _registryQueue.Enqueue(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Remove));
    }

    #endregion

    #region IFileSystemLoader Members

    public void LoadFileSystemTableTo(ObservableDictionary<string, string> fileTable)
    {
      if (fileTable == null)
        throw new ArgumentNullException("fileTable");
      fileTable.Clear();
      var files = _loader.LoadFileSystemTable();
      foreach (var file in files)
        fileTable.Add(file.Key, file.Value);
      fileTable.ItemAdded += FileTable_ItemAdded;
      fileTable.ItemChanged += FileTable_ItemChanged;
      fileTable.ItemRemoved += FileTable_ItemRemoved;
    }

    #endregion

    #region IRegistryLoader Members

    public void LoadRegistryTo(ObservableDictionary<uint, VirtualRegistryKey> keyList)
    {
      if (keyList == null)
        throw new ArgumentNullException("keyList");
      keyList.Clear();
      var keys = _loader.LoadRegistry();
      foreach (var key in keys)
        keyList.Add(key.Handle, key);
      keyList.ItemAdded += Registry_ItemAdded;
      keyList.ItemChanged += Registry_ItemChanged;
      keyList.ItemRemoved += Registry_ItemRemoved;
    }

    #endregion

  }
}
