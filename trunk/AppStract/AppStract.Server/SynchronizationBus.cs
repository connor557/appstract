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
using System.Collections.Generic;
using System.Threading;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Core.Virtualization.Engine.Registry;
using AppStract.Core.System.IPC;
using AppStract.Utilities.Observables;

namespace AppStract.Server
{
  /// <summary>
  /// Synchronizes queuries with the host process.
  /// </summary>
  /// <remarks>
  /// <see cref="DatabaseAction{T}"/>s are enqueued until <see cref="Flush"/> is called, which flushes them as a single batch to the host process.
  /// When <see cref="AutoFlush"/> is set to true, queries are flushed every time <see cref="FlushInterval"/> passes.
  /// <br />
  /// If the <see cref="SynchronizationBus"/> detects that the process is queried to shut down,
  /// the queues are automatically flushed to the <see cref="ProcessSynchronizer"/> of the host process.
  /// </remarks>
  internal sealed class SynchronizationBus : IFileSystemSynchronizer, IRegistrySynchronizer
  {

    #region Variables

    /// <summary>
    /// The <see cref="IResourceLoader"/> to use for loading the resources.
    /// </summary>
    private readonly IResourceLoader _loader;
    /// <summary>
    /// The <see cref="ISynchronizer"/> to use for synchronization
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
    private readonly object _fileSystemSyncObject;
    /// <summary>
    /// The object to lock when performing actions on <see cref="_registryQueue"/>.
    /// </summary>
    private readonly object _registrySyncObject;
    /// <summary>
    /// The object to lock when performing actions
    /// related to <see cref="_flushInterval"/> and/or <see cref="_autoFlush"/>.
    /// </summary>
    private readonly object _flushSyncObject;
    /// <summary>
    /// The interval between each call to <see cref="Flush"/>,
    /// in milliseconds.
    /// </summary>
    private int _flushInterval;
    /// <summary>
    /// Whether <see cref="_fileSystemQueue"/> and <see cref="_registryQueue"/>
    /// must be automatically flushed.
    /// </summary>
    private bool _autoFlush;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets whether the enqueued <see cref="DatabaseAction{T}"/>s must be automatically flushed
    /// each time <see cref="FlushInterval"/> has passed.
    /// Default value is false.
    /// </summary>
    public bool AutoFlush
    {
      get { return _autoFlush; }
      set
      {
        lock (_flushSyncObject)
        {
          if (_autoFlush == value)
            return;
          _autoFlush = value;
          if (_autoFlush)
            new Thread(StartFlushing) { IsBackground = true, Name = "CommBus" }.Start();
        }
      }
    }

    /// <summary>
    /// Gets or sets the interval between each call to <see cref="Flush"/>, in milliseconds.
    /// </summary>
    /// <remarks>
    /// The default interval is 500 milliseconds.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// An <see cref="ArgumentOutOfRangeException"/> is thrown if the interval specified
    /// is not equal to or greater than 0.
    /// </exception>
    public int FlushInterval
    {
      get { return _flushInterval; }
      set
      {
        if (value < 0)
          throw new ArgumentOutOfRangeException("value", "The FlushInterval specified must be greater than -1.");
        _flushInterval = value;
      }
    }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of <see cref="SynchronizationBus"/>.
    /// </summary>
    /// <param name="resourceSynchronizer">
    /// The <see cref="ISynchronizer"/> to use for synchronization
    /// between the current guest process and the host process.
    /// </param>
    /// <param name="resourceLoader">
    /// The <see cref="IResourceLoader"/> to use for loading the resources.
    /// </param>
    public SynchronizationBus(ISynchronizer resourceSynchronizer, IResourceLoader resourceLoader)
    {
      _synchronizer = resourceSynchronizer;
      _loader = resourceLoader;
      _fileSystemQueue = new Queue<DatabaseAction<FileTableEntry>>();
      _registryQueue = new Queue<DatabaseAction<VirtualRegistryKey>>();
      _autoFlush = false;
      _flushInterval = 500;
      _fileSystemSyncObject = new object();
      _registrySyncObject = new object();
      _flushSyncObject = new object();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Flushes all enqueued items to the <see cref="ProcessSynchronizer"/> 
    /// attached to the current <see cref="SynchronizationBus"/> instance.
    /// </summary>
    public void Flush()
    {
      DatabaseAction<FileTableEntry>[] fsActions;
      DatabaseAction<VirtualRegistryKey>[] regActions;
      lock (_fileSystemSyncObject)
      {
        fsActions = _fileSystemQueue.ToArray();
        _fileSystemQueue.Clear();
      }
      lock (_registrySyncObject)
      {
        regActions = _registryQueue.ToArray();
        _registryQueue.Clear();
      }
      using (Hooking.HookManager.ACL.GetHookingExclusion())
      {
        if (fsActions.Length > 0)
          _synchronizer.SyncFileSystemActions(fsActions);
        if (regActions.Length > 0)
          _synchronizer.SyncRegistryActions(regActions);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Starts flushing.
    /// This method doesn't return unless <see cref="_autoFlush"/> is set to false.
    /// </summary>
    private void StartFlushing()
    {
      while (true)
      {
        int flushInterval;
        lock (_flushSyncObject)
        {
          if (!_autoFlush)
            return;
          Flush();
          if (!_autoFlush)
            return;
          flushInterval = _flushInterval;
        }
        Thread.Sleep(flushInterval);
      }
    }

    /// <summary>
    /// Eventhandler for the ItemAdded event of the file table.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="item"></param>
    /// <param name="args"></param>
    private void FileTable_ItemAdded(ICollection<KeyValuePair<string, FileTableEntry>> sender, KeyValuePair<string, FileTableEntry> item, EventArgs args)
    {
      lock (_fileSystemSyncObject)
        _fileSystemQueue.Enqueue(new DatabaseAction<FileTableEntry>(item.Value, DatabaseActionType.Set));
    }

    /// <summary>
    /// Eventhandler for the ItemChanged event of the file table.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="item"></param>
    /// <param name="args"></param>
    private void FileTable_ItemChanged(ICollection<KeyValuePair<string, FileTableEntry>> sender, KeyValuePair<string, FileTableEntry> item, EventArgs args)
    {
      lock (_fileSystemSyncObject)
        _fileSystemQueue.Enqueue(new DatabaseAction<FileTableEntry>(item.Value, DatabaseActionType.Set));
    }

    /// <summary>
    /// Eventhandler for the ItemRemoved event of the file table.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="item"></param>
    /// <param name="args"></param>
    private void FileTable_ItemRemoved(ICollection<KeyValuePair<string, FileTableEntry>> sender, KeyValuePair<string, FileTableEntry> item, EventArgs args)
    {
      lock (_fileSystemSyncObject)
        _fileSystemQueue.Enqueue(new DatabaseAction<FileTableEntry>(item.Value, DatabaseActionType.Remove));
    }

    /// <summary>
    /// Eventhandler for the ItemAdded event of the registry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="item"></param>
    /// <param name="args"></param>
    private void Registry_ItemAdded(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item, EventArgs args)
    {
      lock (_registrySyncObject)
        _registryQueue.Enqueue(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Set));
    }

    /// <summary>
    /// Eventhandler for the ItemChanged event of the registry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="item"></param>
    /// <param name="args"></param>
    private void Registry_ItemChanged(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item, EventArgs args)
    {
      lock (_registrySyncObject)
        _registryQueue.Enqueue(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Set));
    }

    /// <summary>
    /// Eventhandler for the ItemRemoved event of the registry.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="item"></param>
    /// <param name="args"></param>
    private void Registry_ItemRemoved(ICollection<KeyValuePair<uint, VirtualRegistryKey>> sender, KeyValuePair<uint, VirtualRegistryKey> item, EventArgs args)
    {
      lock (_registrySyncObject)
        _registryQueue.Enqueue(new DatabaseAction<VirtualRegistryKey>(item.Value, DatabaseActionType.Remove));
    }

    #endregion

    #region IFileSystemSynchronizer Members

    public void SynchronizeFileSystemTableWith(ObservableDictionary<string, FileTableEntry> fileTable)
    {
      if (fileTable == null)
        throw new ArgumentNullException("fileTable");
      fileTable.Clear();
      IEnumerable<FileTableEntry> files;
      using (Hooking.HookManager.ACL.GetHookingExclusion())
        files = _loader.LoadFileSystemTable();
      foreach (var file in files)
        fileTable.Add(file.Key, file);
      fileTable.ItemAdded += FileTable_ItemAdded;
      fileTable.ItemChanged += FileTable_ItemChanged;
      fileTable.ItemRemoved += FileTable_ItemRemoved;
    }

    #endregion

    #region IRegistrySynchronizer Members

    public RegistryRuleCollection GetEngineRules()
    {
      using (Hooking.HookManager.ACL.GetHookingExclusion())
        return _loader.GetRegistryEngineRules();
    }

    public void SynchronizeRegistryWith(ObservableDictionary<uint, VirtualRegistryKey> keyList)
    {
      if (keyList == null)
        throw new ArgumentNullException("keyList");
      keyList.Clear();
      IEnumerable<VirtualRegistryKey> keys;
      using (Hooking.HookManager.ACL.GetHookingExclusion())
        keys = _loader.LoadRegistry();
      foreach (var key in keys)
        keyList.Add(key.Handle, key);
      keyList.ItemAdded += Registry_ItemAdded;
      keyList.ItemChanged += Registry_ItemChanged;
      keyList.ItemRemoved += Registry_ItemRemoved;
    }

    #endregion

  }
}
