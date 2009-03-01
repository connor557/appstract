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
    /// <param name="matchingValue">The matching value, if any.</param>
    /// <returns></returns>
    public static bool StartsWithAny(this string value, IEnumerable<string> values, out string matchingValue)
    {
      matchingValue = null;
      foreach (string v in values)
      {
        if (value.StartsWith(v))
        {
          matchingValue = v;
          return true;
        }
      }
      return false;
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

  }
}
