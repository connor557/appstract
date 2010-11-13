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
using AppStract.Host.Data.Application;
using AppStract.Engine.Configuration;
using AppStract.Engine.Data.Connection;
using AppStract.Engine.Data.Databases;
using AppStract.Engine.Virtualization.Registry;
using AppStract.Utilities.Logging;

namespace AppStract.Host.System.IPC
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
    /// The collection of engine rules to apply on the file system virtualization engine.
    /// </summary>
    private readonly FileSystemRuleCollection _fsRuleCollection;
    /// <summary>
    /// The <see cref="RegistryDatabase"/> used by the current instance.
    /// </summary>
    private readonly RegistryDatabase _registryDatabase;
    /// <summary>
    /// The collection of engine rules to apply on the registry virtualization engine.
    /// </summary>
    private readonly RegistryRuleCollection _regRuleCollection;

    #endregion

    #region Properties

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
    /// <param name="fileSystemRoot">The directory to use as root of the file system.</param>
    /// <param name="fileSystemRuleCollection">The collection of engine rules to apply on the file system virtualization engine.</param>
    /// <param name="registryDatabaseFile">The file to use with a default <see cref="RegistryDatabase"/>.</param>
    /// <param name="registryRuleCollection">The collection of engine rules to apply on the registry virtualization engine.</param>
    public ProcessSynchronizer(ApplicationFile fileSystemRoot, FileSystemRuleCollection fileSystemRuleCollection,
      ApplicationFile registryDatabaseFile, RegistryRuleCollection registryRuleCollection)
    {
      if (fileSystemRoot.Type != FileType.Directory)
        throw new ArgumentException("The root location specified for the file system is not valid.", "fileSystemRoot");
      if (registryDatabaseFile.Type != FileType.Database)
        throw new ArgumentException("The filename specified for the registry database is not valid.", "registryDatabaseFile");
      _registryDatabase = RegistryDatabase.CreateDefaultDatabase(registryDatabaseFile.FileName);
      _registryDatabase.Initialize();
      _fileSystemRoot = fileSystemRoot.FileName;
      _fsRuleCollection = fileSystemRuleCollection;
      _regRuleCollection = registryRuleCollection;
    }

    #endregion

    #region IServerReporter Members

    public void Ping()
    {
      // No action required.
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

    public FileSystemRuleCollection GetFileSystemEngineRules()
    {
      return _fsRuleCollection;
    }

    public RegistryRuleCollection GetRegistryEngineRules()
    {
      return _regRuleCollection;
    }

    public IEnumerable<VirtualRegistryKey> LoadRegistry()
    {
      var keys = _registryDatabase.ReadAll();
      return keys;
    }

    #endregion

  }
}
