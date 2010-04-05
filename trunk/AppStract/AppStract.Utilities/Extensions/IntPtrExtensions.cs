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
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace AppStract.Utilities.Extensions
{
  public static class IntPtrExtensions
  {

    #region Object.ToByteArray()

    /// <summary>
    /// Convert an object to a byte array.
    /// </summary>
    /// <param name="o">Object to convert to byte array.</param>
    /// <returns></returns>
    public static byte[] ToByteArray(this object o)
    {
      if (o == null)
        throw new NullReferenceException();
      var oType = o.GetType();
      if (oType.IsEnum)
        oType = Enum.GetUnderlyingType(oType);
      if (oType == typeof (byte[]))
        return (byte[]) o;
      if (oType == typeof (string))
        return o.ToString().ToByteArray();
      if (oType == typeof (char))
        return ((char) o).ToByteArray();
      if (oType == typeof (Int16) || oType == typeof (UInt16))
        return ((Int16) o).ToByteArray();
      if (oType == typeof (Int32) || oType == typeof (UInt32))
        return ((Int32) o).ToByteArray();
      if (oType == typeof (Int64) || oType == typeof (UInt64))
        return ((Int64) o).ToByteArray();
      if (oType == typeof (float))
        return ((float) o).ToByteArray();
      if (oType == typeof (double))
        return ((double) o).ToByteArray();
      if (!oType.IsSerializable)
        throw new NotSupportedException("\"" + oType + "\" is not supported.");
      var ms = new MemoryStream();
      var bf1 = new BinaryFormatter();
      bf1.Serialize(ms, o);
      return ms.ToArray();
    }

    public static byte[] ToByteArray(this string o)
    {
      return new System.Text.ASCIIEncoding().GetBytes(o);
    }

    public static byte[] ToByteArray(this char value)
    {
      return BitConverter.GetBytes(value);
    }

    public static byte[] ToByteArray(this Int16 o)
    {
      return BitConverter.GetBytes(o);
    }

    public static byte[] ToByteArray(this Int32 o)
    {
      return BitConverter.GetBytes(o);
    }

    public static byte[] ToByteArray(this Int64 o)
    {
      return BitConverter.GetBytes(o);
    }

    public static byte[] ToByteArray(this float o)
    {
      return BitConverter.GetBytes(o);
    }

    public static byte[] ToByteArray(this double o)
    {
      return BitConverter.GetBytes(o);
    }

    #endregion

    #region Object.ToPointer()

    /// <summary>
    /// Returns a pointer for the current <see cref="object"/>.
    /// Always call <see cref="Marshal.FreeHGlobal"/> after usage, to free the allocated memory.
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
    public static IntPtr ToPointer(this object o)
    {
      int tmp;
      return o.ToPointer(out tmp);
    }

    /// <summary>
    /// Returns a pointer for the current <see cref="object"/>.
    /// Always call <see cref="Marshal.FreeHGlobal"/> after usage, to free the allocated memory.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="allocatedBytes"></param>
    /// <returns></returns>
    public static IntPtr ToPointer(this object o, out int allocatedBytes)
    {
      if (o == null)
        throw new NullReferenceException();
      allocatedBytes = 0;
      var oType = o.GetType();
      if (oType.IsEnum)
        oType = Enum.GetUnderlyingType(oType);
      if (oType == typeof (byte[]))
        return ((byte[]) o).ToPointer(out allocatedBytes);
      if (oType == typeof (string))
        return o.ToString().ToPointer(out allocatedBytes);
      if (oType == typeof (char))
        return ((char) o).ToPointer(out allocatedBytes);
      if (oType == typeof (byte) || oType == typeof (sbyte))
        return ((byte) o).ToPointer(out allocatedBytes);
      if (oType == typeof (Int16) || oType == typeof (UInt16))
        return ((Int16) o).ToPointer(out allocatedBytes);
      if (oType == typeof (Int32) || oType == typeof (UInt32))
        return ((Int32) o).ToPointer(out allocatedBytes);
      if (oType == typeof (Int64) || oType == typeof (UInt64))
        return ((Int64) o).ToPointer(out allocatedBytes);
      if (oType == typeof (float))
        return ((float) o).ToPointer(out allocatedBytes);
      if (oType == typeof (double))
        return ((double) o).ToPointer(out allocatedBytes);
      if (!oType.IsSequential())
        throw new NotSupportedException("\"" + oType + ".ToPointer()\" is not supported.");
      allocatedBytes = Marshal.SizeOf(o);
      var result = Marshal.AllocHGlobal(allocatedBytes);
      try
      {
        Marshal.StructureToPtr(o, result, false);
      }
      catch
      {
        allocatedBytes = 0;
        Marshal.FreeHGlobal(result);
        throw;
      }
      return result;
    }

    public static IntPtr ToPointer(this byte[] value, out int allocatedBytes)
    {
      if (value == null)
        throw new NullReferenceException();
      allocatedBytes = value.Length;
      var ptr = Marshal.AllocHGlobal(allocatedBytes);
      try
      {
        Marshal.Copy(value, 0, ptr, allocatedBytes);
      }
      catch
      {
        allocatedBytes = 0;
        Marshal.FreeHGlobal(ptr);
        throw;
      }
      return ptr;
    }

    public static IntPtr ToPointer(this string value, out int allocatedBytes)
    {
      if (value == null)
        throw new NullReferenceException();
      if (!string.IsNullOrEmpty(value) && !value.EndsWith("\0"))
        value += '\0';  // End string with a null character to ensure compatibility
      allocatedBytes = value.Length * 2; // A character is 2 bytes in .NET
      try
      {
        return Marshal.StringToHGlobalUni(value);
      }
      catch
      {
        allocatedBytes = 0;
        throw;
      }
    }

    public static IntPtr ToPointer(this char value, out int allocatedBytes)
    {
      return ((Int16)value).ToPointer(out allocatedBytes);
    }

    public static IntPtr ToPointer(this byte value, out int allocatedBytes)
    {
      allocatedBytes = 1;
      var ptr = Marshal.AllocHGlobal(allocatedBytes);
      try
      {
        Marshal.WriteByte(ptr, value);
      }
      catch
      {
        allocatedBytes = 0;
        Marshal.FreeHGlobal(ptr);
        throw;
      }
      return ptr;
    }

    public static IntPtr ToPointer(this Int16 value, out int allocatedBytes)
    {
      allocatedBytes = 2;
      var ptr = Marshal.AllocHGlobal(allocatedBytes);
      try
      {
        Marshal.WriteInt16(ptr, value);
      }
      catch
      {
        allocatedBytes = 0;
        Marshal.FreeHGlobal(ptr);
        throw;
      }
      return ptr;
    }

    public static IntPtr ToPointer(this Int32 value, out int allocatedBytes)
    {
      allocatedBytes = 4;
      var ptr = Marshal.AllocHGlobal(allocatedBytes);
      try
      {
        Marshal.WriteInt32(ptr, value);
      }
      catch
      {
        allocatedBytes = 0;
        Marshal.FreeHGlobal(ptr);
        throw;
      }
      return ptr;
    }

    public static IntPtr ToPointer(this Int64 value, out int allocatedBytes)
    {
      allocatedBytes = 8;
      var ptr = Marshal.AllocHGlobal(allocatedBytes);
      try
      {
        Marshal.WriteInt64(ptr, value);
      }
      catch
      {
        allocatedBytes = 0;
        Marshal.FreeHGlobal(ptr);
        throw;
      }
      return ptr;
    }

    public static IntPtr ToPointer(this float value, out int allocatedBytes)
    {
      return (new Union32 {Float = value}).Integer.ToPointer(out allocatedBytes);
    }

    public static IntPtr ToPointer(this double value, out int allocatedBytes)
    {
      return (new Union64 { Double = value }).Integer.ToPointer(out allocatedBytes);
    }

    #endregion

    #region IntPtr.Write()

    /// <summary>
    /// Writes the specified <paramref name="data"/> to the current <see cref="IntPtr"/>.
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static void Write(this IntPtr ptr, object data)
    {
      if (ptr == IntPtr.Zero)
        throw new NullReferenceException();
      if (data == null)
        throw new ArgumentNullException("data");
      var oType = data.GetType();
      if (oType.IsEnum)
        oType = Enum.GetUnderlyingType(oType);
      if (oType == typeof(byte[]))
        ptr.Write((byte[])data);
      else if (oType == typeof(string))
        ptr.Write(data.ToString());
      else if (oType == typeof(char))
        ptr.Write((char)data);
      else if (oType == typeof(byte) || oType == typeof(sbyte))
        ptr.Write((byte) data);
      else if (oType == typeof(Int16) || oType == typeof(UInt16))
        ptr.Write((Int16)data);
      else if (oType == typeof(Int32) || oType == typeof(UInt32))
        ptr.Write((Int32)data);
      else if (oType == typeof(Int64) || oType == typeof(UInt64))
        ptr.Write((Int64)data);
      else if (oType == typeof(float))
        ptr.Write((float)data);
      else if (oType == typeof(double))
        ptr.Write((double)data);
      else if (oType.IsSequential())
        Marshal.StructureToPtr(data, ptr, false);
      else
        throw new NotSupportedException("\"" + oType + "\" is not supported.");
    }

    public static void Write(this IntPtr ptr, byte[] data)
    {
      Marshal.Copy(data, 0, ptr, data.Length);
    }

    public static void Write(this IntPtr ptr, string data)
    {
      var binary = new System.Text.ASCIIEncoding().GetBytes(data);
      ptr.Write(binary);
    }

    public static void Write(this IntPtr ptr, char data)
    {
      ptr.Write((Int16) data);
    }

    public static void Write(this IntPtr ptr, byte data)
    {
      Marshal.WriteByte(ptr, data);
    }

    public static void Write(this IntPtr ptr, Int16 data)
    {
      Marshal.WriteInt16(ptr, data);
    }

    public static void Write(this IntPtr ptr, Int32 data)
    {
      Marshal.WriteInt32(ptr, data);
    }

    public static void Write(this IntPtr ptr, Int64 data)
    {
      Marshal.WriteInt64(ptr, data);
    }

    public static void Write(this IntPtr ptr, float data)
    {
      ptr.Write(new Union32 { Float = data }.Integer);
    }

    public static void Write(this IntPtr ptr, double data)
    {
      ptr.Write(new Union64 { Double = data }.Integer);
    }

    #endregion

    #region IntPtr.Read()

    /// <summary>
    /// Reads the data allocated at the current <see cref="IntPtr"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="ptr"></param>
    /// <returns></returns>
    public static T Read<T>(this IntPtr ptr)
    {
      return ptr.Read<T>(0);
    }

    /// <summary>
    /// Reads the data allocated at the current <see cref="IntPtr"/>.
    /// </summary>
    /// <typeparam name="T"><see cref="Type"/> of the data located at the current <see cref="IntPtr"/>.</typeparam>
    /// <param name="ptr"></param>
    /// <param name="pointerSize">Size of the data, in bytes.</param>
    /// <returns></returns>
    public static T Read<T>(this IntPtr ptr, uint pointerSize)
    {
      var oType = typeof (T);
      var o = ptr.Read(oType, pointerSize);
      if (o == null || o.GetType() == oType
          || (oType.IsEnum && o.GetType() == Enum.GetUnderlyingType(oType)))
        return (T) o;
      throw new InvalidCastException("An internal error occured when casting the result to an object of type \"" + oType +
                                     "\"");
    }

    /// <summary>
    /// Reads the data allocated at the current <see cref="IntPtr"/>.
    /// </summary>
    /// <param name="ptr"></param>
    /// <param name="objectType"><see cref="Type"/> of the data located at the current <see cref="IntPtr"/>.</param>
    /// <param name="pointerSize">Size of the data, in bytes.</param>
    /// <returns></returns>
    public static object Read(this IntPtr ptr, Type objectType, uint pointerSize)
    {
      if (ptr == IntPtr.Zero)
        throw new NullReferenceException("Can't marshal data from a zero-pointer.");
      if (objectType == null)
        throw new ArgumentNullException("objectType");
      if (objectType.IsEnum)
        objectType = Enum.GetUnderlyingType(objectType);
      if (objectType == typeof(string))
        return pointerSize == 0
                 ? Marshal.PtrToStringAuto(ptr)
                 : Marshal.PtrToStringAuto(ptr, (int) pointerSize/2);
      if (objectType == typeof (Int16))
        return Marshal.ReadInt16(ptr);
      if (objectType == typeof (Int32))
        return Marshal.ReadInt32(ptr);
      if (objectType == typeof (Int64))
        return Marshal.ReadInt64(ptr);
      if (objectType == typeof(float))
        return (new Union32 { Integer = Marshal.ReadInt32(ptr) }).Float;
      if (objectType == typeof(double))
        return (new Union64 { Integer = Marshal.ReadInt64(ptr) }).Double;
      if (objectType == typeof (byte[]))
      {
        var result = new byte[pointerSize];
        for (var i = 0; i < pointerSize; i++)
          result[i] = Marshal.ReadByte(ptr, i);
        return result;
      }
      if (objectType.IsSequential())
        return Marshal.PtrToStructure(ptr, objectType);
      if (pointerSize == 0)
        return null;
      throw new NotSupportedException("Marshaling an instance of type \"" + objectType + "\" is not supported.");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns whether <see cref="Marshal.PtrToStructure(IntPtr,Type)"/> can be used with the current <see cref="Type"/>.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsSequential(this Type type)
    {
      return (!type.IsGenericType
              && ((type.Attributes & TypeAttributes.SequentialLayout) == TypeAttributes.SequentialLayout
                  || (type.Attributes & TypeAttributes.ExplicitLayout) == TypeAttributes.ExplicitLayout));
    }

    #endregion

    #region Private Types

    /// <summary>
    /// Represents an union for 32-bit types.
    /// All fields in this union have the exact same binary value assigned to them.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size=4)]
    private struct Union32
    {
      [FieldOffset(0)]
      public Int32 Integer;
      [FieldOffset(0)]
      public UInt32 UnsignedInteger;
      [FieldOffset(0)]
      public float Float;
    }

    /// <summary>
    /// Represents an union for 64-bit types.
    /// All fields in this union have the exact same binary value assigned to them.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    private struct Union64
    {
      [FieldOffset(0)]
      public Int64 Integer;
      [FieldOffset(0)]
      public UInt64 UnsignedInteger;
      [FieldOffset(0)]
      public double Double;
    }

    #endregion

  }
}
