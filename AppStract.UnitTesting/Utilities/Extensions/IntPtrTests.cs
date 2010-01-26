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
using System.Linq;
using System.Runtime.InteropServices;
using AppStract.Utilities.Extensions;
using NUnit.Framework;

namespace AppStract.UnitTesting.Utilities.Extensions
{
  [TestFixture]
  public class IntPtrTests
  {

    #region Classes/Structs

    private class TestClass
    {
      string MyString { get; set; }
      byte MyByte { get; set; }
      byte[] MyByteArray { get; set; }
      decimal MyDecimal { get; set; }
      decimal[] MyDecimalArray { get; set; }

      public void SetDefaultValues()
      {
        MyByte = 201;
        MyByteArray = new byte[] {200, 137, 29, 5};
        MyDecimal = decimal.MaxValue - 100054;
        MyDecimalArray = new[] {decimal.MaxValue, decimal.MinValue, decimal.MinusOne, decimal.One};
        MyString = "SomeRandomStringValue0153#}^@éµù°-*|";
      }

      public bool Compare(TestClass other)
      {
        return MyByte == other.MyByte
               && MyDecimal == other.MyDecimal
               && MyString == other.MyString
               && MyByteArray.All(e => other.MyByteArray.Any(oe => oe == e))
               && MyDecimalArray.All(e => other.MyDecimalArray.Any(oe => oe == e));
      }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    private class SequentialTestClass
    {
      string MyString { get; set; }
      byte MyByte { get; set; }
      byte[] MyByteArray { get; set; }
      decimal MyDecimal { get; set; }
      decimal[] MyDecimalArray { get; set; }

      public void SetDefaultValues()
      {
        MyByte = 201;
        MyByteArray = new byte[] {200, 137, 29, 5};
        MyDecimal = decimal.MaxValue - 100054;
        MyDecimalArray = new[] {decimal.MaxValue, decimal.MinValue, decimal.MinusOne, decimal.One};
        MyString = "SomeRandomStringValue0153#}^@éµù°-*|";
      }

      public bool Compare(SequentialTestClass other)
      {
        return MyByte == other.MyByte
               && MyDecimal == other.MyDecimal
               && MyString == other.MyString
               && MyByteArray.All(e => other.MyByteArray.Any(oe => oe == e))
               && MyDecimalArray.All(e => other.MyDecimalArray.Any(oe => oe == e));
      }
    }

    private struct TestStruct
    {
      string MyString { get; set; }
      byte MyByte { get; set; }
      byte[] MyByteArray { get; set; }
      decimal MyDecimal { get; set; }
      decimal[] MyDecimalArray { get; set; }

      public void SetDefaultValues()
      {
        MyByte = 201;
        MyByteArray = new byte[] {200, 137, 29, 5};
        MyDecimal = decimal.MaxValue - 100054;
        MyDecimalArray = new[] {decimal.MaxValue, decimal.MinValue, decimal.MinusOne, decimal.One};
        MyString = "SomeRandomStringValue0153#}^@éµù°-*|";
      }

      public bool Compare(TestStruct other)
      {
        return MyByte == other.MyByte
               && MyDecimal == other.MyDecimal
               && MyString == other.MyString
               && MyByteArray.All(e => other.MyByteArray.Any(oe => oe == e))
               && MyDecimalArray.All(e => other.MyDecimalArray.Any(oe => oe == e));
      }
    }

    #endregion

    #region Class/Struct Tests

    [Test]
    [ExpectedException(typeof (NotSupportedException))]
    public void ClassTest()
    {
      var test = new TestClass();
      test.SetDefaultValues();
      int allocatedBytes;
      var result = test.ToPointer(out allocatedBytes).Read<TestClass>((uint) allocatedBytes);
      Assert.IsTrue(test.Compare(result));
    }

    [Test]
    public void SeqClassTest()
    {
      var test = new SequentialTestClass();
      test.SetDefaultValues();
      int allocatedBytes;
      var result = test.ToPointer(out allocatedBytes).Read<SequentialTestClass>((uint) allocatedBytes);
      Assert.IsTrue(test.Compare(result));
    }

    [Test]
    public void StructTest()
    {
      var test = new TestStruct();
      test.SetDefaultValues();
      int allocatedBytes;
      var result = test.ToPointer(out allocatedBytes).Read<TestStruct>((uint) allocatedBytes);
      Assert.IsTrue(test.Compare(result));
    }

    #endregion

    #region Primitive Types Tests

    [Test]
    public void StringTest()
    {
      object test = "someStringValue";
      var result = test.ToPointer().Read<string>();
      Assert.IsTrue((string)test == result);
    }

    [Test]
    public void ByteTest()
    {
      object test = byte.MaxValue;
      var result = test.ToPointer().Read<byte>();
      Assert.IsTrue((byte)test == result);
    }

    [Test]
    public void ByteArrayTest()
    {
      object test = new byte[] {byte.MinValue, 138, byte.MaxValue};
      int allocBytes;
      var result = test.ToPointer(out allocBytes).Read<byte[]>((uint) allocBytes);
      Assert.IsTrue(((byte[])test).All(e => result.Any(oe => oe == e)));
    }

    [Test]
    public void CharTest()
    {
      object test = 'x';
      var result = test.ToPointer().Read<char>();
      Assert.IsTrue((char)test == result);
    }

    [Test]
    public void Int16Test()
    {
      object test = Int16.MaxValue;
      var result = test.ToPointer().Read<Int16>();
      Assert.IsTrue((Int16)test == result);
    }

    [Test]
    public void UInt16Test()
    {
      object test = UInt16.MaxValue;
      var result = test.ToPointer().Read<UInt16>();
      Assert.IsTrue((UInt16)test == result);
    }

    [Test]
    public void Int32Test()
    {
      object test = Int32.MaxValue;
      var result = test.ToPointer().Read<Int32>();
      Assert.IsTrue((Int32) test == result);
    }

    [Test]
    public void UInt32Test()
    {
      object test = UInt32.MaxValue;
      var result = test.ToPointer().Read<UInt32>();
      Assert.IsTrue((UInt32)test == result);
    }

    [Test]
    public void Int64Test()
    {
      object test = Int64.MaxValue;
      var result = test.ToPointer().Read<Int64>();
      Assert.IsTrue((Int64)test == result);
    }

    [Test]
    public void UInt64Test()
    {
      object test = UInt64.MaxValue;
      var result = test.ToPointer().Read<UInt64>();
      Assert.IsTrue((UInt64)test == result);
    }

    [Test]
    public void FloatTest()
    {
      object test = float.MaxValue;
      var result = test.ToPointer().Read<float>();
      Assert.IsTrue((float)test == result);
    }

    [Test]
    public void DoubleTest()
    {
      object test = double.MaxValue;
      var result = test.ToPointer().Read<double>();
      Assert.IsTrue((double)test == result);
    }

    #endregion

  }
}