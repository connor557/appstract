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
using System.IO;
using System.Threading;
using AppStract.Core.Logging;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Utilities.Observables;

namespace AppStract.Server.FileSystem
{
  /// <summary>
  /// The default implementation of <see cref="IFileSystemProvider"/>.
  /// </summary>
  public class FileSystemProvider : IFileSystemProvider
  {

    #region Variables

    /// <summary>
    /// The file table of the current <see cref="FileSystemProvider"/>.
    /// The keys are the paths of the real file system,
    /// the values are the replacement paths relative to <see cref="_root"/>.
    /// </summary>
    private readonly ObservableDictionary<string, string> _fileTable;
    /// <summary>
    /// Manages the synchronization between multiple threads accessing the file table.
    /// </summary>
    private readonly ReaderWriterLockSlim _fileTableLock;
    /// <summary>
    /// The root of the filesystem,
    /// which is a path to a directory in the real file system.
    /// </summary>
    private readonly string _root;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the fully qualified path of the working directory.
    /// </summary>
    public string CurrentDirectory
    {
      get { return _root; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="FileSystemProvider"/>,
    /// using the <paramref name="rootDirectory"/> specified as root.
    /// </summary>
    /// <param name="rootDirectory">The path to use as root directory.</param>
    public FileSystemProvider(string rootDirectory)
    {
      if (rootDirectory == null)
        throw new ArgumentNullException("rootDirectory");
      _fileTable = new ObservableDictionary<string, string>();
      _fileTableLock = new ReaderWriterLockSlim();
      _root = !Path.IsPathRooted(rootDirectory)
                ? Path.GetFullPath(rootDirectory)
                : rootDirectory;
      VirtualEnvironment.CreateVirtualFolders(_root);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Loads the underlying filetable of the current <see cref="FileSystemProvider"/>.
    /// </summary>
    public void LoadFileTable(IFileSystemLoader dataSource)
    {
      _fileTableLock.EnterWriteLock();
      try
      {
        dataSource.LoadFileSystemTableTo(_fileTable);
      }
      finally
      {
        _fileTableLock.ExitWriteLock();
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tries to get the value linked to the specified <paramref name="key"/> from the file table.
    /// </summary>
    /// <param name="key">Key to get value for.</param>
    /// <param name="value">The value for the key, null if the key doesn't have an entry.</param>
    /// <returns>True if the key is found, false otherwise.</returns>
    private bool TryGetFile(string key, out string value)
    {
      value = null;
      _fileTableLock.EnterReadLock();
      try
      {
        if (_fileTable.ContainsKey(key))
          value = _fileTable[key];
      }
      finally
      {
        _fileTableLock.ExitReadLock();
      }
      return value != null;
    }

    /// <summary>
    /// Returns the full path to the specified library.
    /// </summary>
    /// <param name="libraryPath">Library to search for.</param>
    /// <returns>Full path to the specified library.</returns>
    private string TryFindLibrary(string libraryPath)
    {
      string result;
      /// Check the file table.
      if (TryGetFile(libraryPath, out result))
        return result;
      /// Still not found? Redirect the request and see if then the library can be found.
      string redirectedPath = FileAccessRedirector.Redirect(libraryPath);
      if (File.Exists(redirectedPath))
        /// The file exists in the virtual file system, return it.
        return redirectedPath;
      /// Still not found?
      /// We're sure the virtual folders don't contain the dll (because of TryGetFile and the redirection).
      /// NOTE: Are we sure? Need to debug this!
      /// Return the parameter and let Windows handle the search.
      return libraryPath;
    }

    /// <summary>
    /// Adds a new file to the file table for the given key.
    /// </summary>
    /// <param name="fileRequest">The <see cref="FileRequest"/> to base the new <see cref="FileTableEntry"/> on.</param>
    /// <returns>The created <see cref="FileTableEntry"/>.</returns>
    private FileTableEntry AddNewEntryToFileTable(FileRequest fileRequest)
    {
      string fileEntryValue = FileAccessRedirector.Redirect(fileRequest.FullName);
      /// Verify if fileEntryValue is a unique filename in the virtual environment.
      string fullVirtualPath = Path.Combine(_root, fileEntryValue);
      int cnt = 0;
      while (File.Exists(fullVirtualPath))
      {
        string filename = Path.GetFileName(fullVirtualPath);
        string newFilename = string.Format("{0}{1}{2}",
                                           Path.GetFileNameWithoutExtension(filename),
                                           cnt++,
                                           Path.GetExtension(filename));
        fileEntryValue = fileEntryValue.Replace(filename, newFilename);
        fullVirtualPath = Path.Combine(_root, fileEntryValue);
      }
      /// The value is now (probably?) unique, write it to the file table.
      /// Why probably?
      ///   A concurrent thread might use the same value,
      ///   but the chance that this happens is almost zero
      ///   and is not worth the overhead of adding a lock statement.
      return WriteEntryToTable(fileRequest.Name, fileEntryValue, false);
    }

    /// <summary>
    /// Returns the full path of a unique, zero-filled, temporary file.
    /// </summary>
    /// <param name="createFileOnDisk">Specifies whether the file needs to be created by this method.</param>
    /// <returns>The full path to the temporary file.</returns>
    private string GetTemporaryFile(bool createFileOnDisk)
    {
      string tempFolder = Path.Combine(_root,
                                       VirtualEnvironment.GetFolderPath(VirtualFolder.Temporary));
      return VirtualEnvironment.GetUniqueFile(tempFolder, createFileOnDisk);
    }

    /// <summary>
    /// Writes a new entry to the table.
    /// The written key and value are returned as a <see cref="FileTableEntry"/>.
    /// </summary>
    /// <param name="key">The key of the new entry.</param>
    /// <param name="value">The value of the new entry.</param>
    /// <param name="overwrite">True if the value linked to the existing(?) key must be overwritten.</param>
    /// <returns></returns>
    private FileTableEntry WriteEntryToTable(string key, string value, bool overwrite)
    {
      _fileTableLock.EnterWriteLock();
      try
      {
        if (!_fileTable.ContainsKey(key))
          _fileTable.Add(key, value);
        else if (overwrite)
          _fileTable[key] = value;
        else
          value = _fileTable[key];
      }
      finally
      {
        _fileTableLock.ExitWriteLock();
      }
      return new FileTableEntry(key, value, FileKind.Unspecified);
    }

    #endregion

    #region IFileSystemProvider Members

    public virtual FileTableEntry GetFile(FileRequest fileRequest)
    {
      if (fileRequest.Name.StartsWith(@"\\.\")) /// Don't bother to try to redirect pipes.
        return new FileTableEntry(fileRequest.Name, fileRequest.Name, FileKind.Unspecified);
      if (FileAccessRedirector.IsTemporaryLocation(fileRequest.FullName)) /// Don't redirect temporary locations.
        return new FileTableEntry(fileRequest.FullName, fileRequest.FullName, FileKind.Unspecified);
      GuestCore.Log(new LogMessage(LogLevel.Debug, "Guest process requested file: " + fileRequest));
      /// Are we looking for a library?
      if (fileRequest.ResourceKind == ResourceKind.Library)
        return new FileTableEntry(fileRequest.Name, TryFindLibrary(fileRequest.Name), FileKind.File);
      /// Are we looking for a regular file?
      string filename; // Variable to hold the file's location.
      if (fileRequest.ResourceKind == ResourceKind.FileOrDirectory
          /// Can the file be found in the virtual file table?
          && TryGetFile(fileRequest.Name, out filename))
        /// The file is found, return its full path.
        return new FileTableEntry(fileRequest.Name, Path.Combine(_root, filename), FileKind.Unspecified);

      /// The requested resource doesn't exist yet... How will the requester handle this?
      if (fileRequest.CreationDisposition == FileCreationDisposition.CREATE_ALWAYS
          || fileRequest.CreationDisposition == FileCreationDisposition.CREATE_NEW
          || fileRequest.CreationDisposition == FileCreationDisposition.OPEN_ALWAYS)
      {
        /// The CreationDisposition specifies that the file will be created.
        /// Add a new entry to the file table and return it.
        var entry = AddNewEntryToFileTable(fileRequest);
        entry.Value = Path.Combine(_root, entry.Value);
        GuestCore.Log(new LogMessage(LogLevel.Debug, "New FileTableEntry: " + entry));
        return entry;
      }
      /// Else, the file won't be created.
      return new FileTableEntry(fileRequest.Name, fileRequest.Name, FileKind.Unspecified);
    }

    public virtual void DeleteFile(FileTableEntry fileTableEntry)
    {
      _fileTableLock.EnterWriteLock();
      try
      {
        _fileTable.Remove(new KeyValuePair<string, string>(fileTableEntry.Key, fileTableEntry.Value));
        if (fileTableEntry.FileKind != FileKind.Directory)
          return;
        /// Else, delete all subdirectories and subfiles, if any.
        /// NOTE: Won't Windows API handle this? Not sure...
        var markedForRemoval = new List<KeyValuePair<string, string>>();
        foreach (var entry in _fileTable)
            if (entry.Value.StartsWith(fileTableEntry.Value))
              markedForRemoval.Add(entry);  /// Can't remove while enumerating
        foreach (var entry in markedForRemoval)
          _fileTable.Remove(entry);
      }
      finally
      {
        _fileTableLock.ExitWriteLock();
      }
    }

    #endregion

  }
}
