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

using System.Collections.Generic;

namespace System.Reflection.GAC
{
  internal static class InsuranceBaseExtensions
  {

    public static InsuranceBase FindElement(this IEnumerable<InsuranceBase> items, string identifier)
    {
      foreach (var item in items)
        if (item.InsuranceIdentifier == identifier)
          return item;
      return null;
    }

    public static InsuranceFile FindElement(this IEnumerable<InsuranceFile> items, string identifier)
    {
      foreach (var item in items)
        if (item.InsuranceIdentifier == identifier)
          return item;
      return null;
    }

    public static InsuranceRegistryKey FindElement(this IEnumerable<InsuranceRegistryKey> items, string identifier)
    {
      foreach (var item in items)
        if (item.InsuranceIdentifier == identifier)
          return item;
      return null;
    }
  }
}
