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
using System.Runtime.InteropServices;

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
        String InFileName,
        UInt32 InDesiredAccess,
        UInt32 InShareMode,
        IntPtr InSecurityAttributes,
        UInt32 InCreationDisposition,
        UInt32 InFlagsAndAttributes,
        IntPtr InTemplateFile
       );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate IntPtr DCreateFile_Ansi(
        String InFileName,
        UInt32 InDesiredAccess,
        UInt32 InShareMode,
        IntPtr InSecurityAttributes,
        UInt32 InCreationDisposition,
        UInt32 InFlagsAndAttributes,
        IntPtr InTemplateFile
       );

    #endregion

    #region CreateDirectory

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate IntPtr DCreateDirectory_Unicode(
        String InFileName,
        IntPtr InSecurityAttributes
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate IntPtr DCreateDirectory_Ansi(
        String InFileName,
        IntPtr InSecurityAttributes
      );

    #endregion

    #region DeleteFile

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate bool DDeleteFile_Unicode(
        String InFileName
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate bool DDeleteFile_Ansi(
        String InFileName
      );

    #endregion

    #region LoadLibrary

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate IntPtr DLoadLibraryEx_Unicode(
        String dllFileName,
        IntPtr handel,
        uint mozart
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate IntPtr DLoadLibraryEx_Ansi(
        String dllFileName,
        IntPtr handel,
        uint mozart
      );

    #endregion

  }
}
