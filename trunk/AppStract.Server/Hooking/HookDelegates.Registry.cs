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
    public delegate uint DSetValue(
        IntPtr hkey,
        [MarshalAs(UnmanagedType.LPWStr)] String lpValueName,
        uint Reserved, uint dwType,
        IntPtr lpData, uint cbData
      );

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DCreateKey
                   (IntPtr hKey, string lpSubKey, int Reserved,
                   string lpClass, uint dwOptions, uint samDesired,
                   ref int lpSecurityAttributes,
                   out IntPtr phkResult, ref int lpdwDisposition);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DOpenKey
                   (IntPtr hKey, string subKey, uint options,
                    int sam, out IntPtr phkResult);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DQueryValue
                   (IntPtr hkey, [MarshalAs(UnmanagedType.LPWStr)] String lpValueName,
                    IntPtr lpReserved, ref uint? lpType, IntPtr? lpData, ref uint? lpcbData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DCloseKey
                   (IntPtr hKey);

  }
}
