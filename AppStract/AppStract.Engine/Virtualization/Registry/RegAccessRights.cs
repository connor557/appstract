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

namespace AppStract.Engine.Virtualization.Registry
{
  /// <summary>
  /// A mask that specifies the access rights for the key.
  /// </summary>
  [Flags]
  public enum RegAccessRights
  {
    /// <summary>
    /// The standard security descriptor.
    /// </summary>
    Standard = 0x00000000,
    /// <summary>
    /// Required to query the values of a registry key.
    /// </summary>
    QueryValue = 0x00000001,
    /// <summary>
    /// Required to create, delete, or set a registry value.
    /// </summary>
    SetValue = 0x00000002,
    /// <summary>
    /// Required to create a subkey of a registry key.
    /// </summary>
    CreateSubKey = 0x00000004,
    /// <summary>
    /// Required to enumerate the subkeys of a registry key.
    /// </summary>
    EnumerateSubKeys = 0x00000008,
    /// <summary>
    /// Required to request change notifications for a registry key or for subkeys of a registry key.
    /// </summary>
    Notify = 0x00000010,
    /// <summary>
    /// Reserved for system use.
    /// </summary>
    CreateLink = 0x00000020,
    /// <summary>
    /// Indicates that an application on 64-bit Windows should operate on the 32-bit registry view.
    /// </summary>
    WOW64_32Key = 0x00000200,
    /// <summary>
    /// Indicates that an application on 64-bit Windows should operate on the 64-bit registry view.
    /// </summary>
    WOW64_64Key = 0x00000100,
    WOW64_Res = 0x00000300,
    /// <summary>
    /// Combines the <see cref="Standard"/>, <see cref="QueryValue"/>, <see cref="EnumerateSubKeys"/>, and <see cref="Notify"/> values.
    /// </summary>
    Read = 0x00020019,
    /// <summary>
    /// Combines the <see cref="Standard"/>, <see cref="SetValue"/>, and <see cref="CreateSubKey"/> access rights.
    /// </summary>
    Write = 0x00020006,
    /// <summary>
    /// Equivalent to <see cref="Read"/>.
    /// </summary>
    Execute = 0x00020019,
    /// <summary>
    /// Combines the <see cref="Standard"/>, <see cref="Read"/>, <see cref="SetValue"/>, <see cref="CreateSubKey"/>, <see cref="EnumerateSubKeys"/>, <see cref="Notify"/>, and <see cref="CreateLink"/> access rights.
    /// </summary>
    AllAccess = 0x000F003F
  }
}
