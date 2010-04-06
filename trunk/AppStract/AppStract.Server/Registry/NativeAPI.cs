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
using AppStract.Core.Virtualization.Interop;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// Defines native registry API functions.
  /// </summary>
  internal static class NativeAPI
  {

    /// <summary>
    /// The <see cref="KeyInformationClass"/> enumeration type represents the type of information to supply about a registry key.
    /// </summary>
    /// <remarks>
    /// Use the <see cref="KeyInformationClass"/> values to specify the type of data to be supplied by the ZwEnumerateKey and ZwQueryKey routines.
    /// </remarks>
    public enum KeyInformationClass
    {
      /// <summary>
      /// A KEY_BASIC_INFORMATION structure is supplied.
      /// </summary>
      KeyBasicInformation = 0,
      /// <summary>
      /// A KEY_NODE_INFORMATION structure is supplied.
      /// </summary>
      KeyNodeInformation = 1,
      /// <summary>
      /// A KEY_FULL_INFORMATION structure is supplied.
      /// </summary>
      KeyFullInformation = 2,
      /// <summary>
      /// A KEY_NAME_INFORMATION structure is supplied.
      /// </summary>
      KeyNameInformation = 3,
      /// <summary>
      /// A KEY_CACHED_INFORMATION structure is supplied.
      /// </summary>
      KeyCachedInformation = 4,
      /// <summary>
      /// Undefined
      /// </summary>
      KeyFlagsInformation = 5,
      /// <summary>
      /// A KEY_VIRTUALIZATION_INFORMATION structure is supplied.
      /// </summary>
      KeyVirtualizationInformation = 6,
      /// <summary>
      /// Undefined
      /// </summary>
      KeyHandleTagsInformation = 7,
      /// <summary>
      /// The maximum value in this enumeration type.
      /// </summary>
      MaxKeyInfoClass = 8
    }

    /// <summary>
    /// The ZwQueryKey routine provides information about the class of a registry key, and the number and sizes of its subkeys.
    /// </summary>
    /// <param name="KeyHandle">
    /// Handle to the registry key to obtain information about. This handle is created by a successful call to ZwCreateKey or ZwOpenKey.
    /// </param>
    /// <param name="KeyInformationClass">
    /// Specifies a KEY_INFORMATION_CLASS value that determines the type of information returned in the KeyInformation buffer.
    /// </param>
    /// <param name="KeyInformation">
    /// Pointer to a caller-allocated buffer that receives the requested information.
    /// </param>
    /// <param name="bufferSize">
    /// Specifies the size, in bytes, of the KeyInformation buffer.
    /// </param>
    /// <param name="resultSize">
    /// Pointer to a variable that receives the size, in bytes, of the requested key information.
    /// If ZwQueryKey returns STATUS_SUCCESS, the variable contains the amount of data returned.
    /// If ZwQueryKey returns STATUS_BUFFER_OVERFLOW or STATUS_BUFFER_TOO_SMALL, you can use the value of the variable to determine the required buffer size.
    /// </param>
    /// <remarks>
    /// The KeyHandle passed to ZwQueryKey must have been opened with KEY_QUERY_VALUE access.
    /// </remarks>
    /// <returns></returns>
    [DllImport("ntdll.dll", SetLastError = true)]
    public static extern NativeResultCode NtQueryKey(UIntPtr KeyHandle,
                                                      KeyInformationClass KeyInformationClass,
                                                      IntPtr KeyInformation,
                                                      int bufferSize,
                                                      out int resultSize);

    /// <summary>
    /// Closes a handle to the specified registry key.
    /// </summary>
    /// <param name="hKey">A handle to the open key to be closed.</param>
    /// <returns></returns>
    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern NativeResultCode RegCloseKey(uint hKey);

  }
}
