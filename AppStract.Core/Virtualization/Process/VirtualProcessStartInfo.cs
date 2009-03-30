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
using AppStract.Core.Data.Application;

namespace AppStract.Core.Virtualization.Process
{
  /// <summary>
  /// Specifies a set of values that are used when starting a <see cref="VirtualizedProcess"/>.
  /// </summary>
  public class VirtualProcessStartInfo
  {

    #region Variables

    private ApplicationFile _executable;
    private ApplicationFile _dbFileSystem;
    private ApplicationFile _dbRegistry;
    private ApplicationFile _workingDirectory;
    private string _arguments;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the executable to start the process with.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public ApplicationFile Executable
    {
      get { return _executable; }
      set
      {
        if (value.Type != FileType.Assembly_Managed
            || value.Type != FileType.Assembly_Native)
          throw new ArgumentException(
            "The value specified is an illegal type for the main executable.",
            "value");
        _executable = value;
      }
    }

    /// <summary>
    /// Gets or sets the location of the database containing all known file system entries.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public ApplicationFile DatabaseFileSystem
    {
      get { return _dbFileSystem; }
      set
      {
        if (value.Type != FileType.Database)
          throw new ArgumentException(
            "The value specified is an illegal type for the file system database.",
            "value");
        _dbFileSystem = value;
      }
    }

    /// <summary>
    /// Gets or sets the location of the database containing all known registry entries.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public ApplicationFile DatabaseRegistry
    {
      get { return _dbRegistry; }
      set
      {
        if (value.Type != FileType.Database)
          throw new ArgumentException(
            "The value specified is an illegal type for the registry database.",
            "value");
        _dbRegistry = value;
      }
    }

    /// <summary>
    /// Gets or sets the container's root from which the process starts.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public ApplicationFile WorkingDirectory
    {
      get { return _workingDirectory; }
      set
      {
        if (value.Type != FileType.Directory)
          throw new ArgumentException("The working directory specified is not a directory.",
                                      "value");
        _workingDirectory = value;
      }
    }

    /// <summary>
    /// Gets or sets the set of command-line arguments to use when starting the application.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    public string Arguments
    {
      get { return _arguments; }
      set
      {
        if (_arguments == null)
          throw new NullReferenceException();
        _arguments = value;
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="VirtualProcessStartInfo"/>
    ///  based on the <see cref="ApplicationData"/> specified.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="data"></param>
    public VirtualProcessStartInfo(ApplicationData data)
    {
      if (data ==null)
        throw new ArgumentNullException("data");
      if (data.Files.ExeMain.Type != FileType.Assembly_Managed
          || data.Files.ExeMain.Type != FileType.Assembly_Native)
        throw new ArgumentException("The ApplicationData specified contains an illegal value for the main executable.",
                                    "data");
      if (data.Files.DatabaseFileSystem.Type != FileType.Database)
        throw new ArgumentException("The ApplicationData specified contains an illegal value for the file system database.",
                                    "data");
      if (data.Files.DatabaseRegistry.Type != FileType.Database)
        throw new ArgumentException("The ApplicationData specified contains an illegal value for the registry database.",
                                    "data");
      _executable = data.Files.ExeMain;
      _dbFileSystem = data.Files.DatabaseFileSystem;
      _dbRegistry = data.Files.DatabaseRegistry;
      _arguments = "";
      _workingDirectory = new ApplicationFile();
    }

    /// <summary>
    /// Initializes a new instance of <see cref="VirtualProcessStartInfo"/>
    ///  based on the <see cref="ApplicationData"/> specified.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="data"></param>
    /// <param name="workingDirectory"></param>
    public VirtualProcessStartInfo(ApplicationData data, ApplicationFile workingDirectory)
      : this(data)
    {
      if (data == null)
        throw new ArgumentNullException("data");
      if (workingDirectory == null)
        throw new ArgumentNullException("workingDirectory");
      if (workingDirectory.Type != FileType.Directory)
        throw new ArgumentException("The working directory specified is not a directory.",
                                    "workingDirectory");
      _workingDirectory = workingDirectory;
      _arguments = "";
    }

    #endregion

  }
}
