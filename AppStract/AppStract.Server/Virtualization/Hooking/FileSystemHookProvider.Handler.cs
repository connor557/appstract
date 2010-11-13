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
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Engine.Virtualization.FileSystem;

namespace AppStract.Engine.Virtualization.Hooking
{
  public partial class FileSystemHookProvider
  {

    /// <summary>
    /// Contains the methods to use for handling intercepted file system calls.
    /// </summary>
    private class Handler : HookHandler
    {

      #region Variables

      /// <summary>
      /// The virtual file system to query when processing intercepted file system calls.
      /// </summary>
      private readonly IFileSystemProvider _fileSystem;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of <see cref="Handler"/>.
      /// </summary>
      /// <param name="fileSystemProvider">The virtual file system to query when processing intercepted file system calls.</param>
      public Handler(IFileSystemProvider fileSystemProvider)
      {
        _fileSystem = fileSystemProvider;
      }

      #endregion

      #region Public Methods

      /// <summary>
      /// Handles intercepted file access.
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="desiredAccess"></param>
      /// <param name="shareMode"></param>
      /// <param name="securityAttributes"></param>
      /// <param name="creationDisposition"></param>
      /// <param name="flagsAndAttributes"></param>
      /// <param name="templateFile"></param>
      /// <returns></returns>
      public IntPtr CreateFile(string fileName, FileAccessRightFlags desiredAccess, FileShareModeFlags shareMode,
                                 NativeSecurityAttributes securityAttributes, FileCreationDisposition creationDisposition,
                                 FileFlagsAndAttributes flagsAndAttributes, IntPtr templateFile)
      {
        var request = new FileRequest
        {
          CreationDisposition = creationDisposition,
          Path = fileName,
          ResourceType = ResourceType.File
        };
        using (EngineCore.Engine.GetEngineProcessingSpace())
        {
          var virtualPath = _fileSystem.GetVirtualPath(request);
          return HostFileSystem.NativeAPI.CreateFile(virtualPath, desiredAccess, shareMode, securityAttributes,
                                      creationDisposition, flagsAndAttributes, templateFile);
        }
      }

      /// <summary>
      /// Handles intercepted requests to delete a file.
      /// </summary>
      /// <param name="fileName"></param>
      /// <returns></returns>
      public bool DeleteFile(string fileName)
      {
        var request = new FileRequest
        {
          CreationDisposition = FileCreationDisposition.OpenExisting,
          Path = fileName,
          ResourceType = ResourceType.File
        };
        using (EngineCore.Engine.GetEngineProcessingSpace())
        {
          var virtualPath = _fileSystem.GetVirtualPath(request);
          return HostFileSystem.NativeAPI.DeleteFile(virtualPath);
        }
      }

      /// <summary>
      /// Handles intercepted directory access.
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="securityAttributes"></param>
      /// <returns></returns>
      public bool CreateDirectory(string fileName, NativeSecurityAttributes securityAttributes)
      {
        var request = new FileRequest
        {
          CreationDisposition = FileCreationDisposition.OpenAlways,
          Path = fileName,
          ResourceType = ResourceType.Directory
        };
        using (EngineCore.Engine.GetEngineProcessingSpace())
        {
          var virtualPath = _fileSystem.GetVirtualPath(request);
          return HostFileSystem.NativeAPI.CreateDirectory(virtualPath, securityAttributes);
        }
      }

      /// <summary>
      /// Handles intercepted requests to remove a directory.
      /// </summary>
      /// <param name="pathName"></param>
      /// <returns></returns>
      public bool RemoveDirectory(string pathName)
      {
        var request = new FileRequest
        {
          CreationDisposition = FileCreationDisposition.OpenExisting,
          Path = pathName,
          ResourceType = ResourceType.Directory
        };
        using (EngineCore.Engine.GetEngineProcessingSpace())
        {
          var virtualPath = _fileSystem.GetVirtualPath(request);
          return HostFileSystem.NativeAPI.RemoveDirectory(virtualPath);
        }
      }

      /// <summary>
      /// Handles intercepted library access.
      /// </summary>
      /// <param name="fileName"></param>
      /// <param name="file"></param>
      /// <param name="flags"></param>
      /// <returns></returns>
      public IntPtr LoadLibraryEx(string fileName, IntPtr file, ModuleLoadFlags flags)
      {
        var request = new FileRequest
        {
          CreationDisposition = FileCreationDisposition.OpenExisting,
          Path = fileName,
          ResourceType = ResourceType.Library
        };
        using (EngineCore.Engine.GetEngineProcessingSpace())
        {
          var virtualPath = _fileSystem.GetVirtualPath(request);
          return HostFileSystem.NativeAPI.LoadLibraryEx(virtualPath, file, flags);
        }
      }

      /// <summary>
      /// Retrieves the path of the directory designated for temporary files.
      /// </summary>
      /// <param name="bufferSize">The size of the string buffer identified by lpBuffer, in TCHARs.</param>
      /// <param name="tempPath">
      /// A pointer to a string buffer that receives the null-terminated string specifying the temporary file path.
      /// The returned string ends with a backslash, for example, C:\TEMP\.
      /// </param>
      /// <returns>
      /// If the function succeeds, the return value is the length, in TCHARs, of the string copied to lpBuffer,
      /// not including the terminating null character. If the return value is greater than nBufferLength,
      /// the return value is the length, in TCHARs, of the buffer required to hold the path.
      /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
      /// </returns>
      public int GetTempPath(int bufferSize, ref string tempPath)
      {
        if (tempPath == null)
          return 0;
        string path;
        using (EngineCore.Engine.GetEngineProcessingSpace())
        {
          var request = new FileRequest { Path = System.IO.Path.GetTempPath() };
          path = _fileSystem.GetVirtualPath(request);
        }
        if (!path.EndsWith(@"\"))
          path = path + @"\";
        if (bufferSize >= path.Length + 1) // + terminating null character
        {
          try
          {
            tempPath = path;
          }
          catch (AccessViolationException)
          {
            return 0;
          }
        }
        return path.Length;
      }

      #endregion

    }

  }
}
