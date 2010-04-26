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
using AppStract.Core.Data.Application;
using AppStract.Core.Data.Databases;
using AppStract.Core.System.Logging;
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Core.Virtualization.Engine.Registry;

namespace AppStract.Core.System.IPC
{
  /// <summary>
  /// Provides a way of data synchronization between multiple processes.
  /// </summary>
  internal sealed class ProcessSynchronizer : MarshalByRefObject, IProcessSynchronizer
  {

    #region Variables

    /// <summary>
    /// The root as used by the file system.
    /// </summary>
    private readonly string _fileSystemRoot;
    /// <summary>
    /// The <see cref="FileSystemDatabase"/> used by the current instance.
    /// </summary>
    private readonly FileSystemDatabase _fileSystemDatabase;
    /// <summary>
    /// The <see cref="RegistryDatabase"/> used by the current instance.
    /// </summary>
    private readonly RegistryDatabase _registryDatabase;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the <see cref="FileSystemDatabase"/> used by the current instance.
    /// </summary>
    public FileSystemDatabase FileSystemDatabase
    {
      get { return _fileSystemDatabase; }
    }

    /// <summary>
    /// Gets the <see cref="RegistryDatabase"/> used by the current instance.
    /// </summary>
    public RegistryDatabase RegistryDatabase
    {
      get { return _registryDatabase; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="ProcessSynchronizer"/>.
    /// The constructor will create default databases from the specified files.
    /// </summary>
    /// <param name="fileSystemDatabaseFile">The file to use with a default <see cref="FileSystemDatabase"/>.</param>
    /// <param name="fileSystemRoot">The directory to use as root of the file system.</param>
    /// <param name="registryDatabaseFile">The file to use with a default <see cref="RegistryDatabase"/>.</param>
    public ProcessSynchronizer(ApplicationFile fileSystemDatabaseFile, ApplicationFile fileSystemRoot, ApplicationFile registryDatabaseFile)
    {
      if (fileSystemDatabaseFile.Type != FileType.Database)
        throw new ArgumentException("The filename specified for the file system database is not valid.", "fileSystemDatabaseFile");
      if (fileSystemRoot.Type != FileType.Directory)
        throw new ArgumentException("The root location specified for the file system is not valid.", "fileSystemRoot");
      if (registryDatabaseFile.Type != FileType.Database)
        throw new ArgumentException("The filename specified for the registry database is not valid.", "registryDatabaseFile");
      _fileSystemDatabase = FileSystemDatabase.CreateDefaultDatabase(fileSystemDatabaseFile.FileName);
      _registryDatabase = RegistryDatabase.CreateDefaultDatabase(registryDatabaseFile.FileName);
      _fileSystemDatabase.Initialize();
      _registryDatabase.Initialize();
      _fileSystemRoot = fileSystemRoot.FileName;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ProcessSynchronizer"/>.
    /// </summary>
    /// <param name="fileSystemDatabase">The <see cref="FileSystemDatabase"/> to send the incomming <see cref="DatabaseAction{T}"/>s to.</param>
    /// <param name="fileSystemRoot">The directory to use as root of the file system.</param>
    /// <param name="registryDatabase">The <see cref="RegistryDatabase"/> to send the incomming <see cref="DatabaseAction{T}"/>s to.</param>
    public ProcessSynchronizer(FileSystemDatabase fileSystemDatabase, ApplicationFile fileSystemRoot, RegistryDatabase registryDatabase)
    {
      if (fileSystemRoot.Type != FileType.Directory)
        throw new ArgumentException("The root location specified for the file system is not valid.", "fileSystemRoot");
      _fileSystemDatabase = fileSystemDatabase;
      _registryDatabase = registryDatabase;
      _fileSystemRoot = fileSystemRoot.FileName;
    }

    #endregion

    #region IServerReporter Members

    public void Ping()
    {
      /// No action required.
    }

    public void ReportMessage(LogMessage message)
    {
      CoreBus.Log.Log(message);
    }

    public void ReportMessage(IEnumerable<LogMessage> messages)
    {
      foreach (var message in messages)
        CoreBus.Log.Log(message);
    }

    public LogLevel GetRequiredLogLevel()
    {
      return CoreBus.Log.LogLevel;
    }

    #endregion

    #region ISynchronizer Members

    public void SyncFileSystemActions(IEnumerable<DatabaseAction<FileTableEntry>> actions)
    {
      _fileSystemDatabase.EnqueueAction(actions);
    }

    public void SyncRegistryActions(IEnumerable<DatabaseAction<VirtualRegistryKey>> actions)
    {
      _registryDatabase.EnqueueAction(actions);
    }

    #endregion

    #region IResourceLoader Members

    public string FileSystemRoot
    {
      get { return _fileSystemRoot; }
    }

    public IEnumerable<FileTableEntry> LoadFileSystemTable()
    {
      var entries = _fileSystemDatabase.ReadAll();
      return entries;
    }

    public IEnumerable<VirtualRegistryKey> LoadRegistry()
    {
      var keys = _registryDatabase.ReadAll();
      return keys;
    }

    #endregion

  }
}
