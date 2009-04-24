#region Copyright (C) 2008-2009 Simon Allaeys

/*
    Copyright (C) 2008-2009 Simon Allaeys
 
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
using AppStract.Core.Data.Application;
using AppStract.Core.Virtualization.Process;

namespace AppStract.Core.Virtualization.Packaging
{
  /// <summary>
  /// Class able to package an application.
  /// To achieve this, the application's installer is executed
  /// and the data is intercepted to a <see cref="PackagedApplication"/>.
  /// </summary>
  public class Packager
  {

    #region Constants

    private const string _dbFileSystem = "AppStract.FileSystem.db3";
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
    {
      ApplicationData data = new ApplicationData();
      var workingDirectory = new ApplicationFile(outputFolder);
      if (workingDirectory.Type != FileType.Directory)
        throw new ArgumentException("The value specified for the outputFolder is invalid.", "outputFolder");
      data.Files.RootDirectory = new ApplicationFile("");
      data.Files.Executable = new ApplicationFile(executable);
      if (data.Files.Executable.Type != FileType.Assembly_Managed
          && data.Files.Executable.Type != FileType.Assembly_Native)
        throw new ArgumentException("The value specified for the executable is invalid.", "executable");
      data.Files.DatabaseFileSystem = new ApplicationFile(_dbFileSystem);
      data.Files.DatabaseRegistry = new ApplicationFile(_dbRegistry);
      _startInfo = new VirtualProcessStartInfo(data, workingDirectory);
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

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles the <see cref="_process"/>.Exited event.
    /// If packaging succeeded (<see cref="_succeeded"/>), <see cref="_result"/> is set from the retreived data.
    /// </summary>
    /// <param name="sender">The exited <see cref="VirtualizedProcess"/>.</param>
    /// <param name="exitCode">The associated <see cref="ExitCode"/>.</param>
    private void Process_Exited(VirtualizedProcess sender, ExitCode exitCode)
    {
      if (sender != _process)
        throw new ApplicationException("An unexpected exception occured in the application workflow."
                                       + " Process_Exited event is called from an unknown Process."
                                       + " Please contact the developers about this issue.");
      _succeeded = exitCode == ExitCode.Success;
      if (_succeeded)
      {
        _result = new PackagedApplication(_startInfo.WorkingDirectory.File,
                                          _process.GetExecutables(), /// We already checked if sender equals _process.
                                          _startInfo.Files.DatabaseFileSystem.File,
                                          _startInfo.Files.DatabaseRegistry.File);
      }
      _waitHandle.Set();
    }

    #endregion

  }
}
