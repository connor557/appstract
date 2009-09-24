using System.Collections.Generic;

namespace AppStract.Utilities.Extensions
{
  public static class StringExtensions
  {

    /// <summary>
    /// Determines whether the beginning of this instance matches any of the specified values.
    /// </summary>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <returns></returns>
    public static bool StartsWithAny(this string value, IEnumerable<string> values)
    {
      string tmp;
      return value.StartsWithAny(values, out tmp);
    }

    /// <summary>
    /// Determines whether the beginning of this instance matches any of the specified values.
    /// </summary>
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
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <param name="matchingValue">The matching value, if any.</param>
    /// <param name="ignoreCase">True to ignore case when comparing; otherwise, false.</param>
    /// <returns></returns>
    public static bool StartsWithAny(this string value, IEnumerable<string> values, out string matchingValue, bool ignoreCase)
    {
      if (ignoreCase)
        value = value.ToLowerInvariant();
      foreach (string v in values)
      {
        matchingValue = ignoreCase ? v.ToLowerInvariant() : v;
        if (value.StartsWith(matchingValue))
        {
          matchingValue = v;
          return true;
        }
      }
      matchingValue = null;
      return false;
    }

    /// <summary>
    /// Determines whether the ending of this instance matches any of the specified values.
    /// </summary>
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <returns></returns>
    public static bool EndsWithAny(this string value, IEnumerable<string> values)
    {
      string tmp;
      return value.EndsWithAny(values, out tmp);
    }

    /// <summary>
    /// Determines whether the ending of this instance matches any of the specified values.
    /// </summary>
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
    /// <param name="value">This instance.</param>
    /// <param name="values">The strings to compare.</param>
    /// <param name="matchingValue">The matching value, if any.</param>
    /// <param name="ignoreCase">True to ignore case when comparing; otherwise, false.</param>
    /// <returns></returns>
    public static bool EndsWithAny(this string value, IEnumerable<string> values, out string matchingValue, bool ignoreCase)
    {
      if (ignoreCase)
        value = value.ToLowerInvariant();
      foreach (string v in values)
      {
        matchingValue = ignoreCase ? v.ToLowerInvariant() : v;
        if (value.EndsWith(matchingValue))
        {
          matchingValue = v;
          return true;
        }
      }
      matchingValue = null;
      return false;
    }

  }
}
