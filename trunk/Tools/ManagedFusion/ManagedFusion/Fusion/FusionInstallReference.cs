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
  /// The <see cref="FusionInstallReference"/> structure represents a reference that is made
  /// when an application has installed an assembly in the GAC.
  /// </summary>
  /// <remarks>
  /// Native name: FUSION_INSTALL_REFERENCE
  /// </remarks>
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  internal struct FusionInstallReference
  {

    #region Variables

    /// <summary>
    /// The size of the structure in bytes.
    /// </summary>
    /// <remarks>
    /// <see cref="Marshal.SizeOf(System.Type)"/> returns 32 when passing the type of <see cref="FusionInstallReference"/>,
    /// but after extensive debugging it is determined that the real size in unmanaged memory is 40 bytes.
    /// Testing is done on Windows XP x86 and on Windows Vista x64.
    /// </remarks>
    private UInt32 _size;
    /// <summary>
    /// Reserved, must be zero.
    /// </summary>
    private UInt32 _flags;
    /// <summary>
    /// The entity that adds the reference.
    /// </summary>
    public Guid GuidScheme;
    /// <summary>
    /// A unique string that identifies the application that installed the assembly.
    /// </summary>
    public string Identifier;
    /// <summary>
    /// A string that is only understood by the entity that adds the reference. The GAC only stores this string.
    /// </summary>
    public string NonCannonicalData;

    #endregion

    #region Constructors

    public FusionInstallReference(Guid entity, string uniqueId, string nonCannonicalData)
    {
      // The following line sets _size to 32
      //    _size = (uint)Marshal.SizeOf(typeof (FusionInstallReference));
      // When the following code is used the size is returned as 40:
      //    int size = Marshal.SizeOf(new FusionInstallReference(...));
      // After extensive testing it is determined that _size must always be 40.
      // _size is not made a constant because then it would be impossible to marshal instances of FusionInstallReference
      _size = 40;
      _flags = 0;
      GuidScheme = entity;
      Identifier = uniqueId;
      NonCannonicalData = nonCannonicalData;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a pointer to a copy of the current <see cref="FusionInstallReference"/>.
    /// Always call <see cref="Marshal.FreeHGlobal"/> after usage, to free the allocated memory.
    /// </summary>
    /// <returns></returns>
    public IntPtr WriteToPointer()
    {
      var allocatedBytes = Marshal.SizeOf(this);
      var result = Marshal.AllocHGlobal(allocatedBytes);
      try
      {
        Marshal.StructureToPtr(this, result, false);
      }
      catch
      {
        Marshal.FreeHGlobal(result);
        throw;
      }
      return result;
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Reads the <see cref="FusionInstallReference"/> allocated at the specified <see cref="IntPtr"/>.
    /// </summary>
    /// <param name="ptr"></param>
    /// <returns></returns>
    public static FusionInstallReference ReadFromPointer(IntPtr ptr)
    {
      return (FusionInstallReference)Marshal.PtrToStructure(ptr, typeof(FusionInstallReference));
    }

    #endregion

  }
}
