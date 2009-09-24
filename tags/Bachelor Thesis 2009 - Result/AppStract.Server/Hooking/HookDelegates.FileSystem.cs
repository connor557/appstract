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

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate IntPtr DCreateFile(
        String InFileName,
        UInt32 InDesiredAccess,
        UInt32 InShareMode,
        IntPtr InSecurityAttributes,
        UInt32 InCreationDisposition,
        UInt32 InFlagsAndAttributes,
        IntPtr InTemplateFile
       );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate IntPtr DCreateDirectory(
        String InFileName,
        IntPtr InSecurityAttributes
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate IntPtr DLoadLibraryEx(
        String dllFileName,
        IntPtr handel,
        uint mozart
      );

  }
}
