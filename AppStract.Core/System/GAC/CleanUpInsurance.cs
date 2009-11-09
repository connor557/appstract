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
using System.Linq;
using System.IO;
using System.Reflection;
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
    private const string _TrackingRegistryKey = @"Software\AppStract";

    #endregion

    #region Variables

    /// <summary>
    /// The flags to base the method of insurance on.
    /// </summary>
    private readonly CleanUpInsuranceFlags _flags;
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
        if (insuranceBase.MatchesWith(_insuranceRegistryKey, false))
          insuranceBase.JoinWith(_insuranceRegistryKey);
      }
      else if (_insuranceRegistryKey != null)
        insuranceBase = _insuranceRegistryKey;
      if (insuranceBase != null)
      {
        _assemblies = new List<AssemblyName>(insuranceBase.Assemblies);
        _creationDateTime = insuranceBase.CreationDateTime;
        _uniqueId = insuranceBase.InsuranceIdentifier;
      }
      else
      {
        _assemblies = new List<AssemblyName>(0);
        _creationDateTime = DateTime.Now;
        _uniqueId = "-1";
      }
    }

    private CleanUpInsurance(IEnumerable<AssemblyName> assemblyNames)
    {
      _flags = CoreBus.Configuration.User.GacCleanUpInsurance;
      _assemblies = new List<AssemblyName>(assemblyNames);
      _creationDateTime = DateTime.Now;
      var uniqueId = new byte[10];
      new RNGCryptoServiceProvider().GetBytes(uniqueId);
      _uniqueId = Convert.ToBase64String(uniqueId);
    }

    #endregion

    #region Static Methods

    /// <summary>
    /// Creates an insurance for the <paramref name="assemblyNames"/>.
    /// The method of insuring is based on the <see cref="CleanUpInsuranceFlags"/> set in the user's configuration.
    /// </summary>
    /// <param name="assemblyNames">The assemblies to insure cleanup for.</param>
    /// <returns></returns>
    public static CleanUpInsurance CreateInsurance(IEnumerable<AssemblyName> assemblyNames)
    {
      var insurance = new CleanUpInsurance(assemblyNames);
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
      // Check _trackingFilesDirectory for entries
      // Check registry for entries
      // Possible to check for running processes?
      throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the <see cref="CleanUpInsurance"/> matching the identifier specified.
    /// If no match is found, null is returned.
    /// </summary>
    /// <param name="uniqueId"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public static CleanUpInsurance LoadFromSystem(string uniqueId, CleanUpInsuranceFlags flags)
    {
      InsuranceFile insuranceFile = null;
      InsuranceRegistryKey insuranceRegKey = null;
      if (flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile)
          && Directory.Exists(InsuranceDirectory)
          && File.Exists(InsuranceDirectory + uniqueId))
        insuranceFile = InsuranceFile.Read(InsuranceDirectory + uniqueId);
      if (flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry))
      {
        using (var regKey = Registry.CurrentUser.OpenSubKey(_TrackingRegistryKey))
          if (regKey != null)      // Verify existence of key holding all insurances
            using (var subKey = regKey.OpenSubKey(uniqueId, false))
              if (subKey != null) // Verify existence of key holding the specific insurance
                insuranceRegKey = InsuranceRegistryKey.Read(subKey);
      }
      // Possible to check for a running process?
      return new CleanUpInsurance(insuranceFile, insuranceRegKey, null);
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
      CleanFiles();
      CleanRegistry();
      CleanProcesses();
    }

    #endregion

    #region Private Methods

    private void CreateFileInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile))
        return;
      Directory.CreateDirectory(InsuranceDirectory);
      _insuranceFile = new InsuranceFile(InsuranceDirectory + _uniqueId, LocalMachine.Identifier, _creationDateTime, _assemblies);
      InsuranceFile.Write(_insuranceFile);
    }

    private void CreateRegistryInsurance()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry))
        return;
      RegistryKey key;
      using (var rootKey = Registry.CurrentUser.CreateSubKey(_TrackingRegistryKey))
        key = rootKey.CreateSubKey(_uniqueId);
      _insuranceRegistryKey = new InsuranceRegistryKey(key, LocalMachine.Identifier, _creationDateTime, _assemblies);
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

    private void CleanFiles()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile))
        return;
      File.Delete(_insuranceFile.FileName);
      if (Directory.GetFiles(InsuranceDirectory).Length == 0)
        Directory.Delete(InsuranceDirectory);
    }

    private void CleanRegistry()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry)
        || _insuranceRegistryKey == null)
        return;
      bool deleteTree;
      using (var key = Registry.CurrentUser.OpenSubKey(_TrackingRegistryKey))
      {
        var subKeys = key.GetSubKeyNames();
        if (subKeys.Contains(_uniqueId))
          key.DeleteSubKeyTree(_uniqueId);
        deleteTree = subKeys.Length == 1;
      }
      if (deleteTree)
        Registry.CurrentUser.DeleteSubKey(_TrackingRegistryKey);
    }

    private void CleanProcesses()
    {
      if (!_flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService)
          || _insuranceProcess == null)
        return;
      _insuranceProcess.Refresh();
      if (!_insuranceProcess.HasExited)
        _insuranceProcess.Kill();
    }

    #endregion

  }
}
