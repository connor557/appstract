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
using System.Security.Cryptography;
using AppStract.Core.Data.Application;
using EasyHook;
using SystemProcess = System.Diagnostics.Process;

namespace AppStract.Core.System.GAC
{
  /// <summary>
  /// Manages the local system's Global Assembly Cache by registering and unregistering assemblies
  /// that need to be shared by an other application and the current application.
  /// </summary>
  /// <remarks>
  /// ToDo:
  ///   - Verify if the assembly doesn't already exist in the GAC before registering it
  ///   - Support for disaster recovery
  ///     -> store changes made to the GAC in the system's registry until they are undone
  ///     -> in case they are not undone during the current run, GacManager can automatically clean them on the next run
  ///   - Implement ForceGacCleanUp() method
  /// </remarks>
  public class GacManager : IDisposable
  {

    #region Variables

    private readonly ApplicationFile _thisAppExe;
    private readonly ApplicationFile _otherAppExe;
    /// <summary>
    /// All assemblies that are shared by both the host and guest application.
    /// </summary>
    private readonly List<ApplicationFile> _sharedAssemblies;

    /// <summary>
    /// All assemblies that can only be shared by registering them to the GAC.
    /// </summary>
    private List<ApplicationFile> _gacAssemblies;
    /// <summary>
    /// Indicates whether the <see cref="_gacAssemblies"/> have been registered to the GAC.
    /// </summary>
    private bool _gacRegistered;

    private readonly string _gacUniqueId;
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
      // Create unique installation identifier
      byte[] uniqueIdBytes = new byte[30];
      new RNGCryptoServiceProvider().GetBytes(uniqueIdBytes);
      _gacUniqueId = Convert.ToBase64String(uniqueIdBytes);
      // Get and verify all associated files
      _thisAppExe = new ApplicationFile(SystemProcess.GetCurrentProcess().MainModule.FileName);
      _otherAppExe = new ApplicationFile(otherAppExe);
      if (_otherAppExe.Type != FileType.Assembly_Managed && _otherAppExe.Type != FileType.Assembly_Native)
        throw new GacException("\"" + _otherAppExe.FileName + "\" is no valid executable.");
      _sharedAssemblies = new List<ApplicationFile>();
      foreach (var sharedAssembly in sharedAssemblies)
      {
        var file = new ApplicationFile(sharedAssembly);
        if (file.Type != FileType.Assembly_Managed && file.Type != FileType.Assembly_Native)
          throw new GacException("\"" + file.FileName + "\" is no valid assembly.");
        _sharedAssemblies.Add(file);
      }
    }

    ~GacManager()
    {
      UnregisterGacAssemblies();
    }

    #endregion

    #region Methods

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
        GC.ReRegisterForFinalize(this);
        if (_gacAssemblies == null)
          _gacAssemblies = DetermineGacAssemblies();
        RegisterGacAssemblies();
      }
    }

    /// <summary>
    /// Removes the specified <paramref name="assemblies"/> from the local system's
    /// Global Assembly Cache.
    /// </summary>
    /// <param name="assemblies">The assemblies to unregister from the GAC.</param>
    public static void ForceGacCleanUp(IEnumerable<string> assemblies)
    {
      // ToDo: Implement functionality so GacManager can remove the specified assemblies from the GAC
      throw new NotImplementedException();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Determines which assemblies specified in <see cref="_sharedAssemblies"/> have to be registered to the GAC.
    /// </summary>
    /// <exception cref="GacException">
    /// A <see cref="GacException"/> is thrown if one of the files is not a .NET assembly with valid metadata, like a strong name.
    /// </exception>
    private List<ApplicationFile> DetermineGacAssemblies()
    {
      var gacAssemblies = new List<ApplicationFile>();
      var otherBinDir = Path.GetDirectoryName(_otherAppExe.FileName).ToUpperInvariant();
      var thisBinDir = Path.GetDirectoryName(_thisAppExe.FileName).ToUpperInvariant();
      var sameBinDir = otherBinDir == thisBinDir;
      foreach (var file in _sharedAssemblies)
      {
        if (file.Type != FileType.Assembly_Managed)
          continue;
        var dir = Path.GetDirectoryName(file.FileName).ToUpperInvariant();
        if (sameBinDir && dir == otherBinDir)
          continue;
        // Verify that the file is a .NET assembly with valid metadata
        if (Assembly.ReflectionOnlyLoadFrom(file.FileName).GetName().GetPublicKey().Length == 0)
          throw new GacException("\"" + file.FileName + "\" is not strongly signed.");
        gacAssemblies.Add(file);
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
          throw new GacException("Can't unregister asssemblies from the GAC if they have not been determined yet.");
        var gacContext = NativeAPI.GacCreateContext();
        try
        {
          foreach (var assembly in _gacAssemblies)
            NativeAPI.GacInstallAssembly(gacContext, assembly.FileName, "AppStract GAC Manager", _gacUniqueId);
        }
        finally
        {
          NativeAPI.GacReleaseContext(ref gacContext);
        }
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
        var gacContext = NativeAPI.GacCreateContext();
        try
        {
          foreach (var assembly in _gacAssemblies)
          {
            try
            {
              NativeAPI.GacUninstallAssembly(gacContext, assembly.FileName, "AppStract GAC Manager", _gacUniqueId);
            }
            catch (Exception e)
            {
              CoreBus.Log.Critical("Unable to clean \"{0}\" from Global Assembly Cache.", e, assembly);
            }
          }
        }
        finally
        {
          NativeAPI.GacReleaseContext(ref gacContext);
        }
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
