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
using AppStract.Core.Virtualization.Engine.FileSystem;

namespace AppStract.Server.Engine.FileSystem
{
  /// <summary>
  /// Provides information about, and means to manipulate,
  /// the current virtual environment and platform.
  /// </summary>
  public class VirtualEnvironment
  {

    #region Variables

    /// <summary>
    /// The root of the virtual file system.
    /// </summary>
    private readonly string _root;
    /// <summary>
    /// Provides the virtual counterparts of paths used by the host file system.
    /// </summary>
    private readonly FileSystemRedirector _redirector;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the root folder of the virtual environment.
    /// </summary>
    public string FileSystemRoot
    {
      get { return _root; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new <see cref="VirtualEnvironment"/> which can be used to redirect <see cref="FileRequest"/>s to.
    /// </summary>
    /// <param name="rootDirectory"></param>
    public VirtualEnvironment(string rootDirectory)
    {
      if (rootDirectory == null)
        throw new ArgumentNullException("rootDirectory");
      rootDirectory = !Path.IsPathRooted(rootDirectory)
                        ? Path.GetFullPath(rootDirectory)
                        : rootDirectory;
      _root = rootDirectory.ToLowerInvariant();
      _redirector = new FileSystemRedirector();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the current virtual environment so it can be used to redirect calls to.
    /// </summary>
    public void Initialize()
    {
      CreateSystemFolders(FileSystemRoot);
    }

    /// <summary>
    /// Returns whether or nor the specified <paramref name="path"/> is virtualizable.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsVirtualizable(string path)
    {
      if (path.StartsWith(@"\\.\"))
      {
        // Physical Disks and Volumes or Changer Device or Tape Drive or Communications Resource or Named Pipe
        // EXCEPT FOR paths like: @"\\.\C:\" -> opens the file system of the C: volume.
        if (!(path.Length >= 7 && path[5] == ':' && path[6] == '\\'))
          return false;
      }
      if (path.StartsWith(@"\\\\.\\"))
        // Changer Device or Tape Drive from C or C++
        return false;
      if (path.Equals("CONIN$", StringComparison.InvariantCultureIgnoreCase)
          || path.Equals("CONOUT$", StringComparison.InvariantCultureIgnoreCase))
        // Console In or Console Out
        return false;
      if (path.ToLowerInvariant().StartsWith(_root))
        // The path already points to the virtual environment.
        return false;
      // None of the above, save to virtualize the specified path.
      return true;
    }

    /// <summary>
    /// Returns whether or nor the specified <paramref name="path"/> is located in the virtual file system.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public bool IsLocatedInVirtualFileSystem(string path)
    {
      return path.StartsWith(_root, StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Redirects the given <see cref="FileRequest"/> to the virtual environment.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public string RedirectRequest(FileRequest request)
    {
      // Redirect the path to the virtual environment.
      var result = _redirector.Redirect(request, _root);
      // Verify the result.
      result = GetVerifiedRequestResult(result);
      return result.Path;
    }

    #endregion

    #region Private Static Methods

    /// <summary>
    /// Tries to create all system-folders, as defined in <see cref="VirtualFolder"/>.
    /// </summary>
    /// <param name="rootDirectory">The folder under which all system folders must be created.</param>
    /// <returns>True if all folders are created; False if the creation of one or more folders failed.</returns>
    private static void CreateSystemFolders(string rootDirectory)
    {
      EngineCore.Log.Message("Creating system folders for a virtual environment with root \"{0}\"", rootDirectory);
      foreach (VirtualFolder virtualFolder in Enum.GetValues(typeof(VirtualFolder)))
        if (!HostFileSystem.TryCreateDirectory(Path.Combine(rootDirectory, virtualFolder.ToPath())))
          EngineCore.Log.Critical("Failed to create virtual system folder: " + virtualFolder);
    }

    /// <summary>
    /// Verifies that the specified <see cref="FileRequestResult"/> can be used in the current virtual environment.
    /// </summary>
    /// <param name="fileRequestResult"></param>
    /// <returns></returns>
    private static FileRequestResult GetVerifiedRequestResult(FileRequestResult fileRequestResult)
    {
      if (File.Exists(fileRequestResult.Path))
        // File exists, request is suposed to be valid.
        return fileRequestResult;
      // Path doesn't exist, determine whether it should be used anyway.
      if (fileRequestResult.Request.ResourceType == ResourceType.Library)
        // Don't use: The target is a library unknown to the virtual environment.
        fileRequestResult.Path = fileRequestResult.Request.Path;
      if (fileRequestResult.Request.CreationDisposition == FileCreationDisposition.OpenExisting)
        // Don't use: The target won't be created, it's save to return original path.
        fileRequestResult.Path = fileRequestResult.Request.Path;
      if (fileRequestResult.Request.CreationDisposition == FileCreationDisposition.Unspecified)
      {
        // Note: The following lines need to be reviewed!
        // In some cases the API hook handler receives this value for FileCreationDisposition,
        // which is (according to the documentation) invalid for any of the file management API functions.
        // The source and meaning of this value is unknown, the following is more or less a work-around.
        if (fileRequestResult.SystemFolder != VirtualFolder.Temporary)
          fileRequestResult.Path = fileRequestResult.Request.Path;
        // Else: the path leads to a temporary resource, which should always reside in the virtual environment.
      }
      return fileRequestResult;
    }

    #endregion

  }
}