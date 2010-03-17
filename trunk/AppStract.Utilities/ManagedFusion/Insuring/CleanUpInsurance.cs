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
using AppStract.Utilities.Helpers;
using Microsoft.Win32;

namespace AppStract.Utilities.ManagedFusion.Insuring
{
  /// <summary>
  /// Represents and provides an insurance for specified assemblies to be uninstalled from the global assembly cache after they are not needed anymore.
  /// The insurance can be completly undone by calling <see cref="Dispose()"/>.
  /// </summary>
  /// <remarks>
  /// The way the insurance is provided depends on the <see cref="CleanUpInsuranceFlags"/> set in the user's configuration.
  /// </remarks>
  public class CleanUpInsurance : IDisposable
  {

    #region Variables

    /// <summary>
    /// The data used to create the current insurance with.
    /// </summary>
    private readonly InsuranceData _data;
    /// <summary>
    /// The assemblies to ensure GAC-cleanup for.
    /// </summary>
    private readonly List<AssemblyName> _assemblies;
    /// <summary>
    /// The string representing the creation <see cref="DateTime"/> for the current instance.
    /// </summary>
    private readonly DateTime _timeStamp;
    /// <summary>
    /// The unique ID for the current instance.
    /// </summary>
    private readonly Guid _uniqueId;
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
    /// Gets the assemblies for which cleanup is insured by the current instance.
    /// </summary>
    public IEnumerable<AssemblyName> Assemblies
    {
      get { return _assemblies; }
    }

    /// <summary>
    /// Gets the <see cref="InstallerDescription"/> needed to install and uninstall the insured assemblies.
    /// </summary>
    public InstallerDescription Installer
    {
      get { return _data.Installer; }
    }

    /// <summary>
    /// Gets the flags used to base the insurance-methods on.
    /// </summary>
    public CleanUpInsuranceFlags Flags
    {
      get { return _data.Flags; }
    }

    /// <summary>
    /// Gets the date and time on which the insurance has be created.
    /// </summary>
    public DateTime TimeStamp
    {
      get { return _timeStamp; }
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
      var flags = (_insuranceFile != null ? CleanUpInsuranceFlags.TrackByFile : CleanUpInsuranceFlags.None)
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
      _assemblies = new List<AssemblyName>(insuranceBase.Assemblies);
      _timeStamp = insuranceBase.TimeStamp;
      _uniqueId = insuranceBase.InsuranceIdentifier;
      _data = new InsuranceData(insuranceBase.InstallerDescription, flags,
                                        _insuranceFile != null ? Path.GetDirectoryName(_insuranceFile.FileName) : null,
                                        _insuranceRegistryKey != null ? Path.GetDirectoryName(_insuranceRegistryKey.RegistryKeyName) : null,
                                        null);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CleanUpInsurance"/> built from the data specified.
    /// </summary>
    /// <param name="creationData"></param>
    /// <param name="assemblyNames"></param>
    private CleanUpInsurance(InsuranceData creationData, IEnumerable<AssemblyName> assemblyNames)
    {
      _data = creationData;
      _assemblies = new List<AssemblyName>(assemblyNames);
      _timeStamp = DateTime.Now;
      _uniqueId = Guid.NewGuid();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Removes all ways of insuring from the local system, for the current <see cref="CleanUpInsurance"/>.
    /// </summary>
    /// <exception cref="ApplicationException">
    /// An <see cref="ApplicationException"/> is thrown if the GAC couldn't be cleaned while <paramref name="cleanCacheFirst"/> is set to true.
    /// </exception>
    /// <param name="cleanCacheFirst">True if the GAC needs to be clean/cleaned before disposing the insurance.</param>
    public void Dispose(bool cleanCacheFirst)
    {
      if (cleanCacheFirst && !CleanCache())
        throw new ApplicationException("Failed to clean the global assembly cache.");
      Dispose(CleanUpInsuranceFlags.All);
    }

    /// <summary>
    /// Removes all specified ways of insuring from the local system, for the current <see cref="CleanUpInsurance"/>.
    /// The specified assemblies are NOT uninstalled from the global assembly cache.
    /// </summary>
    /// <param name="flags">The ways of insuring that need to be disposed.</param>
    public void Dispose(CleanUpInsuranceFlags flags)
    {
      if (flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile))
        DisposeFileInsurance();
      if (flags.IsSpecified((CleanUpInsuranceFlags.TrackByRegistry)))
        DisposeRegistryInsurance();
      if (flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService))
        DisposeProcessInsurance();
    }

    /// <summary>
    /// Returns a <see cref="string"/> that represents the current <see cref="CleanUpInsurance"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return string.Format("[{0}] {1} ({2} items)", _timeStamp, _uniqueId, _assemblies.Count);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Cleans the global assembly cache by uninstaling the insured <see cref="Assemblies"/>.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <returns>True if the cache has been cleaned; Otherwise, false.</returns>
    private bool CleanCache()
    {
      var gac = new AssemblyCache(_data.Installer);
      var failed = false;
      foreach (var assembly in _assemblies)
      {
        if (!AssemblyCache.IsInstalled(assembly))
          continue;
        var disposition = gac.UninstallAssembly(assembly);
        failed = disposition == UninstallDisposition.HasReferences || disposition == UninstallDisposition.StillInUse
                   ? true
                   : failed;
      }
      return !failed;
    }

    private void CreateFileInsurance()
    {
      Directory.CreateDirectory(_data.TrackingFilesFolder);
      _insuranceFile = new InsuranceFile(Path.Combine(_data.TrackingFilesFolder, _uniqueId.ToString()), _uniqueId,
                                         _data.Installer, LocalMachine.Identifier, _timeStamp, _assemblies);
      InsuranceFile.Write(_insuranceFile);
    }

    private void CreateRegistryInsurance()
    {
      using (var rootKey = Registry.CurrentUser.CreateSubKey(_data.TrackingRegistryKey))
        rootKey.CreateSubKey(_uniqueId.ToString()).Close();
      _insuranceRegistryKey = new InsuranceRegistryKey(Path.Combine(_data.TrackingRegistryKey, _uniqueId.ToString()),
                                                       _uniqueId, _data.Installer, LocalMachine.Identifier,
                                                       _timeStamp, _assemblies);
      InsuranceRegistryKey.Write(_insuranceRegistryKey);
    }

    private void CreateWatchingProcessInsurance()
    {
      if (!_data.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile)
        && !_data.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry))
        CreateFileInsurance();  // Used to pass data to the watcher process,
      // the watcher process is responsible for deleting this file after usage.
      var startInfo = new ProcessStartInfo
                        {
                          FileName = _data.TrackingProcessExecutable,
                          Arguments = "\"IID=" + _uniqueId + "\" "
                                      + "\"FLAGS=" + (int)_data.Flags + "\" "
                                      + "\"FILE=" + _data.TrackingFilesFolder + "\" "
                                      + "\"REG=" + _data.TrackingRegistryKey + "\" "
                                      + "\"PID=" + Process.GetCurrentProcess().Id + "\" ",
                          CreateNoWindow = true
                        };
      _insuranceProcess = Process.Start(startInfo);
    }

    private void DisposeFileInsurance()
    {
      if (!_data.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile)
          || _insuranceFile == null
          || _insuranceFile.MachineId != LocalMachine.Identifier
          || !Directory.Exists(_data.TrackingFilesFolder))
        return;
      File.Delete(_insuranceFile.FileName);
      if (Directory.GetFiles(_data.TrackingFilesFolder).Length == 0)
        Directory.Delete(_data.TrackingFilesFolder);
    }

    private void DisposeRegistryInsurance()
    {
      if (!_data.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry)
          || _insuranceRegistryKey == null
          || _insuranceRegistryKey.MachineId != LocalMachine.Identifier)
        return;
      bool deleteTree;
      using (var key = Registry.CurrentUser.OpenSubKey(_data.TrackingRegistryKey))
      {
        if (key == null) return;
        var subKeys = key.GetSubKeyNames();
        if (subKeys.Contains(_uniqueId.ToString()))
          key.DeleteSubKeyTree(_uniqueId.ToString());
        deleteTree = subKeys.Length == 1;
      }
      if (deleteTree)
        Registry.CurrentUser.DeleteSubKey(_data.TrackingRegistryKey);
    }

    private void DisposeProcessInsurance()
    {
      if (!_data.Flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService)
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
    /// <param name="creationData"></param>
    /// <param name="assemblyNames">The assemblies to insure cleanup for.</param>
    /// <returns></returns>
    public static CleanUpInsurance CreateInsurance(InsuranceData creationData, IEnumerable<AssemblyName> assemblyNames)
    {
      var insurance = new CleanUpInsurance(creationData, assemblyNames);
      if (insurance._assemblies.Count == 0)
        return insurance;
      if (creationData.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByFile))
        insurance.CreateFileInsurance();
      if (creationData.Flags.IsSpecified(CleanUpInsuranceFlags.TrackByRegistry))
        insurance.CreateRegistryInsurance();
      if (creationData.Flags.IsSpecified(CleanUpInsuranceFlags.ByWatchService))
        insurance.CreateWatchingProcessInsurance();
      return insurance;
    }

    /// <summary>
    /// Returns all insurances found in the local system.
    /// </summary>
    /// <param name="trackingFilesFolder"></param>
    /// <param name="trackingRegistryKey"></param>
    /// <returns></returns>
    public static List<CleanUpInsurance> LoadFromSystem(string trackingFilesFolder, string trackingRegistryKey)
    {
      // Find valid InsuranceFiles
      var files = trackingFilesFolder != null
                    ? GetFileInsurances(trackingFilesFolder)
                    : new List<InsuranceFile>(0);
      // Find valid InsuranceRegistryKeys
      var keys = trackingRegistryKey != null
                   ? GetRegistryInsurances(trackingRegistryKey)
                   : new List<InsuranceRegistryKey>(0);
      // Build the result
      var result = new List<CleanUpInsurance>();
      // - Enumerate all files, while trying to find matching registrykeys
      foreach (var file in files)
      {
        var key = keys.FirstOrDefault(k => file.InsuranceIdentifier.Equals(k.InsuranceIdentifier));
        keys.Remove(key);
        result.Add(new CleanUpInsurance(file, key, null));
      }
      // - Now enumerate the keys that didn't match to a file.
      foreach (var key in keys)
        result.Add(new CleanUpInsurance(null, key, null));
      return result;
    }

    /// <summary>
    /// Returns the <see cref="CleanUpInsurance"/> matching the identifier specified.
    /// If no match is found, null is returned.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <param name="trackingFilesFolder"></param>
    /// <param name="trackingRegistryKey"></param>
    /// <param name="insuranceId"></param>
    /// <returns></returns>
    public static CleanUpInsurance LoadFromSystem(string trackingFilesFolder, string trackingRegistryKey, Guid insuranceId)
    {
      if (insuranceId == Guid.Empty)
        throw new ArgumentException("The specified insurance identifier is not a valid GUID", "insuranceId");
      InsuranceFile insuranceFile = null;
      InsuranceRegistryKey insuranceRegKey = null;
      // Load from file
      if (trackingFilesFolder != null && File.Exists(Path.Combine(trackingFilesFolder, insuranceId.ToString())))
        InsuranceFile.TryRead(Path.Combine(trackingFilesFolder, insuranceId.ToString()), out insuranceFile);
      // Load from registry
      if (trackingRegistryKey != null)
        using (var regKey = Registry.CurrentUser.OpenSubKey(trackingRegistryKey + insuranceId, false))
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

    /// <summary>
    /// Returns a list of <see cref="InsuranceFile"/>s which are read from the specified <paramref name="folder"/>.
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    private static List<InsuranceFile> GetFileInsurances(string folder)
    {
      if (!Directory.Exists(folder))
        return new List<InsuranceFile>(0);
      var identifiers = Directory.GetFiles(folder);
      var files = new List<InsuranceFile>();
      foreach (var file in identifiers)
      {
        InsuranceFile insuranceFile;
        if (InsuranceFile.TryRead(file, out insuranceFile))
          files.Add(insuranceFile);
      }
      return files;
    }

    /// <summary>
    /// Returns a list of <see cref="InsuranceRegistryKey"/>s which are read from the registrykey with the specified name.
    /// </summary>
    /// <param name="regKeyName">The name of a subkey of the CurrentUser rootkey.</param>
    /// <returns></returns>
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

    #region IDisposable Members

    /// <summary>
    /// Removes all traces of the current <see cref="CleanUpInsurance"/> from the local system.
    /// </summary>
    public void Dispose()
    {
      Dispose(true);
    }

    #endregion

  }
}
