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
using System.Runtime.InteropServices;

namespace ManagedFusion.Fusion
{
  /// <summary>
  /// The IInstallReferenceEnum interface enumerates all references that are set on an assembly in the GAC.
  /// </summary>
  /// <remarks>
  /// References that belong to the assembly are locked for changes while those references are being enumerated. 
  /// </remarks>
  [ComImport, Guid("56b1a988-7c0c-4aa2-8639-c3eb5a90226f"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IInstallReferenceEnum
  {
    /// <summary>
    /// IInstallReferenceEnum::GetNextInstallReferenceItem returns the next reference information for an assembly. 
    /// </summary>
    /// <param name="ppRefItem">Pointer to a memory location that receives the <see cref="IInstallReferenceItem"/> pointer.</param>
    /// <param name="dwFlags">Must be zero.</param>
    /// <param name="pvReserved">Must be null.</param>
    /// <returns></returns>
    [PreserveSig]
    int GetNextInstallReferenceItem(
      out IInstallReferenceItem ppRefItem,
      uint dwFlags,
      IntPtr pvReserved);
  }
}
