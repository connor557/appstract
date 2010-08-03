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
using System.Runtime.InteropServices;
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Server.FileSystem;

namespace AppStract.Server.Hooking
{
  /// <summary>
  /// Provides all API hooks regarding the file system.
  /// </summary>
  public class FileSystemHookProvider : HookProvider
  {

    #region Private Types

    /// <summary>
    /// Contains all delegates for hooked functions.
    /// </summary>
    private static class Delegates
    {

      #region CreateFile

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate IntPtr CreateFileW(
          string fileName,
          FileAccessRightFlags desiredAccess,
          FileShareModeFlags shareMode,
          NativeSecurityAttributes securityAttributes,
          FileCreationDisposition creationDisposition,
          FileFlagsAndAttributes flagsAndAttributes,
          IntPtr templateFile
         );

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate IntPtr CreateFileA(
          string fileName,
          FileAccessRightFlags desiredAccess,
          FileShareModeFlags shareMode,
          NativeSecurityAttributes securityAttributes,
          FileCreationDisposition creationDisposition,
          FileFlagsAndAttributes flagsAndAttributes,
          IntPtr templateFile
         );

      #endregion

      #region CreateDirectory

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate bool CreateDirectoryW(
          string fileName,
          NativeSecurityAttributes securityAttributes
        );

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate bool CreateDirectoryA(
          string fileName,
          NativeSecurityAttributes securityAttributes
        );

      #endregion

      #region DeleteFile

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate bool DeleteFileW(
          string fileName
        );

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate bool DeleteFileA(
          string fileName
        );

      #endregion

      #region RemoveDirectory

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate bool RemoveDirectoryW(
          string pathName
        );

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate bool RemoveDirectoryA(
          string pathName
        );

      #endregion

      #region LoadLibrary

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate IntPtr LoadLibraryExW(
          string fileName,
          IntPtr file,
          ModuleLoadFlags flags
        );

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate IntPtr LoadLibraryExA(
          string fileName,
          IntPtr file,
          ModuleLoadFlags flags
        );

      #endregion

    }

    /// <summary>
    /// Contains the methods to use for handling intercepted file system calls.
    /// </summary>
    private class FileSystemHookHandler : HookHandler
    {

      #region Variables

      /// <summary>
      /// The virtual file system to query when processing intercepted file system calls.
      /// </summary>
      private readonly IFileSystemProvider _fileSystem;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of <see cref="FileSystemHookHandler"/>.
      /// </summary>
      /// <param name="fileSystemProvider">The virtual file system to query when processing intercepted file system calls.</param>
      public FileSystemHookHandler(IFileSystemProvider fileSystemProvider)
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
        using (GuestCore.HookManager.ACL.GetHookingExclusion())
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
        using (GuestCore.HookManager.ACL.GetHookingExclusion())
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
        using (GuestCore.HookManager.ACL.GetHookingExclusion())
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
        using (GuestCore.HookManager.ACL.GetHookingExclusion())
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
        using (GuestCore.HookManager.ACL.GetHookingExclusion())
        {
          var virtualPath = _fileSystem.GetVirtualPath(request);
          return HostFileSystem.NativeAPI.LoadLibraryEx(virtualPath, file, flags);
        }
      }

      #endregion
      
    }

    #endregion

    #region Variables

    /// <summary>
    /// The object to use as callback object for the API hooks.
    /// </summary>
    private readonly object _callback;
    /// <summary>
    /// The <see cref="HookHandler"/> containing all methods needed by the API hooks.
    /// </summary>
    private readonly FileSystemHookHandler _hookHandler;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="FileSystemHookProvider"/>.
    /// </summary>
    /// <param name="fileSystemProvider">The <see cref="IFileSystemProvider"/> to use for the processing of intercepted API calls.</param>
    public FileSystemHookProvider(IFileSystemProvider fileSystemProvider)
    {
      _callback = fileSystemProvider;
      _hookHandler = new FileSystemHookHandler(fileSystemProvider);
    }

    #endregion

    #region Protected Methods

    protected override IEnumerable<HookData> GetHooks()
    {
      var hooks = new List<HookData>(10);
      hooks.Add(new HookData("Create Directory [Unicode]",
                             "kernel32.dll", "CreateDirectoryW",
                             new Delegates.CreateDirectoryW(_hookHandler.CreateDirectory),
                             _callback));
      hooks.Add(new HookData("Create Directory [Ansi]",
                             "kernel32.dll", "CreateDirectoryA",
                             new Delegates.CreateDirectoryA(_hookHandler.CreateDirectory),
                             _callback));
      hooks.Add(new HookData("Create File [Unicode]",
                             "kernel32.dll", "CreateFileW",
                             new Delegates.CreateFileW(_hookHandler.CreateFile),
                             _callback));
      hooks.Add(new HookData("Create File [Ansi]",
                             "kernel32.dll", "CreateFileA",
                             new Delegates.CreateFileA(_hookHandler.CreateFile),
                             _callback));
      hooks.Add(new HookData("Delete File [Unicode]",
                             "kernel32.dll", "DeleteFileW",
                             new Delegates.DeleteFileW(_hookHandler.DeleteFile),
                             _callback));
      hooks.Add(new HookData("Delete File [Ansi]",
                             "kernel32.dll", "DeleteFileA",
                             new Delegates.DeleteFileA(_hookHandler.DeleteFile),
                             _callback));
      hooks.Add(new HookData("Remove Directory [Unicode]",
                             "kernel32.dll", "RemoveDirectoryW",
                             new Delegates.RemoveDirectoryW(_hookHandler.RemoveDirectory),
                             _callback));
      hooks.Add(new HookData("Remove Directory [Ansi]",
                             "kernel32.dll", "RemoveDirectoryA",
                             new Delegates.RemoveDirectoryA(_hookHandler.RemoveDirectory),
                             _callback));
      hooks.Add(new HookData("Load Library [Unicode]",
                             "kernel32.dll", "LoadLibraryExW",
                             new Delegates.LoadLibraryExW(_hookHandler.LoadLibraryEx),
                             _callback));
      hooks.Add(new HookData("Load Library [Ansi]",
                             "kernel32.dll", "LoadLibraryExA",
                             new Delegates.LoadLibraryExA(_hookHandler.LoadLibraryEx),
                             _callback));
      return hooks;
    }

    #endregion

  }
}
