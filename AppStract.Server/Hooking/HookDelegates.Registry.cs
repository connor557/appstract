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

    #region CreateKey

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DCreateKey_Unicode
                   (IntPtr hKey, string lpSubKey, int Reserved,
                   string lpClass, uint dwOptions, uint samDesired,
                   ref int lpSecurityAttributes,
                   out IntPtr phkResult, ref int lpdwDisposition);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DCreateKey_Ansi
                   (IntPtr hKey, string lpSubKey, int Reserved,
                   string lpClass, uint dwOptions, uint samDesired,
                   ref int lpSecurityAttributes,
                   out IntPtr phkResult, ref int lpdwDisposition);
    #endregion

    #region OpenKey

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DOpenKey_Unicode
                   (IntPtr hKey, string subKey, uint options,
                    int sam, out IntPtr phkResult);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DOpenKey_Ansi
                   (IntPtr hKey, string subKey, uint options,
                    int sam, out IntPtr phkResult);

    #endregion

    #region CloseKey

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DCloseKey
                   (IntPtr hKey);

    #endregion

    #region SetValue

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DSetValue_Unicode
                    (IntPtr hkey,
                    string lpValueName,
                    uint Reserved, uint dwType,
                    IntPtr lpData, uint cbData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DSetValue_Ansi
                    (IntPtr hkey,
                    string lpValueName,
                    uint Reserved, uint dwType,
                    IntPtr lpData, uint cbData);

    #endregion

    #region QueryValue

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DQueryValue_Unicode
                   (IntPtr hkey, string lpValueName,
                    IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DQueryValue_Ansi
                   (IntPtr hkey, string lpValueName,
                    IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

    #endregion

  }
}
