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

namespace System.Runtime.Interop.Fusion
{
  /// <summary>
  /// The <see cref="AssemblyInfo"/> structure contains information about an assembly in the side-by-side assembly store.
  /// The information is used by the <see cref="IAssemblyCache.QueryAssemblyInfo"/> method.
  /// </summary>
  /// <remarks>
  /// Native name: ASSEMBLY_INFO
  /// </remarks>
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct AssemblyInfo
  {

    /// <summary>
    /// Size of the structure in bytes. Permits additions to the structure in future version of the .NET Framework.
    /// </summary>
    public uint cbAssemblyInfo;
    /// <summary>
    /// The flags set when querying info using <see cref="IAssemblyCache.QueryAssemblyInfo"/>.
    /// </summary>
    public AssemblyInfoFlags assemblyFlags;
    /// <summary>
    /// The size of the files that make up the assembly in kilobytes (KB).
    /// </summary>
    public ulong assemblySize;
    /// <summary>
    /// The current path of the directory that contains the files that make up the assembly. The path must end with a zero.
    /// </summary>
    [MarshalAs(UnmanagedType.LPWStr)] public string currentAssemblyPath;
    /// <summary>
    /// Size of the buffer that the currentAssemblyPath field points to.
    /// </summary>
    public uint currentAssemblyPathSize;

  }
}
