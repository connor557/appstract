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
using System.IO;
using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.FileSystem;

namespace AppStract.Server.FileSystem
{
  /// <summary>
  /// The default implementation of <see cref="IFileSystemProvider"/>.
  /// </summary>
  public sealed class FileSystemProvider : IFileSystemProvider
  {

    #region Variables

    /// <summary>
    /// The root of the filesystem,
    /// which is a path to a directory in the real file system.
    /// </summary>
    private readonly string _root;
    /// <summary>
    /// The collection of engine rules to apply during the virtualization process.
    /// </summary>
    private readonly FileSystemRuleCollection _engineRules;

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
    /// <param name="dataSource">
    /// The <see cref="IFileSystemSynchronizer"/> to use as data source for the already known file table entries.
    /// </param>
    /// <param name="rootDirectory">
    /// The path to use as root directory.
    /// All filenames going in and out of the current <see cref="FileSystemProvider"/> should be interpreted as relatives to this path.
    /// </param>
    public FileSystemProvider(IFileSystemSynchronizer dataSource, string rootDirectory)
    {
      if (rootDirectory == null)
        throw new ArgumentNullException("rootDirectory");
      _root = !Path.IsPathRooted(rootDirectory)
                ? Path.GetFullPath(rootDirectory)
                : rootDirectory;
      _engineRules = dataSource.GetFileSystemEngineRules();
      VirtualEnvironment.CreateVirtualFolders(_root);
    }

    #endregion

    #region IFileSystemProvider Members

    public string GetVirtualPath(string path, ResourceType resourceType)
    {
      if (string.IsNullOrEmpty(path)
          || IsPipe(path)
          || FileAccessRedirector.IsTemporaryLocation(path))
        return path;
      VirtualizationType virtualizationType;
      if (!_engineRules.HasRule(path, out virtualizationType))
        GuestCore.Log.Warning("No known engine rule for \"{0}\"", path);
      if (virtualizationType == VirtualizationType.Transparent)
        return path;
      var redirectedPath = FileAccessRedirector.Redirect(path);
      GuestCore.Log.Debug("FileSystem Redirection: \"{0}\" => \"{1}\"", path, redirectedPath);
      return redirectedPath;
    }

    #endregion

    #region Private Static Methods

    private static bool IsPipe(string path)
    {
      return path.StartsWith(@"\\.\");
    }

    #endregion

  }
}
