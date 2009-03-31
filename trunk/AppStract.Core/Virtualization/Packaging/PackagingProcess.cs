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
using AppStract.Core.Virtualization.Process;
using AppStract.Core.Virtualization.Synchronization;

namespace AppStract.Core.Virtualization.Packaging
{
  class PackagingProcess : VirtualizedProcess
  {

    #region Constructors

    private PackagingProcess(VirtualProcessStartInfo startInfo, ProcessSynchronizer synchronizer)
      : base(startInfo, synchronizer)
    {

    }

    #endregion

    #region Public Methods

    public new static PackagingProcess Start(VirtualProcessStartInfo startInfo)
    {
      if (startInfo.DatabaseRegistry.Type != FileType.Database)
        throw new ArgumentException("The destination file specified for the file system database is not valid.", "startInfo");
      if (startInfo.DatabaseRegistry.Type != FileType.Database)
        throw new ArgumentException("The destination file specified for the registry database is not valid.", "startInfo");
      var registryDatabase = RegistryDatabase.CreateDefaultDatabase(startInfo.DatabaseRegistry.File);
      var fileSystemDatabase = WatchingFileSystemDatabase.CreateDefaultDatabase(startInfo.DatabaseFileSystem.File);
      var synchronizer = new ProcessSynchronizer(fileSystemDatabase, startInfo.WorkingDirectory, registryDatabase);
      var process = new PackagingProcess(startInfo, synchronizer);
      return process;
    }

    public IEnumerable<string> GetExecutables()
    {
      if (!HasExited)
        return new List<string>(0);
      var db = _resourceSynchronizer.FileSystemDatabase as WatchingFileSystemDatabase;
      if (db == null) /// Should never occur.
        throw new InvalidCastException("An unexpected exception occured in the application workflow."
                                       + " Expected object of type "
                                       + typeof (WatchingFileSystemDatabase)
                                       + " but received an object of type "
                                       + _resourceSynchronizer.FileSystemDatabase.GetType()
                                       + " Please contact the developers about this issue.");
      var executables = new List<string>();
      foreach (FileTableEntry entry in db.Executables)
        executables.Add(entry.Key); /// BUG? Use entry.Key or entry.Value?
      return executables;
    }

    #endregion

  }
}
