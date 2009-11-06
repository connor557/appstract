#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AppStract.Utilities.Extensions
{
  public static class IntPtrExtensions
  {

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
      if (oType.IsSequential())
      {
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
      if (oType == typeof (byte[]))
        return ((byte[]) o).ToPointer(out allocatedBytes);
      if (oType == typeof (string))
        return o.ToString().ToPointer(out allocatedBytes);
      if (oType == typeof(char))
        return ((char)o).ToPointer(out allocatedBytes);
      if (oType == typeof(Int16) || oType == typeof(UInt16))
        return ((Int16)o).ToPointer(out allocatedBytes);
      if (oType == typeof(Int32) || oType == typeof(UInt32))
        return ((Int32)o).ToPointer(out allocatedBytes);
      if (oType == typeof(Int64) || oType == typeof(UInt64))
        return ((Int64)o).ToPointer(out allocatedBytes);
      if (oType == typeof(float))
        return ((float)o).ToPointer(out allocatedBytes);
      if (oType == typeof(double))
        return ((double)o).ToPointer(out allocatedBytes);
      throw new NotSupportedException("\"" + oType + ".ToPointer()\" is not supported.");
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

    public static IntPtr ToPointer(this char value, out int allocatedBytes)
    {
      return ((Int16)value).ToPointer(out allocatedBytes);
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
    /// <typeparam name="T"></typeparam>
    /// <param name="ptr"></param>
    /// <param name="bufferSize"></param>
    /// <returns></returns>
    public static T Read<T>(this IntPtr ptr, uint bufferSize)
    {
      var oType = typeof (T);
      var o = ptr.Read(oType, bufferSize);
      if (o == null || o.GetType() == oType)
        return (T) o;
      throw new InvalidCastException("An internal error occured when casting the result to an object of type \"" + oType + "\"");
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
      if (objectType.IsSequential())
        return Marshal.PtrToStructure(ptr, objectType);
      if (objectType == typeof (string))
        return Marshal.PtrToStringAuto(ptr);
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
      // For all following types the number of allocated bytes needs to be specified.
      if (pointerSize == 0)
        return null;
      throw new NotSupportedException("Marshaling an instance of \"" + objectType + "\" is not implemented.");
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
              && ((type.Attributes & TypeAttributes.SequentialLayout) != 0
                  || (type.Attributes & TypeAttributes.ExplicitLayout) != 0));
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
