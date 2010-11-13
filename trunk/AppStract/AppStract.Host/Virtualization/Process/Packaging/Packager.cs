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
using System.Threading;
using AppStract.Host.Data.Application;
using AppStract.Engine.Virtualization;
using AppStract.Engine.Configuration;

namespace AppStract.Host.Virtualization.Process.Packaging
{
  /// <summary>
  /// Class able to package an application.
  /// To achieve this, the application's installer is executed
  /// and the data is intercepted to a <see cref="PackagedApplication"/>.
  /// </summary>
  public sealed class Packager
  {

    #region Constants

    /// <summary>
    /// The default filename of the database containing the registry data.
    /// </summary>
    private const string _dbRegistry = "AppStract.Registry.db3";

    #endregion

    #region Variables

    /// <summary>
    /// The <see cref="VirtualProcessStartInfo"/> used to start <see cref="_process"/> with.
    /// </summary>
    private readonly VirtualProcessStartInfo _startInfo;
    /// <summary>
    /// Used to let <see cref="CreatePackage"/> wait for the packaging to end.
    /// </summary>
    private readonly AutoResetEvent _waitHandle;
    /// <summary>
    /// The <see cref="PackagingProcess"/> virtualizing the installer's process.
    /// </summary>
    private PackagingProcess _process;
    /// <summary>
    /// The result after packaging, if <see cref="_succeeded"/>.
    /// </summary>
    private PackagedApplication _result;
    /// <summary>
    /// Whether packaging succeeded.
    /// </summary>
    private bool _succeeded;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="Packager"/>.
    /// </summary>
    /// <param name="executable">The application's installer executable.</param>
    /// <param name="outputFolder">The location where the application must be packaged to.</param>
    public Packager(string executable, string outputFolder)
      : this(GetDefaultApplicationData(executable), outputFolder)
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="Packager"/>.
    /// </summary>
    /// <param name="applicationData">The data to base the packaging process on.</param>
    /// <param name="outputFolder">The location where the application must be packaged to.</param>
    public Packager(ApplicationData applicationData, string outputFolder)
    {
      var workingDirectory = new ApplicationFile(outputFolder);
      if (workingDirectory.Type != FileType.Directory)
        throw new ArgumentException("The value specified for the outputFolder is invalid.", "outputFolder");
      _startInfo = new VirtualProcessStartInfo(applicationData, workingDirectory);
      _waitHandle = new AutoResetEvent(false);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a <see cref="PackagedApplication"/> for the application specified in the
    /// current instance's constructor. The method waits to return until the packaging process exited.
    /// </summary>
    /// <exception cref="PackageException">A <see cref="PackageException"/> is thrown when packaging failed.</exception>
    /// <returns></returns>
    public PackagedApplication CreatePackage()
    {
      _process = PackagingProcess.Start(_startInfo);
      _process.Exited += Process_Exited;
      WaitHandle.WaitAll(new[] { _waitHandle });
      if (!_succeeded)
        throw new PackageException("Packaging of " + _startInfo + " did not succeed.");
      return _result;
    }

    /// <summary>
    /// Returns a default instance of <see cref="ApplicationData"/> for the packaging of <see cref="executable"/>.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if the value specified for <paramref name="executable"/> is invalid.
    /// </exception>
    /// <param name="executable"></param>
    /// <returns></returns>
    public static ApplicationData GetDefaultApplicationData(string executable)
    {
      if (string.IsNullOrEmpty(executable))
        throw new ArgumentNullException("executable");
      ApplicationData data = new ApplicationData();
      data.Settings.RegistryEngineRuleCollection = RegistryRuleCollection.GetDefaultRuleCollection();
      data.Files.RootDirectory = new ApplicationFile(".");
      data.Files.Executable = new ApplicationFile(executable);
      if (data.Files.Executable.Type != FileType.Assembly_Managed
          && data.Files.Executable.Type != FileType.Assembly_Native)
        throw new ArgumentException("The value specified for the executable is invalid.", "executable");
      data.Files.RegistryDatabase = new ApplicationFile(_dbRegistry);
      return data;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the <see cref="_process"/>.Exited event.
    /// If packaging succeeded (<see cref="_succeeded"/>), <see cref="_result"/> is set from the retreived data.
    /// </summary>
    /// <param name="sender">The exited <see cref="VirtualizedProcess"/>.</param>
    /// <param name="exitCode">The associated <see cref="NativeResultCode"/>.</param>
    private void Process_Exited(VirtualizedProcess sender, int exitCode)
    {
      if (sender != _process)
        throw new ApplicationException("An unexpected exception occured in the application workflow."
                                       + " Process_Exited event is called from an unknown Process.");
      _succeeded = exitCode == (int)NativeResultCode.Success;
      _result = !_succeeded
                  ? null
                  : new PackagedApplication(_startInfo.WorkingDirectory.FileName,
                                            _process.GetExecutables(),
                                            _startInfo.Files.RegistryDatabase.FileName);
      _waitHandle.Set();
      sender.Dispose();
    }

    #endregion

  }
}
