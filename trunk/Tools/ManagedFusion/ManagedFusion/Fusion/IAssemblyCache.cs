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
  /// The IAssemblyCache interface is the top-level interface that provides access to the GAC.
  /// </summary>
  [ComImport, Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae"),
  InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  internal interface IAssemblyCache
  {

    /// <summary>
    /// The IAssemblyCache::UninstallAssembly method removes a reference to an assembly from the GAC. 
    /// If other applications hold no other references to the assembly, the files that make up the assembly are removed from the GAC. 
    /// </summary>
    /// <param name="dwFlags">No flags defined. Must be zero.</param>
    /// <param name="pszAssemblyName">The name of the assembly. A zero-ended Unicode string.</param>
    /// <param name="pRefData">A pointer to a <see cref="FusionInstallReference"/> structure. Although this is not recommended, 
    ///		this parameter can be null. The assembly is installed without an application reference, or all existing application 
    ///		references are gone.</param>
    /// <param name="pulDisposition">Pointer to an integer that indicates the action that is performed by the function.</param>
    /// <returns>The return values are defined as follows: 
    ///		S_OK - The assembly has been uninstalled.
    ///		S_FALSE - The operation succeeded, but the assembly was not removed from the GAC. 
    ///		The reason is described in pulDisposition.</returns>
    [PreserveSig]
    int UninstallAssembly(
      uint dwFlags,
      [MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName,
      IntPtr pRefData,
      [MarshalAs(UnmanagedType.U4)] out UninstallDisposition pulDisposition);

    /// <summary>
    /// The IAssemblyCache::QueryAssemblyInfo method retrieves information about an assembly from the GAC. 
    /// </summary>
    /// <param name="queryResultType">The type of information to query.</param>
    /// <param name="pszAssemblyName">The name of the assembly to query information for, this is NOT the full name.</param>
    /// <param name="pAsmInfo">The resulting <see cref="AssemblyInfo"/>.</param>
    /// <returns></returns>
    [PreserveSig]
    int QueryAssemblyInfo(
      QueryTypeId queryResultType,
      [MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName,
      ref AssemblyInfo pAsmInfo);

    /// <summary>
    /// Undocumented
    /// </summary>
    /// <param name="dwFlags"></param>
    /// <param name="pvReserved"></param>
    /// <param name="ppAsmItem"></param>
    /// <param name="pszAssemblyName"></param>
    /// <returns></returns>
    [PreserveSig]
    int CreateAssemblyCacheItem(
      uint dwFlags,
      IntPtr pvReserved,
      out IAssemblyCacheItem ppAsmItem,
      [MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName);

    /// <summary>
    /// Undocumented
    /// </summary>
    /// <param name="ppAsmScavenger"></param>
    /// <returns></returns>
    [PreserveSig]
    int CreateAssemblyScavenger(
      [MarshalAs(UnmanagedType.IUnknown)] out object ppAsmScavenger);

    /// <summary>
    /// The IAssemblyCache::InstallAssembly method adds a new assembly to the GAC. The assembly must be persisted in the file 
    /// system and is copied to the GAC.
    /// </summary>
    /// <param name="dwFlags">At most, one of the bits of the <see cref="InstallBehaviour"/> enumeration.</param>
    /// <param name="pszManifestFilePath"> A string pointing to the dynamic-linked library (DLL) that contains the assembly manifest. 
    ///	Other assembly files must reside in the same directory as the DLL that contains the assembly manifest.</param>
    /// <param name="pRefData">A pointer to a <see cref="FusionInstallReference"/> that indicates the application on whose behalf the 
    /// assembly is being installed. Although this is not recommended, this parameter can be null, but this leaves the assembly 
    /// without any application reference.</param>
    /// <returns></returns>
    [PreserveSig]
    int InstallAssembly(
      InstallBehaviour dwFlags,
      [MarshalAs(UnmanagedType.LPWStr)] string pszManifestFilePath,
      IntPtr pRefData);

  }
}
