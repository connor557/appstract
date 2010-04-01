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

namespace ManagedFusion.Fusion
{
  /// <summary>
  /// The IAssemblyEnum interface enumerates the assemblies in the GAC.
  /// </summary>
  [ComImport, Guid("21b8916c-f28e-11d2-a473-00c04f8ef448"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IAssemblyEnum
  {
    /// <summary>
    /// The IAssemblyEnum::GetNextAssembly method enumerates the assemblies in the GAC. 
    /// </summary>
    /// <param name="ppAppCtx">Must be null.</param>
    /// <param name="ppName">Pointer to a memory location that is to receive the interface pointer to the assembly 
    /// name of the next assembly that is enumerated.</param>
    /// <param name="dwFlags">Must be zero.</param>
    /// <returns></returns>
    [PreserveSig]
    int GetNextAssembly(
      out IApplicationContext ppAppCtx,
      out IAssemblyName ppName,
      uint dwFlags);

    /// <summary>
    /// Reset the enumeration to the first assembly.
    /// </summary>
    /// <returns></returns>
    [PreserveSig]
    int Reset();

    /// <summary>
    /// Create a copy of the assembly enum that is independently enumerable.
    /// </summary>
    /// <param name="ppEnum"></param>
    /// <returns></returns>
    [PreserveSig]
    int Clone(
      out IAssemblyEnum ppEnum);
  }
}
