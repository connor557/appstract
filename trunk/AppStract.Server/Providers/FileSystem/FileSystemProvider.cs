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
using AppStract.Server.Communication;
using AppStract.Server.FileSystem;
using AppStract.Utilities.Observables;


namespace AppStract.Server.Providers.FileSystem
{
  public class FileSystemProvider : IFileSystemProvider
  {

    #region Variables

    private readonly Random _randomGenerator;
    private readonly IDictionary<string, string> _fileTable;
    private readonly ReaderWriterLockSlim _fileTableLock;
    private readonly IResourceSynchronizer _resourceSynchronizer;
    private readonly string _rootDirectory;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the fully qualified path of the working directory.
    /// </summary>
    public string CurrentDirectory
    {
      get { return _rootDirectory; }
    }

    #endregion

    #region Constructors

    public FileSystemProvider(string rootDirectory, IResourceSynchronizer resourceSynchronizer)
    {
      _fileTable = new ObservableDictionary<string, string>();
      _fileTableLock = new ReaderWriterLockSlim();
      _randomGenerator = new Random(DateTime.Now.Millisecond);
      _resourceSynchronizer = resourceSynchronizer;
      if (!Path.IsPathRooted(rootDirectory))
        _rootDirectory = Path.GetFullPath(rootDirectory);
    }

    #endregion

    #region Public Methods

    public void LoadFileTable()
    {
      _resourceSynchronizer.LoadFileSystemTo(this);
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
      /// Didn't find libraryPath in the file table,
      /// try again with the full path.
      if (!Path.IsPathRooted(libraryPath))
      {
        string fullLibraryPath = Path.GetFullPath(libraryPath);
        if (TryGetFile(fullLibraryPath, out result))
          return result;
      }
      
      /// Still not found, redirect the request and see if the library is found.
      string redirectedPath = FileAccessRedirector.Redirect(libraryPath);
      if (File.Exists(redirectedPath))
        /// The file exists in the virtual file system, return it.
        return redirectedPath;
      /// Still not found?
      /// We're sure the virtual folders don't contain the dll (because of TryGetFile).
      /// ToDo: Are we sure? Need to debug this!
      /// Return the parameter and let Windows handle the search.
      return libraryPath;
    }

    /// <summary>
    /// Adds a new file to the file table for the given key.
    /// </summary>
    /// <param name="keyFilename">Value used as key and used as base for the linked value.</param>
    /// <returns>The created <see cref="FileTableEntry"/>.</returns>
    private FileTableEntry AddNewEntryToFileTable(string keyFilename)
    {
      string fileEntryValue = FileAccessRedirector.Redirect(keyFilename);
      string fullPath = Path.Combine(_rootDirectory, fileEntryValue);
      while (File.Exists(fullPath))
      {
        string filename = Path.GetFileName(fileEntryValue);
        string newFilename = string.Format("{0}{1}{2}",
                                           Path.GetFileNameWithoutExtension(filename),
                                           _randomGenerator.Next(10000, 999999),
                                           Path.GetExtension(filename));
        fileEntryValue = fileEntryValue.Replace(filename, newFilename);
        fullPath = Path.Combine(_rootDirectory, fileEntryValue);
      }
      return WriteEntryToTable(keyFilename, fileEntryValue, false);
    }

    /// <summary>
    /// Returns the full path of a unique, zero-filled, temporary file.
    /// </summary>
    /// <param name="createFileOnDisk">Specifies whether the file needs to be created by this method.</param>
    /// <returns>The full path to the temporary file.</returns>
    private string GetTemporaryFile(bool createFileOnDisk)
    {
      string tempFolder = Path.Combine(_rootDirectory,
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
      return new FileTableEntry(key, value);
    }

    #endregion

    #region IFileSystemProvider Members

    public virtual FileTableEntry GetFile(FileRequest fileRequest)
    {
      /// Are we looking for a library?
      if (fileRequest.ResourceKind == ResourceKind.Library)
        return new FileTableEntry(fileRequest.FileName, TryFindLibrary(fileRequest.FileName));
      
      string filename; // Variable to hold the file's location.
      /// Are we looking for a regular file?
      if (fileRequest.ResourceKind == ResourceKind.FileOrDirectory
          /// Can the file be found in the virtual file table?
          && TryGetFile(fileRequest.FileName, out filename))
        /// The file is found, return its full path.
        return new FileTableEntry(fileRequest.FileName, Path.Combine(_rootDirectory, filename));

      /// The requested resource doesn't exist yet... How will the requester handle this?
      if (fileRequest.CreationDisposition == CreationDisposition.CREATE_ALWAYS
          || fileRequest.CreationDisposition == CreationDisposition.CREATE_NEW
          || fileRequest.CreationDisposition == CreationDisposition.OPEN_ALWAYS)
        /// The CreationDisposition specifies that the file will be created.
        /// Add a new entry to the file table and return it.
        return AddNewEntryToFileTable(fileRequest.FileName);
      /// Else, the file won't be created.
      /// Return a non-existing temporary file without creating a filetable-entry for it.
      return new FileTableEntry(fileRequest.FileName, GetTemporaryFile(false));
    }

    #endregion

  }
}
