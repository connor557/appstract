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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.GAC;
using System.Security.Cryptography;
using AppStract.Utilities.Helpers;
using Microsoft.Win32;

namespace AppStract.Core.System.GAC
{
  /// <summary>
  /// Represents and provides an insurance for specified assemblies to be uninstalled
  /// from the global assembly cache after they are not needed anymore.
  /// As soon as the assemblies do not need to be insured anymore, <see cref="Dispose"/> should be called.
  /// </summary>
  /// <remarks>
  /// The way the insurance is provided depends on the <see cref="CleanUpInsuranceFlags"/> set in the user's configuration.
  /// </remarks>
  public class CleanUpInsurance
  {

    #region Constants

    /// <summary>
    /// The directory containing the files used to track the installed assemblies.
    /// </summary>
    private const string _TrackingFilesDirectory = @"\GAC\";
    /// <summary>
    /// The registry key, under <see cref="Registry.CurrentUser"/>, containing the subkeys used to track the installed assemblies.
    /// </summary>
    private const string _TrackingRegistryKey = @"Software\AppStract\";

    #endregion

    #region Variables

    /// <summary>
    /// The flags to base the method of insurance on.
    /// </summary>
    private readonly CleanUpInsuranceFlags _flags;
    /// <summary>
    /// The <see cref="InstallerDescription"/> for the application that's installing and uninstalling the insured assemblies.
    /// </summary>
    private readonly InstallerDescription _assemblyInstallerDescription;
    /// <summary>
    /// The assemblies to ensure GAC-cleanup for.
    /// </summary>
    private readonly List<AssemblyName> _assemblies;
    /// <summary>
    /// The string representing the creation <see cref="DateTime"/> for the current instance.
    /// </summary>
    private readonly DateTime _creationDateTime;
    /// <summary>
    /// The unique ID for the current instance.
    /// </summary>
    private readonly string _uniqueId;
    /// <summary>
    /// The file holding the insured assemblies.
    /// </summary>
    private InsuranceFile _insuranceFile;
    /// <summary>
    /// The registrykey holding the insured assemblies.
    /// </summary>
    private InsuranceRegistryKey _insuranceRegistryKey;
    /// <summary>
    /// The external process responsible for uninstalling the <see cref="_assemblies"/>.
    /// </summary>
    private Process _insuranceProcess;

    #endregion

    #region Properties

    /// <summary>
    /// The assemblies for which cleanup is insured by the current instance.
    /// </summary>
    public IEnumerable<AssemblyName> InsuredAssemblies
    { 
      get { return _assemblies; }
    }

    /// <summary>
    /// Gets the directory containing all insurance files.
    /// </summary>
    private static string InsuranceDirectory
    {
      get { return CoreBus.Runtime.StartUpDirectory + _TrackingFilesDirectory; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="CleanUpInsurance"/> built from the data specified.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if <paramref name="insuranceFile"/> and <paramref name="insuranceRegistryKey"/> don't match.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// An <see cref="ArgumentNullException"/> is thrown if both <paramref name="insuranceFile"/> and <paramref name="insuranceRegistryKey"/> are null.
    /// </exception>
    /// <param name="insuranceFile"></param>
    /// <param name="insuranceRegistryKey"></param>
    /// <param name="insuranceProcess"></param>
    private CleanUpInsurance(InsuranceFile insuranceFile, InsuranceRegistryKey insuranceRegistryKey, Process insuranceProcess)
    {
      _insuranceFile = insuranceFile;
      _insuranceProcess = insuranceProcess;
      _insuranceRegistryKey = insuranceRegistryKey;
      _flags = (_insuranceFile != null ? CleanUpInsuranceFlags.TrackByFile : CleanUpInsuranceFlags.None)
               | (_insuranceProcess != null ? CleanUpInsuranceFlags.ByWatchService : CleanUpInsuranceFlags.None)
               | (_insuranceRegistryKey != null ? CleanUpInsuranceFlags.TrackByRegistry : CleanUpInsuranceFlags.None);
      InsuranceBase insuranceBase = null;
      if (_insuranceFile != null)
      {
        insuranceBase = _insuranceFile;
        if (_insuranceRegistryKey != null)
        {
          if (!insuranceBase.MatchesWith(_insuranceRegistryKey, false))
            throw new ArgumentException("The InsuranceFile and InsuranceRegistryKey don't match.", "insuranceFile");
          insuranceBase.JoinWith(_insuranceRegistryKey);
        }
      }
      else if (_insuranceRegistryKey != null)
        insuranceBase = _insuranceRegistryKey;
      if (insuranceBase == null)
        throw new ArgumentNullException("insuranceFile",
                                        "At least one of both insuranceFile and insuranceRegistryKey needs to be initialized.");
      _assemblyInstallerDescription = insuranceBase.InstallerDescription;
      _assemblies = new List<AssemblyName>(insuranceBase.Assemblies);
      _creationDateTime = insuranceBase.CreationDateTime;
      _uniqueId = insuranceBase.InsuranceIdentifier;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CleanUpInsurance"/> built from the data specified.
    /// </summary>
    /// <param name="installerDescription"></param>
    /// <param name="assemblyNames"></param>
    private CleanUpInsurance(InstallerDescription installerDescription, IEnumerable<AssemblyName> assemblyNames)
    {
      _flags = CoreBus.Configuration.User.GacCleanUpInsurance;
      _assemblyInstallerDescription = installerDescription;
      _assemblies = new List<AssemblyName>(assemblyNames);
      _creationDateTime = DateTime.Now;
      var uniqueId = new byte[10];
      new RNGCryptoServiceProvider().GetBytes(uniqueId);
      _uniqueId = Convert.ToBase64String(uniqueId);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Removes all insurance methods from the local system.
    /// </summary>
    public void Dispose()
    {
      if (_assemblies.Count == 0)
        return;
      CleanFileInsurance();
      CleanRegistryInsurance();
      CleanProcessInsurance();
    }

    #endregion

    #region Private Methods

    private void CreateFileInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile))
        return;
      Directory.CreateDirectory(InsuranceDirectory);
      _insuranceFile = new InsuranceFile(InsuranceDirectory + _uniqueId, _assemblyInstallerDescription,
                                         LocalMachine.Identifier, _creationDateTime, _assemblies);
      InsuranceFile.Write(_insuranceFile);
    }

    private void CreateRegistryInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry))
        return;
      RegistryKey key;
      using (var rootKey = Registry.CurrentUser.CreateSubKey(_TrackingRegistryKey))
        key = rootKey.CreateSubKey(_uniqueId);
      _insuranceRegistryKey = new InsuranceRegistryKey(key, _assemblyInstallerDescription,
                                                       LocalMachine.Identifier, _creationDateTime, _assemblies);
      InsuranceRegistryKey.Write(_insuranceRegistryKey);
    }

    private void CreateWatchingProcessInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService))
        return;
      var startInfo = new ProcessStartInfo
      {
        FileName = CoreBus.Configuration.Application.WatcherExecutable,
        Arguments = CoreBus.Runtime.CurrentProcess.Id + " FLAGS=" + _flags + " ID=" + _uniqueId,
        CreateNoWindow = true
      };
      _insuranceProcess = Process.Start(startInfo);
    }

    private void CleanFileInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile)
          || !Directory.Exists(InsuranceDirectory))
        return;
      File.Delete(_insuranceFile.FileName);
      if (Directory.GetFiles(InsuranceDirectory).Length == 0)
        Directory.Delete(InsuranceDirectory);
    }

    private void CleanRegistryInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry)
        || _insuranceRegistryKey == null)
        return;
      bool deleteTree;
      using (var key = Registry.CurrentUser.OpenSubKey(_TrackingRegistryKey))
      {
        if (key == null) return;
        var subKeys = key.GetSubKeyNames();
        if (subKeys.Contains(_uniqueId))
          key.DeleteSubKeyTree(_uniqueId);
        deleteTree = subKeys.Length == 1;
      }
      if (deleteTree)
        Registry.CurrentUser.DeleteSubKey(_TrackingRegistryKey);
    }

    private void CleanProcessInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService)
          || _insuranceProcess == null)
        return;
      _insuranceProcess.Refresh();
      if (!_insuranceProcess.HasExited)
        _insuranceProcess.Kill();
    }

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Creates an insurance for the <paramref name="assemblyNames"/>.
    /// The method of insuring is based on the <see cref="CleanUpInsuranceFlags"/> set in the user's configuration.
    /// </summary>
    /// <param name="installerDescription">The installer that's creating the new <see cref="CleanUpInsurance"/>.</param>
    /// <param name="assemblyNames">The assemblies to insure cleanup for.</param>
    /// <returns></returns>
    public static CleanUpInsurance CreateInsurance(InstallerDescription installerDescription, IEnumerable<AssemblyName> assemblyNames)
    {
      var insurance = new CleanUpInsurance(installerDescription, assemblyNames);
      if (insurance._assemblies.Count == 0)
        return insurance;
      insurance.CreateFileInsurance();
      insurance.CreateRegistryInsurance();
      insurance.CreateWatchingProcessInsurance();
      return insurance;
    }

    /// <summary>
    /// Returns all insurances found in the local system.
    /// </summary>
    /// <returns></returns>
    public static List<CleanUpInsurance> LoadFromSystem()
    {
      // Find valid InsuranceFiles
      var files = GetFileInsurances(_TrackingFilesDirectory);
      // Find valid InsuranceRegistryKeys
      var keys = GetRegistryInsurances(_TrackingRegistryKey);
      // Build the result
      var result = new List<CleanUpInsurance>();
      // Enumerate all files, while trying to find matching registrykeys
      foreach (var file in files)
      {
        var key = keys.FindElement(file.InsuranceIdentifier);
        keys.Remove(key);
        result.Add(new CleanUpInsurance(file, key, null));
      }
      // Now enumerate the keys that didn't match to a file.
      foreach (var key in keys)
        result.Add(new CleanUpInsurance(null, key, null));
      return result;
    }

    /// <summary>
    /// Returns the <see cref="CleanUpInsurance"/> matching the identifier specified.
    /// If no match is found, null is returned.
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <returns></returns>
    public static CleanUpInsurance LoadFromSystem(string uniqueId)
    {
      if (string.IsNullOrEmpty(uniqueId))
        throw new ArgumentNullException("uniqueId");
      InsuranceFile insuranceFile = null;
      InsuranceRegistryKey insuranceRegKey = null;
      // Load from file
      if (File.Exists(InsuranceDirectory + uniqueId))
        InsuranceFile.TryRead(InsuranceDirectory + uniqueId, out insuranceFile);
      // Load from registry
      using (var regKey = Registry.CurrentUser.OpenSubKey(_TrackingRegistryKey + uniqueId, false))
        if (regKey != null) // Verify existence of the key
          InsuranceRegistryKey.TryRead(regKey, out insuranceRegKey);
      // Possible to check for a running process?
      // Return null if no CleanUpInsurance can be built from the retrieved data
      if (insuranceFile == null && insuranceRegKey == null)
        return null;
      return new CleanUpInsurance(insuranceFile, insuranceRegKey, null);
    }

    #endregion

    #region Private Static Methods

    private static List<InsuranceFile> GetFileInsurances(string directory)
    {
      var identifiers = Directory.GetFiles(directory);
      var files = new List<InsuranceFile>();
      foreach (var file in identifiers)
      {
        InsuranceFile insuranceFile;
        if (InsuranceFile.TryRead(file, out insuranceFile))
          files.Add(insuranceFile);
      }
      return files;
    }

    private static List<InsuranceRegistryKey> GetRegistryInsurances(string regKeyName)
    {
      using (var regKey = Registry.CurrentUser.OpenSubKey(regKeyName))
      {
        if (regKey == null)
          return new List<InsuranceRegistryKey>(0);
        var items = new List<InsuranceRegistryKey>();
        var subKeys = regKey.GetSubKeyNames();
        foreach (var subKeyName in subKeys)
        {
          using (var subKey = regKey.OpenSubKey(subKeyName))
          {
            InsuranceRegistryKey item;
            if (InsuranceRegistryKey.TryRead(subKey, out item))
              items.Add(item);
          }
        }
        return items;
      }
    }

    #endregion

  }
}
