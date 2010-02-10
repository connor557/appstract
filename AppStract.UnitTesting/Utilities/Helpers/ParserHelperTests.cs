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
using AppStract.Utilities.Helpers;
using NUnit.Framework;

namespace AppStract.UnitTesting.Utilities.Helpers
{


  internal enum MyEnum { Zero = 0, One = 1, Two = 2, Four = 4, Eight = 8 }
  [Flags]
  internal enum MyFlagsEnum { Zero = 0, One = 1, Two = 2, Four = 4, Eight = 8 }

  /// <summary>
  /// Provides extension methods for <see cref="MyFlagsEnum"/>.
  /// </summary>
  internal static class MyFlagsExtensions
  {
    /// <summary>
    /// Returns whether the <see cref="MyFlagsEnum"/> specified is/are defined in the current <see cref="MyFlagsEnum"/>.
    /// </summary>
    /// <param name="flags"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static bool IsSpecified(this MyFlagsEnum flags, MyFlagsEnum flag)
    {
      return ((flags & flag) == flag);
    }
  }

  [TestFixture]
  public class ParserHelperTests
  {

    public void EnumTest()
    {
      
    }

    [Test]
    public void FlagsIntegerTest()
    {
      MyFlagsEnum result;
      Assert.IsTrue(ParserHelper.TryParseEnum(15, out result),
                    "Expected value 15 to be parsable.");
      foreach (MyFlagsEnum flag in Enum.GetValues(typeof (MyFlagsEnum)))
        Assert.IsTrue(result.IsSpecified(flag), flag + " is not specified in the result.");
    }

    [Test]
    public void FlagsIllegalIntegerTest()
    {
      MyFlagsEnum result;
      Assert.IsFalse(ParserHelper.TryParseEnum(16, out result),
                     "Expected value 16 not to be parsable.");
      foreach (MyFlagsEnum flag in Enum.GetValues(typeof (MyFlagsEnum)))
        if (flag != default(MyFlagsEnum))
          Assert.IsFalse(result.IsSpecified(flag), flag + " is specified in the result.");
    }

    [Test]
    public void FlagsNameTest()
    {
      MyFlagsEnum result;
      string value = "";
      foreach (MyFlagsEnum flag in Enum.GetValues(typeof (MyFlagsEnum)))
        value += flag + ", ";
      value = value.Substring(0, value.Length - 2);
      Assert.IsTrue(ParserHelper.TryParseEnum(value, out result),
                    "Expected string to be parsable.");
      foreach (MyFlagsEnum flag in Enum.GetValues(typeof (MyFlagsEnum)))
        Assert.IsTrue(result.IsSpecified(flag), flag + " is not specified in the result.");
    }

    [Test]
    public void FlagsIllegalNameTest()
    {
      MyFlagsEnum result;
      string value = "";
      foreach (MyFlagsEnum flag in Enum.GetValues(typeof (MyFlagsEnum)))
        value += flag + ", ";
      value += "MyIllegalValue";
      Assert.IsFalse(ParserHelper.TryParseEnum(value, out result),
                     "Expected string not to be parsable.");
      foreach (MyFlagsEnum flag in Enum.GetValues(typeof (MyFlagsEnum)))
        if (flag != default(MyFlagsEnum))
          Assert.IsFalse(result.IsSpecified(flag), flag + " is specified in the result.");
    }

  }

}
