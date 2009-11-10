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

using System.Collections.Generic;
using System.IO;
using System.Runtime.Interop.Fusion;
using System.Runtime.InteropServices;
using System.Text;
using AppStract.Utilities.Extensions;
using Microsoft.Win32.Interop;

namespace System.Reflection.GAC
{
  /// <summary>
  /// Allows you to view and manipulate the contents of the Global Assembly Cache.
  /// </summary>
  /// <remarks>
  /// CAUTION:
  /// Do not use these APIs in your application to perform assembly binds or to test for the presence of assemblies
  /// or other run time, development, or design-time operations. Only administrative tools and setup programs must
  /// use these APIs. If you use the GAC, this directly exposes your application to assembly binding fragility
  /// or may cause your application to work improperly on future versions of the .NET Framework.
  /// </remarks>
  public class AssemblyCache : IEnumerable<AssemblyName>
  {

    #region DLL Entries

    /// <summary>
    /// The key entry point for reading the assembly cache.
    /// </summary>
    /// <param name="ppAsmCache">Pointer to return IAssemblyCache</param>
    /// <param name="dwReserved">must be 0</param>
    [DllImport("fusion.dll", SetLastError = true, PreserveSig = false)]
    private static extern void CreateAssemblyCache(out IAssemblyCache ppAsmCache, uint dwReserved);

    /// <summary>
    /// To obtain an instance of the CreateInstallReferenceEnum API, call the CreateInstallReferenceEnum API.
    /// </summary>
    /// <param name="ppRefEnum">A pointer to a memory location that receives the IInstallReferenceEnum pointer.</param>
    /// <param name="pName">The assembly name for which the references are enumerated.</param>
    /// <param name="dwFlags"> Must be zero.</param>
    /// <param name="pvReserved">Must be null.</param>
    [DllImport("fusion.dll", SetLastError = true, PreserveSig = false)]
    private static extern void CreateInstallReferenceEnum(out IInstallReferenceEnum ppRefEnum, IAssemblyName pName,
      uint dwFlags, IntPtr pvReserved);

    /// <summary>
    /// Gets the path to the cached assembly, using the specified flags.
    /// </summary>
    /// <param name="dwCacheFlags">Exactly one of the bits defined in the ASM_CACHE_FLAGS enumeration.</param>
    /// <param name="pwzCachePath">Pointer to a buffer that is to receive the path of the GAC as a Unicode string.</param>
    /// <param name="pcchPath">Length of the pwszCachePath buffer, in Unicode characters.</param>
    [DllImport("fusion.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
    private static extern void GetCachePath(AssemblyCacheFlags dwCacheFlags, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwzCachePath,
      ref uint pcchPath);

    /// <summary>
    /// An instance of IAssemblyName is obtained by calling the CreateAssemblyNameObject API.
    /// </summary>
    /// <param name="ppAssemblyNameObj">Pointer to a memory location that receives the IAssemblyName pointer that is created.</param>
    /// <param name="szAssemblyName">A string representation of the assembly name or of a full assembly reference that is 
    /// determined by dwFlags. The string representation can be null.</param>
    /// <param name="dwFlags">Zero or more of the bits that are defined in the CREATE_ASM_NAME_OBJ_FLAGS enumeration.</param>
    /// <param name="pvReserved"> Must be null.</param>
    [DllImport("fusion.dll", SetLastError = true, CharSet = CharSet.Unicode, PreserveSig = false)]
    internal static extern void CreateAssemblyNameObject(out IAssemblyName ppAssemblyNameObj, string szAssemblyName,
      CreateDisposition dwFlags, IntPtr pvReserved);

    #endregion

    #region Variables

    private readonly IAssemblyCache _gac;
    private readonly InstallerDescription _installer;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the <see cref="InstallerDescription"/> for the application using the current instance of <see cref="AssemblyCache"/>.
    /// </summary>
    public InstallerDescription InstallerDescription
    {
      get { return _installer; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="AssemblyCache"/> for the specified installer.
    /// </summary>
    /// <param name="installerDescription"></param>
    public AssemblyCache(InstallerDescription installerDescription)
    {
      CreateAssemblyCache(out _gac, 0);
      _installer = installerDescription;
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Returns the storage location of the Global Assembly Cache.
    /// </summary>
    /// <returns></returns>
    public static string GetGACLocation()
    {
      return GetPath(AssemblyCacheFlags.GAC);
    }

    /// <summary>
    /// Returns the storage location of the cache of precompiled assemblies
    /// </summary>
    /// <returns></returns>
    public static string GetZapPath()
    {
      return GetPath(AssemblyCacheFlags.ZAP);
    }

    /// <summary>
    /// Returns the storage location of the assemblies that have been downloaded on demand or that have been shadow-copied.
    /// </summary>
    /// <returns></returns>
    public static string GetDownloadPath()
    {
      return GetPath(AssemblyCacheFlags.Download);
    }

    /// <summary>
    /// Gets the path to the cache, specified by the given <see cref="AssemblyCacheFlags"/>.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    private static string GetPath(AssemblyCacheFlags flag)
    {
      uint bufferSize = 255;
      var buffer = new StringBuilder((int)bufferSize);
      GetCachePath(flag, buffer, ref bufferSize);
      return buffer.ToString();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Adds a new assembly to the GAC. The assembly must be persisted in the file system and is copied to the GAC.
    /// </summary>
    /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if the <paramref name="assemblyName"/> parameter is null.</exception>
    /// <exception cref="ArgumentException"> An <see cref="ArgumentException"/> is thrown when the <see cref="AssemblyName.CodeBase"/> property is null for the given <paramref name="assemblyName"/>.</exception>
    /// <exception cref="FileNotFoundException">A <see cref="FileNotFoundException"/> is thrown when the specified assembly cannot be found.</exception>
    /// <exception cref="UnauthorizedAccessException">An <see cref="UnauthorizedAccessException"/> is thrown when the caller does not have the required rights to install an assembly.</exception>
    /// <param name="assemblyName"></param>
    /// <param name="disposition"></param>
    public void InstallAssembly(AssemblyName assemblyName, InstallBehaviour disposition)
    {
      if (assemblyName == null)
        throw new ArgumentNullException("assemblyName");
      if (string.IsNullOrEmpty(assemblyName.CodeBase))
        throw new ArgumentException("The CodeBase property of the AssemblyName \"assembly\" parameter needs to be specified.");
      if (!File.Exists(assemblyName.CodeBase))
        throw new FileNotFoundException("The assembly to install to the GAC doesn't exist.", assemblyName.CodeBase);
      var refPtr = _installer.ToFusionStruct().ToPointer();
      int hResult;
      try
      {
        hResult = _gac.InstallAssembly(disposition, assemblyName.CodeBase, refPtr);
      }
      finally
      {
        Marshal.FreeHGlobal(refPtr);
      }
      Marshal.ThrowExceptionForHR(hResult);
    }

    /// <summary>
    /// Removes an assembly from the GAC and returns the result as an <see cref="UninstallDisposition"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UnauthorizedAccessException">An <see cref="UnauthorizedAccessException"/> is thrown when the caller does not have the required rights to install an assembly.</exception>
    /// <param name="assemblyName">The <see cref="AssemblyName"/> to uninstall from the GAC.</param>
    /// <returns></returns>
    public UninstallDisposition UninstallAssembly(AssemblyName assemblyName)
    {
      if (assemblyName == null)
        throw new ArgumentNullException("assemblyName");
      var refPtr = _installer.ToFusionStruct().ToPointer();
      try
      {
        UninstallDisposition uninstallDisposition;
        var descr = assemblyName.GetFusionCompatibleFullName();
        var hResult = _gac.UninstallAssembly(0, descr, refPtr, out uninstallDisposition);
        Marshal.ThrowExceptionForHR(hResult);
        return uninstallDisposition;
      }
      finally
      {
        Marshal.FreeHGlobal(refPtr);
      }
    }

    /// <summary>
    /// Returns whether an assembly with the specified <see cref="AssemblyName"/> is installed in the Global Assembly Cache.
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public bool IsInstalled(AssemblyName assemblyName)
    {
      var assemblyInfo = new AssemblyInfo();
      assemblyInfo.currentAssemblyPathSize = 255;
      assemblyInfo.currentAssemblyPath = new String('\0', (int) assemblyInfo.currentAssemblyPathSize);
      var descr = assemblyName.GetFusionCompatibleFullName();
      var hResult = _gac.QueryAssemblyInfo(QueryTypeId.Validate, descr, ref assemblyInfo);
      return WinError.Succeeded(hResult)
             && assemblyInfo.assemblyFlags == AssemblyInfoFlags.Installed;
    }

    /// <summary>
    /// Returns all references to the specified <see cref="AssemblyName"/>.
    /// </summary>
    /// <exception cref="FileLoadException">
    /// A <see cref="FileLoadException"/> is thrown when the <see cref="AssemblyName.CodeBase"/> property
    /// of the specified <paramref name="assemblyName"/> is invalid.
    /// </exception>
    /// <param name="assemblyName"></param>
    /// <returns></returns>
    public IEnumerable<InstallerDescription> GetReferences(AssemblyName assemblyName)
    {
      IInstallReferenceEnum referenceEnum;
      try
      {
        CreateInstallReferenceEnum(out referenceEnum, assemblyName.ToIAssemblyName(), 0, IntPtr.Zero);
      }
      catch (FileLoadException)
      {
        referenceEnum = null;
      }
      if (referenceEnum != null)
      {
        IInstallReferenceItem item;
        while (WinError.Succeeded(referenceEnum.GetNextInstallReferenceItem(out item, 0, IntPtr.Zero)))
        {
          IntPtr ptr;
          if (WinError.Succeeded(item.GetReference(out ptr, 0, IntPtr.Zero)))
            yield return new InstallerDescription(ptr.Read<FusionInstallReference>());
        }
      }
    }

    #endregion

    #region IEnumerable<AssemblyName> Members

    public IEnumerator<AssemblyName> GetEnumerator()
    {
        return new AssemblyEnumerator();
    }

    #endregion

    #region IEnumerable Members

    Collections.IEnumerator Collections.IEnumerable.GetEnumerator()
    {
        return new AssemblyEnumerator();
    }

    #endregion

  }
}
