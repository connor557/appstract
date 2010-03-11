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
using ValueType = AppStract.Core.Virtualization.Registry.ValueType;

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
                   (UIntPtr hKey, string lpSubKey, int Reserved,
                   string lpClass, uint dwOptions, uint samDesired,
                   ref int lpSecurityAttributes,
                   out UIntPtr phkResult, ref int lpdwDisposition);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DCreateKey_Ansi
                   (UIntPtr hKey, string lpSubKey, int Reserved,
                   string lpClass, uint dwOptions, uint samDesired,
                   ref int lpSecurityAttributes,
                   out UIntPtr phkResult, ref int lpdwDisposition);
    #endregion

    #region OpenKey

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DOpenKey_Unicode
                   (UIntPtr hKey, string subKey, uint options,
                    int sam, out UIntPtr phkResult);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DOpenKey_Ansi
                   (UIntPtr hKey, string subKey, uint options,
                    int sam, out UIntPtr phkResult);

    #endregion

    #region CloseKey

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DCloseKey
                   (UIntPtr hKey);

    #endregion

    #region SetValue

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DSetValue_Unicode
                    (UIntPtr hKey,
                    string lpValueName,
                    uint Reserved, ValueType dwType,
                    IntPtr lpData, uint cbData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DSetValue_Ansi
                    (UIntPtr hKey,
                    string lpValueName,
                    uint Reserved, ValueType dwType,
                    IntPtr lpData, uint cbData);

    #endregion

    #region QueryValue

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
    public delegate uint DQueryValue_Unicode
                   (UIntPtr hKey, string lpValueName,
                    IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
    public delegate uint DQueryValue_Ansi
                   (UIntPtr hKey, string lpValueName,
                    IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

    #endregion

  }
}
