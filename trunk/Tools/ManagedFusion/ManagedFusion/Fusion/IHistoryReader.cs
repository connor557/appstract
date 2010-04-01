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
using System.Text;
using FileTime = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace ManagedFusion.Fusion
{
  /// <summary>
  /// Undocumented interface. Declared for potential future implementations.
  /// </summary>
  /// <remarks>
  /// An instance is retrieved with the CreateHistoryReader function.
  /// See <see cref="http://msdn.microsoft.com/en-us/library/ms231201.aspx"/>,
  /// <see cref="http://www.koders.com/csharp/fid9779AA56CDC41CCC0356DD0D4C5DEAB13F5E2495.aspx?s=button#L165"/>,
  /// <see cref="http://www.koders.com/csharp/fid9779AA56CDC41CCC0356DD0D4C5DEAB13F5E2495.aspx?s=button#L224"/>,
  /// <see cref="http://www.koders.com/csharp/fid9779AA56CDC41CCC0356DD0D4C5DEAB13F5E2495.aspx?s=button#L232"/>.
  /// </remarks>
  [ComImport, Guid("1D23DF4D-A1E2-4B8B-93D6-6EA3DC285A54"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IHistoryReader
  {
    [PreserveSig]
    int GetFilePath(
      [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzFilePath,
      ref uint pdwSize);

    [PreserveSig]
    int GetApplicationName(
      [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzAppName,
      ref uint pdwSize);

    [PreserveSig]
    int GetEXEModulePath(
      [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzExePath,
      ref uint pdwSize);

    void GetNumActivations(
      out uint pdwNumActivations);

    void GetActivationDate(
      uint dwIdx, // One based.
      out FileTime pftDate);

    [PreserveSig]
    int GetRunTimeVersion(
      ref FileTime pftActivationDate,
      [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder wzRunTimeVersion,
      ref uint pdwSize);

    void GetNumAssemblies(
      ref FileTime pftActivationDate,
      out uint pdwNumAsms);

    void GetHistoryAssembly(
      ref FileTime pftActivationDate,
      uint dwIdx, // One based.
      [MarshalAs(UnmanagedType.IUnknown)] out object ppHistAsm);

  }
}
