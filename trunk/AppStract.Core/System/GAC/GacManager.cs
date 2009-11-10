#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.GAC;
using AppStract.Core.Data.Application;
using SystemProcess = System.Diagnostics.Process;

namespace AppStract.Core.System.GAC
{
  /// <summary>
  /// Manages the local system's Global Assembly Cache by registering and unregistering assemblies
  /// that need to be shared by an other application and the current application.
  /// </summary>
  public class GacManager : IDisposable
  {

    #region Variables

    /// <summary>
    /// The <see cref="ApplicationFile"/> describer for the other application's main executable file.
    /// </summary>
    private readonly ApplicationFile _otherAppExe;
    /// <summary>
    /// All assemblies shared by both the current and the other application.
    /// </summary>
    private readonly List<ApplicationFile> _sharedAssemblies;
    /// <summary>
    /// All assemblies that can only be shared by registering them to the GAC.
    /// </summary>
    private List<AssemblyName> _gacAssemblies;
    /// <summary>
    /// Indicates whether the <see cref="_gacAssemblies"/> have been registered to the GAC.
    /// </summary>
    private bool _gacRegistered;
    /// <summary>
    /// Object to manipulate the global assembly cache with.
    /// </summary>
    private AssemblyCache _assemblyCache;
    /// <summary>
    /// Insurance for leaving the global assembly cache clean.
    /// </summary>
    private CleanUpInsurance _insurance;
    /// <summary>
    /// Object to lock when performing actions on any of the class variables.
    /// </summary>
    private readonly object _gacSyncRoot;

    #endregion

    #region Constructor

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="GacException"></exception>
    /// <param name="otherAppExe"></param>
    /// <param name="sharedAssemblies"></param>
    public GacManager(string otherAppExe, IEnumerable<string> sharedAssemblies)
    {
      _gacSyncRoot = new object();
      _otherAppExe = new ApplicationFile(otherAppExe);
      if (_otherAppExe.Type != FileType.Assembly_Managed
          && _otherAppExe.Type != FileType.Assembly_Native)
        throw new GacException("\"" + _otherAppExe.FileName + "\" is no valid executable.");
      _sharedAssemblies = new List<ApplicationFile>();
      foreach (var sharedAssembly in sharedAssemblies)
      {
        var file = new ApplicationFile(sharedAssembly);
        if (file.Type == FileType.Assembly_Managed)
          _sharedAssemblies.Add(file);
      }
    }

    ~GacManager()
    {
      UnregisterGacAssemblies();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the current instance of <see cref="GacManager"/>,
    /// making sure all defined assemblies are shareable with the other application.
    /// </summary>
    /// <remarks>
    /// The values describing the assemblies and other application are all defined by the constructor.
    /// </remarks>
    public void Initialize()
    {
      lock (_gacSyncRoot)
      {
        if (_gacRegistered) return;
        if (_assemblyCache == null)
          _assemblyCache = new AssemblyCache(CoreBus.Configuration.Application.GacInstallerDescription);
        if (_gacAssemblies == null)
          _gacAssemblies = DetermineGacAssemblies();
        GC.ReRegisterForFinalize(this);
        RegisterGacAssemblies();
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Determines which assemblies specified in <see cref="_sharedAssemblies"/> have to be registered to the GAC.
    /// </summary>
    /// <exception cref="GacException">
    /// A <see cref="GacException"/> is thrown if one of the files is not a .NET assembly with valid metadata, like a strong name.
    /// </exception>
    private List<AssemblyName> DetermineGacAssemblies()
    {
      var gacAssemblies = new List<AssemblyName>();
      var otherBinDir = Path.GetDirectoryName(_otherAppExe.FileName).ToUpperInvariant();
      var thisBinDir = CoreBus.Runtime.StartUpDirectory.ToUpperInvariant();
      var sameBinDir = otherBinDir == thisBinDir;
      foreach (var file in _sharedAssemblies)
      {
        if (file.Type != FileType.Assembly_Managed)
          continue;
        var dir = Path.GetDirectoryName(file.FileName).ToUpperInvariant();
        if (sameBinDir && dir == otherBinDir)
          continue;
        // Verify that the file is a .NET assembly with valid metadata
        var assembly = Assembly.ReflectionOnlyLoadFrom(file.FileName);
        if (assembly.GetName().GetPublicKey().Length == 0)
          throw new GacException("\"" + file.FileName + "\" is not strongly signed.");
        // Verify if it doesn't exist in the GAC yet
        var assemblyName = assembly.GetName();
        if (!_assemblyCache.IsInstalled(assemblyName))
          gacAssemblies.Add(assemblyName);
      }
      return gacAssemblies;
    }

    /// <summary>
    /// Registers the files defined in <see cref="_gacAssemblies"/> to the GAC.
    /// When done, <see cref="_gacRegistered"/> is set to true.
    /// </summary>
    /// <exception cref="GacException">
    /// A <see cref="GacException"/> is thrown if <see cref="_gacAssemblies"/> has not been initialized before calling this method.
    /// </exception>
    private void RegisterGacAssemblies()
    {
      lock (_gacSyncRoot)
      {
        if (_gacRegistered) return;
        if (_gacAssemblies == null)
          throw new GacException("Can't register asssemblies to the GAC if they have not been determined yet.");
        // First insure the removal of those assemblies.
        var creationData = new InsuranceData(_assemblyCache.InstallerDescription,
                                                     CoreBus.Configuration.User.GacCleanUpInsuranceFlags,
                                                     CoreBus.Configuration.Application.GacCleanUpInsuranceFolder,
                                                     CoreBus.Configuration.Application.GacCleanUpInsuranceRegistryKey,
                                                     CoreBus.Configuration.Application.WatcherExecutable);
        _insurance = CleanUpInsurance.CreateInsurance(creationData, _gacAssemblies);
        // Then install the assemblies.
        foreach (var assembly in _gacAssemblies)
          _assemblyCache.InstallAssembly(assembly, InstallBehaviour.Default);
        _gacRegistered = true;
      }
    }

    /// <summary>
    /// Unregisters the files defined in <see cref="_gacAssemblies"/> to the GAC.
    /// When done, <see cref="_gacRegistered"/> is set to false.
    /// </summary>
    /// <exception cref="GacException">
    /// A <see cref="GacException"/> is thrown if <see cref="_gacAssemblies"/> has not been initialized before calling this method.
    /// </exception>
    private void UnregisterGacAssemblies()
    {
      lock (_gacSyncRoot)
      {
        if (!_gacRegistered) return;
        if (_gacAssemblies == null)
          throw new GacException("Can't unregister asssemblies from the GAC if they have not been determined yet.");
        foreach (var assembly in _gacAssemblies)
          _assemblyCache.UninstallAssembly(assembly);
        _insurance.Dispose();
        _gacRegistered = false;
      }
    }

    #endregion

    #region IDisposable Members

    /// <summary>
    /// Disposes the current instance and reverts all changes made to the local system.
    /// </summary>
    public void Dispose()
    {
      lock (_gacSyncRoot)
      {
        UnregisterGacAssemblies();
        GC.SuppressFinalize(this);
      }
    }

    #endregion

  }
}
