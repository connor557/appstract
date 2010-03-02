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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32.Interop;

namespace AppStract.Utilities.Interop
{
  /// <summary>
  /// Helper class for actions related to marshalling.
  /// </summary>
  public static class MarshallingHelpers
  {

    #region Public Methods

    /// <summary>
    /// This method will marshal an object from the pointer specified.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The length of the data must fit in a signed integer to be compatible with .NET Marshalling.
    /// </exception>
    /// <param name="source">Pointer to get the object from.</param>
    /// <param name="dataLength">The number of bytes to copy.</param>
    public static object CopyFromMemory(IntPtr source, uint dataLength)
    {
      if (dataLength > int.MaxValue)
        throw new ArgumentOutOfRangeException("dataLength",
          "The length of the data must fit in a signed integer to be compatible with .NET Marshalling");
      int length = (int)dataLength;
      byte[] data = new byte[length];
      Marshal.Copy(source, data, 0, length);
      return FromByteArray(data);
    }

    /// <summary>
    /// This method will marshal an object to the pointer passed.
    /// The amount of available memory at the <see cref="IntPtr"/> is not checked when marshaling.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// </exception>
    /// <param name="data">The data to marshal</param>
    /// <param name="lpData">
    /// A pointer to a buffer that receives the value's data.
    /// This parameter can be <see cref="IntPtr.Zero"/> if the data is not required.
    /// </param>
    /// <returns>A WinError code, can be ERROR_SUCCESS or ERROR_MORE_DATA</returns>
    public static uint CopyToMemory(object data, IntPtr lpData)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      if (lpData != IntPtr.Zero)
      {
        byte[] bData = ToByteArray(data);
        CopyToMemory(bData, lpData);
      }
      return WinError.ERROR_SUCCESS;
    }

    /// <summary>
    /// This method will marshal an object to the pointer passed.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="data"/> parameter is null.
    /// =OR=
    /// The argument <paramref name="lpcbData"/> can only be null if <paramref name="lpData"/> is also null.
    /// </exception>
    /// <param name="data">The data to marshal</param>
    /// <param name="lpData">
    /// A pointer to a buffer that receives the value's data.
    /// This parameter can be <see cref="IntPtr.Zero"/> if the data is not required.
    /// </param>
    /// <param name="lpcbData">
    /// A pointer to a variable that specifies the size of the buffer pointed to by the lpData parameter,
    /// in bytes. When the function returns, this variable contains the size of the data copied to lpData.
    /// The lpcbData parameter can be NULL only if lpData is <see cref="IntPtr.Zero"/>.
    /// </param>
    /// <returns>A WinError code, can be ERROR_SUCCESS or ERROR_MORE_DATA</returns>
    public static uint CopyToMemory(object data, IntPtr lpData, ref uint? lpcbData)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      if (lpData == IntPtr.Zero && lpcbData == null)
        return WinError.ERROR_SUCCESS;
      if (lpData != IntPtr.Zero && lpcbData == null)
        throw new ArgumentNullException("lpcbData", "The argument lpcbData can only be null if lpData is also null.");
      uint bufferLength = (uint)lpcbData;
      byte[] bData = ToByteArray(data);
      lpcbData = uint.Parse(bData.LongLength.ToString());
      if (lpcbData > bufferLength)
      {
        /// If the buffer specified by lpData parameter is not large enough to hold the data,
        /// the function returns ERROR_MORE_DATA and stores the required buffer size in the variable
        /// pointed to by lpcbData. In this case, the contents of the lpData buffer are undefined.
        return WinError.ERROR_MORE_DATA;
      }
      if (lpData != IntPtr.Zero)
        CopyToMemory(bData, lpData);
      return WinError.ERROR_SUCCESS;
    }

    /// <summary>
    /// Convert an object to a byte array.
    /// </summary>
    /// <param name="o">Object to convert to byte array.</param>
    /// <returns></returns>
    public static byte[] ToByteArray(object o)
    {
      var ms = new MemoryStream();
      var bf1 = new BinaryFormatter();
      bf1.Serialize(ms, o);
      return ms.ToArray();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// This method will marshal a byte array to the pointer passed.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <param name="value"> Byte array to copy. </param>
    /// <param name="lpData"> Pointer to copy to, can be NULL.</param>
    private static void CopyToMemory(byte[] value, IntPtr lpData)
    {
      Marshal.Copy(value, 0, lpData, value.Length);
    }

    /// <summary>
    /// Convert a by array to an object.
    /// </summary>
    /// <param name="array">Byte array to convert to object.</param>
    /// <returns></returns>
    private static object FromByteArray(byte[] array)
    {
      MemoryStream ms = new MemoryStream(array);
      BinaryFormatter bf1 = new BinaryFormatter();
      ms.Position = 0;
      return bf1.Deserialize(ms);
    }

    #endregion

  }
}
