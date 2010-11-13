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
using AppStract.Engine.Virtualization.Registry;
using ValueType = AppStract.Engine.Virtualization.Registry.ValueType;

namespace AppStract.Engine.Virtualization.Hooking
{

  public partial class RegistryHookProvider
  {
    /// <summary>
    /// Contains all delegates for hooked functions.
    /// </summary>
    private static class Delegates
    {

      #region CreateKey

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate NativeResultCode CreateKeyW
        (UIntPtr hKey, string lpSubKey, int reserved, string lpClass, RegOption dwOptions, RegAccessRights samDesired,
         ref int lpSecurityAttributes, ref UIntPtr phkResult, ref RegCreationDisposition lpdwDisposition);

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate NativeResultCode CreateKeyA
        (UIntPtr hKey, string lpSubKey, int reserved, string lpClass, RegOption dwOptions, RegAccessRights samDesired,
         ref int lpSecurityAttributes, ref UIntPtr phkResult, ref RegCreationDisposition lpdwDisposition);

      #endregion

      #region OpenKey

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate NativeResultCode OpenKeyW
        (UIntPtr hKey, string subKey, RegOption options, RegAccessRights sam, out UIntPtr phkResult);

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate NativeResultCode OpenKeyA
        (UIntPtr hKey, string subKey, RegOption options, RegAccessRights sam, out UIntPtr phkResult);

      #endregion

      #region CloseKey

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate NativeResultCode CloseKey
        (UIntPtr hKey);

      #endregion

      #region SetValue

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate NativeResultCode SetValueW
        (UIntPtr hKey, string lpValueName, uint reserved, ValueType dwType, IntPtr lpData, uint cbData);

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate NativeResultCode SetValueA
        (UIntPtr hKey, string lpValueName, uint reserved, ValueType dwType, IntPtr lpData, uint cbData);

      #endregion

      #region QueryValue

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
      public delegate NativeResultCode QueryValueW
        (UIntPtr hKey, string lpValueName, IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

      [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
      public delegate NativeResultCode QueryValueA
        (UIntPtr hKey, string lpValueName, IntPtr lpReserved, IntPtr lpType, IntPtr lpData, IntPtr lpcbData);

      #endregion

    }
  }

}
