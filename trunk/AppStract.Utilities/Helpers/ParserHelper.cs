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

namespace AppStract.Utilities.Helpers
{
  /// <summary>
  /// Helper class for generic parser functions.
  /// </summary>
  public static class ParserHelper
  {

    #region Public Methods

    /// <summary>
    /// Tries to parse an object to a value of the specified <typeparamref name="EnumType"/>.
    /// </summary>
    /// <typeparam name="EnumType">The type of enumeration to parse to.</typeparam>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParseEnum<EnumType>(object value, out EnumType result)
    {
      result = default(EnumType);
      if (value == null) return false;
      if (!typeof(EnumType).IsEnum) return false;
      int iValue;
      return Int32.TryParse(value.ToString(), out iValue)
               ? TryParseEnum(iValue, out result)
               : TryParseEnum(value.ToString(), out result);
    }

    /// <summary>
    /// Tries to parse an integer to a value of the specified <typeparamref name="EnumType"/>.
    /// </summary>
    /// <typeparam name="EnumType">The type of enumeration to parse to.</typeparam>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParseEnum<EnumType>(int value, out EnumType result)
    {
      result = default(EnumType);
      var type = typeof(EnumType);
      if (!type.IsEnum) return false;
      result = (EnumType)Enum.Parse(type, value.ToString());
      // Verify if the result is valid,
      // Enum.Parse might just return the input value, while this is not a defined value of the enum
      if (result.ToString().Contains(", ") || Enum.IsDefined(type, result))
        return true;
      result = default(EnumType);
      return false;
    }

    /// <summary>
    /// Tries to parse a string to a value of the specified <typeparamref name="EnumType"/>.
    /// </summary>
    /// <typeparam name="EnumType">The type of enumeration to parse to.</typeparam>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParseEnum<EnumType>(string value, out EnumType result)
    {
      result = default(EnumType);
      if (string.IsNullOrEmpty(value)) return false;
      var type = typeof(EnumType);
      if (!type.IsEnum) return false;
      value = value.ToUpperInvariant();
      try
      {
        result = (EnumType)Enum.Parse(type, value, true);
        return true;
      }
      catch (ArgumentException)
      {
        return false;
      }
    }

    #endregion

  }
}
