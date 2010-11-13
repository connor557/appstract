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
using AppStract.Engine.Configuration;

namespace AppStract.Engine.Virtualization.FileSystem
{
  /// <summary>
  /// The default implementation of <see cref="IFileSystemProvider"/>.
  /// </summary>
  public sealed class FileSystemProvider : IFileSystemProvider
  {

    #region Variables

    /// <summary>
    /// The object representing the virtual environment to redirect requests to.
    /// </summary>
    private readonly VirtualEnvironment _virtualEnvironment;
    /// <summary>
    /// The collection of engine rules to apply during the virtualization process.
    /// </summary>
    private readonly FileSystemRuleCollection _engineRules;

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
      _engineRules = dataSource.GetFileSystemEngineRules();
      _virtualEnvironment = new VirtualEnvironment(rootDirectory);
      _virtualEnvironment.Initialize();
    }

    #endregion

    #region IFileSystemProvider Members

    public string GetVirtualPath(FileRequest request)
    {
      if (string.IsNullOrEmpty(request.Path)
          || _virtualEnvironment.IsLocatedInVirtualFileSystem(request.Path)
          || !_virtualEnvironment.IsVirtualizable(request.Path))
        return request.Path;
      // Make sure to have a full path AND the path's long file name.
      // BUT only if the requested resource is NOT a library!
      if (request.ResourceType != ResourceType.Library
          && (!Path.IsPathRooted(request.Path) || request.Path.Contains("~1")))
        request.Path = Path.GetFullPath(request.Path);
      // Get the type of virtualization that needs to be applied.
      VirtualizationType virtualizationType;
      if (!_engineRules.HasRule(request.Path, out virtualizationType))
        EngineCore.Log.Warning("No known engine rule for \"{0}\"", request.Path);
      if (virtualizationType == VirtualizationType.Transparent)
        return request.Path;
      // Redirect the call.
      var redirectedPath = _virtualEnvironment.RedirectRequest(request);
      EngineCore.Log.Debug("FileSystem Redirection: \"{0}\" => \"{1}\"", request.Path, redirectedPath);
      return redirectedPath;
    }

    #endregion

  }
}
