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
  /// The IInstallReferenceItem interface represents a reference that has been set on an assembly in the GAC. 
  /// Instances of IInstallReferenceItem are returned by the <see cref="IInstallReferenceEnum"/> interface.
  /// </summary>
  [ComImport, Guid("582dac66-e678-449f-aba6-6faaec8a9394"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IInstallReferenceItem
  {
    /// <summary>
    /// The IInstallReferenceItem::GetReference method returns a <see cref="FusionInstallReference"/> structure. 
    /// </summary>
    /// <param name="ppRefData">A pointer to a <see cref="FusionInstallReference"/> structure.
    /// The memory is allocated by the GetReference method and is freed when IInstallReferenceItem is released.
    /// Callers must not hold a reference to this buffer after the IInstallReferenceItem object is released.
    /// </param>
    /// <param name="dwFlags">Must be zero.</param>
    /// <param name="pvReserved">Must be null.</param>
    /// <returns></returns>
    [PreserveSig]
    int GetReference(
      out IntPtr ppRefData,
      uint dwFlags,
      IntPtr pvReserved);
  }
}
