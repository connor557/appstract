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

using AppStract.Utilities.Extensions;
using NUnit.Framework;

namespace AppStract.UnitTesting.Utilities.Extensions
{
  [TestFixture]
  public class StringTests
  {

    [Test]
    public void StartsWithAnyTest()
    {
      const string value = "myString";
      string tmp;
      Assert.IsTrue(value.StartsWithAny(new[] {"ó", ".", "my"}, out tmp, false) && tmp == "my",
                    "Expected \"myString\" to start with \"my\"");
      Assert.IsFalse(value.StartsWithAny(new[] {"ó", ".", "mY"}, out tmp, false) && tmp == default(string),
                     "Expected no matching start values.");
      Assert.IsTrue(value.StartsWithAny(new[] {"ó", ".", "mY"}, out tmp, true) && tmp == "mY",
                    "Expected \"myString\" to start with \"mY\", being case insensitive");
    }

    [Test]
    public void EndsWithAnyTest()
    {
      const string value = "myString";
      string tmp;
      Assert.IsTrue(value.EndsWithAny(new[] {"ó", ".", "ng"}, out tmp, false) && tmp == "ng",
                    "Expected \"myString\" to end with \"ng\"");
      Assert.IsFalse(value.EndsWithAny(new[] {"ó", ".", "nG"}, out tmp, false) && tmp == default(string),
                     "Expected no matching start values.");
      Assert.IsTrue(value.EndsWithAny(new[] {"ó", ".", "nG"}, out tmp, true) && tmp == "nG",
                    "Expected \"myString\" to end with \"nG\", being case insensitive");
    }

    [Test]
    public void EqualsAnyTest()
    {
      const string value = "myString";
      Assert.IsTrue(value.EqualsAny(new[] {"test", "myString"}, false),
                    "Expected \"myString\" to equal \"myString\"");
      Assert.IsFalse(value.EqualsAny(new[] {"test", "myStrinG"}, false),
                     "Expected \"myString\" not to equal \"myStrinG\"");
      Assert.IsTrue(value.EqualsAny(new[] {"test", "myStrinG"}, true),
                    "Expected \"myString\" to equal \"myStrinG\" being case insensitive");
    }

    [Test]
    public void IsComposedOfTest()
    {
      const string value = "abc";
      Assert.IsTrue(value.IsComposedOf(new[] {'a', 'b', 'c'}, false),
                    "Expected \"abc\" to be composed of 'a' 'b' and 'c'");
      Assert.IsFalse(value.IsComposedOf(new[] {'a', 'B', 'c'}, false),
                     "Expected \"abc\" not to be composed of 'a' 'B' and 'c'");
      Assert.IsTrue(value.IsComposedOf(new[] {'a', 'B', 'c'}, true),
                    "Expected \"abc\" to be composed of 'a' 'B' and 'c' being case insensitive");
    }

  }
}
