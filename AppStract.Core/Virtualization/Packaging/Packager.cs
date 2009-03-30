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
  public class Packager
  {

    #region Constants

    private const string _dbFileSystem = "AppStract.FileSystem.db3";
    private const string _dbRegistry = "AppStract.Registry.db3";

    #endregion

    #region Variables

    private readonly VirtualProcessStartInfo _startInfo;
    private readonly AutoResetEvent _waitHandle;
    private PackagingProcess _process;
    private PackagedApplication _result;
    private bool _succeeded;

    #endregion

    #region Constructors

    public Packager(string executable, string outputFolder)
    {
      ApplicationFile workingDirectory = new ApplicationFile(outputFolder);
      if (workingDirectory.Type != FileType.Directory)
        throw new ArgumentException("The value specified for the outputFolder is invalid.", "outputFolder");
      ApplicationData data = new ApplicationData();
      data.Files.ExeMain.File = executable;
      if (data.Files.ExeMain.Type != FileType.Assembly_Managed
          || data.Files.ExeMain.Type != FileType.Assembly_Native)
        throw new ArgumentException("The value specified for the executable is invalid.", "executable");
      data.Files.DatabaseFileSystem.File = _dbFileSystem;
      data.Files.DatabaseRegistry.File = _dbRegistry;
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
      ThreadPool.QueueUserWorkItem(StartPackaging);
      WaitHandle.WaitAll(new[] { _waitHandle });
      if (!_succeeded)
        throw new PackageException("Packaging of " + _startInfo + " did not succeed.");
      return _result;
    }

    #endregion

    #region Private Methods

    private void StartPackaging(object state)
    {
      _process = PackagingProcess.Start(_startInfo);
      _process.Exited += Process_Exited;
    }

    private void Process_Exited(VirtualizedProcess sender, ExitCode exitCode)
    {
      if (sender != _process)
        throw new ApplicationException("An unexpected exception occured in the application workflow."
                                       + " Expected object of type "
                                       + _process.GetType()
                                       + " but received an object of type "
                                       + sender.GetType()
                                       + " Please contact the developers about this issue.");
      _succeeded = exitCode == ExitCode.Success;
      if (!_succeeded)
        return;
      _result = new PackagedApplication(_startInfo.WorkingDirectory.File,
                                        _process.GetExecutables(), /// We already checked if sender equals _process.
                                        _startInfo.DatabaseFileSystem.File,
                                        _startInfo.DatabaseRegistry.File);
      _waitHandle.Set();
    }

    #endregion

  }
}
