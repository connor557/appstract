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
using AppStract.Host.Data.Application;
using AppStract.Utilities.ManagedFusion;
using AppStract.Utilities.ManagedFusion.Insuring;
using SystemProcess = System.Diagnostics.Process;

namespace AppStract.Host.System.GAC
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
    /// Indicates whether the shared assemblies have been registered to the GAC.
    /// </summary>
    private bool _gacRegistered;
    /// <summary>
    /// Insurance for leaving the global assembly cache clean.
    /// </summary>
    private CleanUpInsurance _insurance;
    /// <summary>
    /// Object to lock when performing actions on any of the class variables.
    /// </summary>
    private readonly object _gacSyncRoot;

    #endregion

    #region Constructor/Destructors

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
      if (_otherAppExe.Type != FileType.Executable)
        throw new GacException("\"" + _otherAppExe.FileName + "\" is no valid executable.");
      _sharedAssemblies = new List<ApplicationFile>();
      foreach (var sharedAssembly in sharedAssemblies)
      {
        var file = new ApplicationFile(sharedAssembly);
        if (file.Type == FileType.Library)
          _sharedAssemblies.Add(file);
      }
    }

    ~GacManager()
    {
      UnregisterAssemblies();
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
      RegisterAssemblies();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Determines which assemblies specified in <see cref="_sharedAssemblies"/> have to be registered to the GAC.
    /// </summary>
    /// <exception cref="GacException">
    /// A <see cref="GacException"/> is thrown if one of the files is not a .NET assembly with valid metadata, like a strong name.
    /// </exception>
    private IEnumerable<AssemblyName> DetermineGacAssemblies()
    {
      var gacAssemblies = new List<AssemblyName>();
      var otherBinDir = Path.GetDirectoryName(_otherAppExe.FileName).ToUpperInvariant();
      var thisBinDir = HostCore.Runtime.StartUpDirectory.ToUpperInvariant();
      var sameBinDir = otherBinDir == thisBinDir;
      foreach (var file in _sharedAssemblies)
      {
        var type = file.GetLibraryType();
        if (type != LibraryType.Managed)
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
        if (!AssemblyCache.IsInstalled(assemblyName))
          gacAssemblies.Add(assemblyName);
      }
      return gacAssemblies;
    }

    /// <summary>
    /// Registers the shared assemblies to the GAC.
    /// When done, <see cref="_gacRegistered"/> is set to true.
    /// </summary>
    private void RegisterAssemblies()
    {
      lock (_gacSyncRoot)
      {
        if (_gacRegistered) return;
        var assemblies = DetermineGacAssemblies();
        // First insure the removal of those assemblies.
        var insuranceData = new InsuranceData(HostCore.Configuration.Application.GacInstallerDescription,
                                              HostCore.Configuration.User.GacCleanUpInsuranceFlags,
                                              HostCore.Configuration.Application.GacCleanUpInsuranceFolder,
                                              HostCore.Configuration.Application.GacCleanUpInsuranceRegistryKey,
                                              HostCore.Configuration.Application.WatcherExecutable);
        _insurance = CleanUpInsurance.CreateInsurance(insuranceData, assemblies);
        GC.ReRegisterForFinalize(this);
        // Then install the assemblies.
        try
        {
          var cache = new AssemblyCache(insuranceData.Installer);
          foreach (var assembly in assemblies)
            cache.InstallAssembly(assembly, InstallBehaviour.Default);
        }
        catch (UnauthorizedAccessException)
        {
          _insurance.Dispose(insuranceData.Flags);
          throw;
        }
        _gacRegistered = true;
      }
    }

    /// <summary>
    /// Unregisters the assemblies from the GAC.
    /// When done, <see cref="_gacRegistered"/> is set to false.
    /// </summary>
    private void UnregisterAssemblies()
    {
      lock (_gacSyncRoot)
      {
        if (!_gacRegistered)
          return;
        try
        {
          _insurance.Dispose(true);
          _gacRegistered = false;
        }
        catch (ApplicationException)
        {
        }
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
        UnregisterAssemblies();
        GC.SuppressFinalize(this);
      }
    }

    #endregion

  }
}
