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
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Registry;

namespace AppStract.Core.Virtualization.Synchronization
{
  /// <summary>
  /// Provides a way of data synchronization between multiple processes.
  /// </summary>
  public class ProcessSynchronizer : MarshalByRefObject, IServerReporter, ISynchronizer, IResourceLoader
  {

    #region Variables

    private readonly FileSystemDatabase _fileSystemDatabase;
    private readonly RegistryDatabase _registryDatabase;

    #endregion

    #region Properties

    public FileSystemDatabase FileSystemDatabase
    {
      get { return _fileSystemDatabase; }
    }

    public RegistryDatabase RegistryDatabase
    {
      get { return _registryDatabase; }
    }

    #endregion

    #region Constructors

    public ProcessSynchronizer(ApplicationFile fileSystemDatabaseFile, ApplicationFile registryDatabaseFile)
    {
      if (fileSystemDatabaseFile.Type != FileType.Database)
        throw new ArgumentException("The filename specified for the file system database is not valid.", "fileSystemDatabaseFile");
      if (registryDatabaseFile.Type != FileType.Database)
        throw new ArgumentException("The filename specified for the registry database is not valid.", "registryDatabaseFile");
      _fileSystemDatabase = FileSystemDatabase.CreateDefaultDatabase(fileSystemDatabaseFile.File);
      _registryDatabase = RegistryDatabase.CreateDefaultDatabase(registryDatabaseFile.File);
    }

    public ProcessSynchronizer(FileSystemDatabase fileSystemDatabase, RegistryDatabase registryDatabase)
    {
      _fileSystemDatabase = fileSystemDatabase;
      _registryDatabase = registryDatabase;
    }

    #endregion

    #region IServerReporter Members

    public void Ping()
    {
      /// No action required.
    }

    public void ReportException(Exception exception)
    {
      CoreBus.Log.Error("", exception);
    }

    public void ReportException(Exception exception, string message)
    {
      CoreBus.Log.Error(message, exception);
    }

    public void ReportMessage(string message)
    {
      CoreBus.Log.Message(message);
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
