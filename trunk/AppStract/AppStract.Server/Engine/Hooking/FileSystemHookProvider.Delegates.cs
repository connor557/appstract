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
using System.Runtime.InteropServices;
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Engine.Engine.FileSystem;

namespace AppStract.Engine.Engine.Hooking
{
  public partial class FileSystemHookProvider
  {

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

      #region GetTempPath

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate int GetTempPathW(
          int bufferLength,
          ref string tempPath
        );

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate int GetTempPathA(
          int bufferLength,
          ref string tempPath
        );


      #endregion

    }

  }
}
