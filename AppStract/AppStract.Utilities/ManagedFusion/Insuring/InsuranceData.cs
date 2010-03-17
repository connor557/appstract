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

namespace AppStract.Utilities.ManagedFusion.Insuring
{
  /// <summary>
  /// Holds settings and specific data needed by the <see cref="CleanUpInsurance"/> class.
  /// </summary>
  public class InsuranceData
  {

    #region Variables

    private readonly InstallerDescription _installer;
    private readonly CleanUpInsuranceFlags _flags;
    private readonly string _trackingFilesFolder;
    private readonly string _trackingRegistryKey;
    private readonly string _trackingProcessExe;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the <see cref="InstallerDescription"/> for the application that's installing and uninstalling the insured assemblies.
    /// </summary>
    public InstallerDescription Installer
    {
      get { return _installer; }
    }

    /// <summary>
    /// Gets the flags to base the method of insurance on.
    /// </summary>
    public CleanUpInsuranceFlags Flags
    {
      get { return _flags; }
    }

    /// <summary>
    /// Gets the folder containing all files used to track insurances.
    /// If <see cref="Flags"/> specifies the <see cref="CleanUpInsuranceFlags.TrackByFile"/> flag, a new insurance file is be added to this folder.
    /// </summary>
    public string TrackingFilesFolder
    {
      get { return _trackingFilesFolder; }
    }

    /// <summary>
    /// Gets the registry key containing all keys used to track insurances.
    /// If <see cref="Flags"/> specifies the <see cref="CleanUpInsuranceFlags.TrackByRegistry"/> flag, a new subkey is be added to this key.
    /// </summary>
    public string TrackingRegistryKey
    {
      get { return _trackingRegistryKey; }
    }

    /// <summary>
    /// Gets the filename of the executable to start a watcher process with,
    /// in case <see cref="Flags"/> specifies the <see cref="CleanUpInsuranceFlags.ByWatchService"/> flag.
    /// </summary>
    public string TrackingProcessExecutable
    {
      get { return _trackingProcessExe; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="InsuranceData"/>,
    /// which can be used with <see cref="CleanUpInsurance.CreateInsurance"/>.
    /// </summary>
    /// <param name="usingInstaller">The <see cref="InstallerDescription"/> for the application that's installing and uninstalling the insured assemblies.</param>
    /// <param name="flags">The flags to base the method of insurance on.</param>
    /// <param name="trackingFilesFolder">The folder containing all files used to track insurances.</param>
    /// <param name="trackingRegistryKey">The registry key containing all keys used to track insurances.</param>
    /// <param name="trackingProcessExecutable">The filename of the executable to use when starting a watcher process.</param>
    public InsuranceData(InstallerDescription usingInstaller, CleanUpInsuranceFlags flags, string trackingFilesFolder, string trackingRegistryKey, string trackingProcessExecutable)
    {
      _installer = usingInstaller;
      _flags = flags;
      _trackingFilesFolder = trackingFilesFolder;
      _trackingRegistryKey = trackingRegistryKey;
      _trackingProcessExe = trackingProcessExecutable;
    }

    #endregion

  }
}
