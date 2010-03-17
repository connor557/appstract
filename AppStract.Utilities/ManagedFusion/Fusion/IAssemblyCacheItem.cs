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

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace AppStract.Utilities.ManagedFusion.Fusion
{
  /// <summary>
  /// Undocumented. Probably only for internal use.
  /// <see cref="IAssemblyCache.CreateAssemblyCacheItem"/>
  /// </summary>
  [ComImport, Guid("9E3AAEB4-D1CD-11D2-BAB9-00C04F8ECEAE"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IAssemblyCacheItem
  {
    /// <summary>
    /// Undocumented.
    /// </summary>
    /// <param name="dwFlags"></param>
    /// <param name="pszStreamName"></param>
    /// <param name="dwFormat"></param>
    /// <param name="dwFormatFlags"></param>
    /// <param name="ppIStream"></param>
    /// <param name="puliMaxSize"></param>
    void CreateStream(
      uint dwFlags,
      [MarshalAs(UnmanagedType.LPWStr)] string pszStreamName,
      uint dwFormat,
      uint dwFormatFlags,
      out IStream ppIStream,
      ref long puliMaxSize);

    /// <summary>
    /// Undocumented.
    /// </summary>
    /// <param name="dwFlags"></param>
    /// <param name="pulDisposition"></param>
    void Commit(
      uint dwFlags,
      out long pulDisposition);

    /// <summary>
    /// Undocumented.
    /// </summary>
    void AbortItem();
  }
}
