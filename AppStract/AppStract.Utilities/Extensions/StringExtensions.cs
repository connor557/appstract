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
using System.Collections.Generic;
using System.Linq;

namespace AppStract.Utilities.Extensions
{
  public static class StringExtensions
  {

    /// <summary>
    /// Determines whether the beginning of this instance matches any of the specified values.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The values enumerable can't contain null or empty strings.
    /// </exception>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <returns></returns>
    public static bool StartsWithAny(this string value, IEnumerable<string> values)
    {
      string tmp;
      return value.StartsWithAny(values, out tmp, false);
    }

    /// <summary>
    /// Determines whether the beginning of this instance matches any of the specified values.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The values enumerable can't contain null or empty strings.
    /// </exception>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <param name="matchingValue">The matching value, if any.</param>
    /// <returns></returns>
    public static bool StartsWithAny(this string value, IEnumerable<string> values, out string matchingValue)
    {
      return value.StartsWithAny(values, out matchingValue, false);
    }

    /// <summary>
    /// Determines whether the beginning of this instance matches any of the specified values.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The values enumerable can't contain null or empty strings.
    /// </exception>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <param name="matchingValue">The matching value, if any.</param>
    /// <param name="ignoreCase">True to ignore case when comparing; otherwise, false.</param>
    /// <returns></returns>
    public static bool StartsWithAny(this string value, IEnumerable<string> values, out string matchingValue, bool ignoreCase)
    {
      List<string> originalValues = new List<string>(values);
      List<string> comparableValues = originalValues;
      if (ignoreCase)
      {
        value = value.ToUpperInvariant();
        comparableValues = new List<string>(values.ToUpperInvariant());
      }
      for (int i = 0; i < originalValues.Count; i++)
      {
        if (string.IsNullOrEmpty(originalValues[i]))
          throw new ArgumentException("The values enumerable can't contain null or empty strings.", "values");
        if (value.StartsWith(comparableValues[i]))
        {
          matchingValue = originalValues[i];
          return true;
        }
      }
      matchingValue = null;
      return false;
    }

    /// <summary>
    /// Determines whether the ending of this instance matches any of the specified values.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The values enumerable can't contain null or empty strings.
    /// </exception>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <returns></returns>
    public static bool EndsWithAny(this string value, IEnumerable<string> values)
    {
      string tmp;
      return value.EndsWithAny(values, out tmp, false);
    }

    /// <summary>
    /// Determines whether the ending of this instance matches any of the specified values.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The values enumerable can't contain null or empty strings.
    /// </exception>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <param name="matchingValue">The matching value, if any.</param>
    /// <returns></returns>
    public static bool EndsWithAny(this string value, IEnumerable<string> values, out string matchingValue)
    {
      return value.EndsWithAny(values, out matchingValue, false);
    }

    /// <summary>
    /// Determines whether the ending of this instance matches any of the specified values.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// The values enumerable can't contain null or empty strings.
    /// </exception>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <param name="matchingValue">The matching value, if any.</param>
    /// <param name="ignoreCase">True to ignore case when comparing; otherwise, false.</param>
    /// <returns></returns>
    public static bool EndsWithAny(this string value, IEnumerable<string> values, out string matchingValue, bool ignoreCase)
    {
      List<string> originalValues = new List<string>(values);
      List<string> comparableValues = originalValues;
      if (ignoreCase)
      {
        value = value.ToUpperInvariant();
        comparableValues = new List<string>(values.ToUpperInvariant());
      }
      for (int i = 0; i < originalValues.Count; i++)
      {
        if (string.IsNullOrEmpty(originalValues[i]))
          throw new ArgumentException("The values enumerable can't contain null or empty strings.", "values");
        if (value.EndsWith(comparableValues[i]))
        {
          matchingValue = originalValues[i];
          return true;
        }
      }
      matchingValue = null;
      return false;
    }

    /// <summary>
    /// Returns whether this string equals any of the given values.
    /// </summary>
    /// <param name="value">The current string.</param>
    /// <param name="values">The strings to compare.</param>
    /// <returns></returns>
    public static bool EqualsAny(this string value, IEnumerable<string> values)
    {
      return EqualsAny(value, values, false);
    }

    /// <summary>
    /// Returns whether this string equals any of the given values.
    /// </summary>
    /// <param name="value">The current string.</param>
    /// <param name="values">The strings to compare.</param>
    /// <param name="ignoreCase">True to ignore casing when comparing; otherwise, false.</param>
    /// <returns></returns>
    public static bool EqualsAny(this string value, IEnumerable<string> values, bool ignoreCase)
    {
      if (ignoreCase)
      {
        value = value.ToUpperInvariant();
        values = new List<string>(values.ToUpperInvariant());
      }
      foreach (var v in values)
        if (value == v)
          return true;
      return false;
    }

    /// <summary>
    /// Returns whether or not the string is composed of the given characters, and thereby contains only these characters.
    /// </summary>
    /// <param name="value">The current string.</param>
    /// <param name="characters">The characters of which this string should be composed.</param>
    /// <returns>True if the string only contains the given characters; False if other characters are found.</returns>
    public static bool IsComposedOf(this string value, IEnumerable<char> characters)
    {
      return IsComposedOf(value, characters, false);
    }

    /// <summary>
    /// Returns whether or not the string is composed of the given characters, and thereby contains only these characters.
    /// </summary>
    /// <param name="value">The current string.</param>
    /// <param name="characters">The characters of which this string should be composed.</param>
    /// <param name="ignoreCase">True to ignore casing when comparing; otherwise, false.</param>
    /// <returns>True if the string only contains the given characters; False if other characters are found.</returns>
    public static bool IsComposedOf(this string value, IEnumerable<char> characters, bool ignoreCase)
    {
      if (ignoreCase)
      {
        value = value.ToUpperInvariant();
        characters = new List<char>(characters.ToUpperInvariant());
      }
      foreach (var c in value)
        if (!characters.Contains(c))
          return false;
      return true;
    }

    /// <summary>
    /// Yield returns the upper cased <see cref="string"/> values contained in the current <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<string> ToUpperInvariant(this IEnumerable<string> value)
    {
      foreach (var val in value)
        yield return val == null ? val : val.ToUpperInvariant();
    }

    /// <summary>
    /// Yield returns the upper cased <see cref="char"/> values contained in the current <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<char> ToUpperInvariant(this IEnumerable<char> value)
    {
      foreach (var val in value)
        yield return char.ToUpperInvariant(val);
    }

  }
}
