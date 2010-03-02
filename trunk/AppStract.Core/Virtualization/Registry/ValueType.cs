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

using System.Collections.Generic;
using Microsoft.Win32;

namespace AppStract.Core.Virtualization.Registry
{
  /// <summary>
  /// The codes indicating the type of data stored in a specified value.
  /// </summary>
  public enum ValueType
  {
    /// <summary>
    /// No Type.
    /// </summary>
    REG_NONE = 0,
    /// <summary>
    /// String value.
    /// </summary>
    REG_SZ = 1,
    /// <summary>
    /// An expandable string value that can contain environment variables.
    /// </summary>
    REG_EXPAND_SZ = 2,
    /// <summary>
    /// Binary Data.
    /// </summary>
    REG_BINARY = 3,
    /// <summary>
    /// A DWORD value, a 32-bit unsigned integer.
    /// </summary>
    REG_DWORD_LITTLE_ENDIAN = 4,
    /// <summary>
    /// A DWORD value, a 32-bit unsigned integer.
    /// </summary>
    REG_DWORD_BIG_ENDIAN = 5,
    /// <summary>
    /// Symbolic link (UNICODE).
    /// </summary>
    REG_LINK = 6,
    /// <summary>
    /// Multi-string value, an array of unique strings.
    /// </summary>
    REG_MULTI_SZ = 7,
    /// <summary>
    /// Resource List.
    /// </summary>
    REG_RESOURCE_LIST = 8,
    /// <summary>
    /// Resource Descriptor.
    /// </summary>
    REG_FULL_RESOURCE_DESCRIPTOR = 9,
    /// <summary>
    /// Resource Requirements List.
    /// </summary>
    REG_RESOURCE_REQUIREMENTS_LIST = 10,
    /// <summary>
    /// A QWORD value, a 64-bit integer.
    /// </summary>
    REG_QWORD_LITTLE_ENDIAN = 11,
    /// <summary>
    /// Specifies an invalid value, should never be used in Win32 communications.
    /// </summary>
    INVALID = int.MaxValue
  }

  public static class ValueTypeExtensions
  {

    private static readonly IDictionary<RegistryValueKind, ValueType> _valueMap
      = new Dictionary<RegistryValueKind, ValueType>
          {
            {RegistryValueKind.Binary, ValueType.REG_BINARY},
            {RegistryValueKind.DWord, ValueType.REG_DWORD_LITTLE_ENDIAN},
            {RegistryValueKind.ExpandString, ValueType.REG_EXPAND_SZ},
            {RegistryValueKind.MultiString, ValueType.REG_MULTI_SZ},
            {RegistryValueKind.QWord, ValueType.REG_QWORD_LITTLE_ENDIAN},
            {RegistryValueKind.String, ValueType.REG_SZ},
            {RegistryValueKind.Unknown, ValueType.REG_NONE}
          };

    /// <summary>
    /// Returns the equivalent <see cref="RegistryValueKind"/> for the current <see cref="valueType"/>.
    /// </summary>
    /// <param name="valueType"></param>
    /// <returns></returns>
    public static RegistryValueKind AsValueKind(this ValueType valueType)
    {
      foreach (var pair in _valueMap)
        if (pair.Value == valueType)
          return pair.Key;
      return RegistryValueKind.Unknown;
    }

    /// <summary>
    /// Returns the equivalent <see cref="ValueType"/> for the current <see cref="RegistryValueKind"/>.
    /// </summary>
    /// <param name="valueKind"></param>
    /// <returns></returns>
    public static ValueType AsValueType(this RegistryValueKind valueKind)
    {
      return _valueMap[valueKind];
    }

  }

}
