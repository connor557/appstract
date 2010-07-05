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
using AppStract.Server.FileSystem;

namespace AppStract.Server.Hooking
{
  /// <summary>
  /// Contains all delegates for hooked functions.
  /// </summary>
  public static partial class HookDelegates
  {

    #region CreateFile

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate IntPtr DCreateFile_Unicode(
        string fileName,
        FileAccessRightFlags desiredAccess,
        FileShareModeFlags shareMode,
        NativeSecurityAttributes securityAttributes,
        FileCreationDisposition creationDisposition,
        FileFlagsAndAttributes flagsAndAttributes,
        IntPtr templateFile
       );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate IntPtr DCreateFile_Ansi(
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
    public delegate bool DCreateDirectory_Unicode(
        string fileName,
        NativeSecurityAttributes securityAttributes
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate bool DCreateDirectory_Ansi(
        string fileName,
        NativeSecurityAttributes securityAttributes
      );

    #endregion

    #region DeleteFile

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate bool DDeleteFile_Unicode(
        string fileName
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate bool DDeleteFile_Ansi(
        string fileName
      );

    #endregion

    #region RemoveDirectory

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate bool DRemoveDirectory_Unicode(
        string pathName
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate bool DRemoveDirectory_Ansi(
        string pathName
      );

    #endregion

    #region LoadLibrary

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate IntPtr DLoadLibraryEx_Unicode(
        string fileName,
        IntPtr file,
        ModuleLoadFlags flags
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate IntPtr DLoadLibraryEx_Ansi(
        string fileName,
        IntPtr file,
        ModuleLoadFlags flags
      );

    #endregion

  }
}
