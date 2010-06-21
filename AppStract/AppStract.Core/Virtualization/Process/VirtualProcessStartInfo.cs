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
using System.IO;
using AppStract.Core.Data.Application;
using AppStract.Core.Virtualization.Engine.Registry;

namespace AppStract.Core.Virtualization.Process
{
  /// <summary>
  /// Specifies a set of values that are used when starting a <see cref="VirtualizedProcess"/>.
  /// </summary>
  public sealed class VirtualProcessStartInfo
  {

    #region Variables

    private readonly ApplicationFiles _files;
    private ApplicationFile _workingDirectory;
    private string _arguments;
    private RegistryRuleCollection _registryRuleCollection;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the <see cref="ApplicationFiles"/> associated with the to be started <see cref="VirtualizedProcess"/>.
    /// All files are defined with absolute paths.
    /// </summary>
    public ApplicationFiles Files
    {
      get { return _files; }
    }

    /// <summary>
    /// Gets or sets the container's root from which the process starts.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public ApplicationFile WorkingDirectory
    {
      get { return _workingDirectory; }
      set
      {
        if (value == null)
          throw new ArgumentNullException();
        if (value.Type != FileType.Directory)
          throw new ArgumentException("The working directory specified is not a directory.",
                                      "value");
        _workingDirectory = value;
      }
    }

    /// <summary>
    /// Gets or sets the set of command-line parameters to use when starting the application.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public string Arguments
    {
      get { return _arguments; }
      set
      {
        if (_arguments == null)
          throw new ArgumentNullException();
        _arguments = value;
      }
    }

    /// <summary>
    /// Gets the collection of engine rules to apply on the virtual registry of any <see cref="VirtualizedProcess"/>
    /// started with the current <see cref="VirtualProcessStartInfo"/>.
    /// </summary>
    public RegistryRuleCollection RegistryRuleCollection
    {
      get { return _registryRuleCollection; }
      set
      {
        if (_registryRuleCollection == null)
          throw new ArgumentNullException();
        _registryRuleCollection = value;
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="VirtualProcessStartInfo"/>
    ///  based on the <see cref="ApplicationData"/> specified.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// An <see cref="ArgumentNullException"/> is thrown if <paramref name="data"/> of one of its properties is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if any of the properties of <paramref name="data"/> is of the wrong type.
    /// </exception>
    /// <param name="data">The data to base the process on.</param>
    /// <param name="workingDirectory">The working directory of the process to start.</param>
    public VirtualProcessStartInfo(ApplicationData data, ApplicationFile workingDirectory)
    {
      if (data == null
          || data.Files.DatabaseFileSystem == null
          || data.Files.DatabaseRegistry == null
          || data.Files.Executable == null
          || data.Files.RootDirectory == null)
        throw new ArgumentNullException("data", "The data argument or one of its properties is null.");
      if (workingDirectory == null)
        throw new ArgumentNullException("workingDirectory", "The workingDirectory argument is null.");
      if (data.Files.Executable.Type != FileType.Assembly_Managed
          && data.Files.Executable.Type != FileType.Assembly_Native)
        throw new ArgumentException("The ApplicationData specified contains an illegal value for the main executable.",
                                    "data");
      if (data.Files.DatabaseFileSystem.Type != FileType.Database)
        throw new ArgumentException(
          "The ApplicationData specified contains an illegal value for the file system database.",
          "data");
      if (data.Files.DatabaseRegistry.Type != FileType.Database)
        throw new ArgumentException(
          "The ApplicationData specified contains an illegal value for the registry database.",
          "data");
      if (workingDirectory.Type != FileType.Directory)
        throw new ArgumentException("The working directory specified is not a directory.",
                                    "workingDirectory");
      _files = new ApplicationFiles
                 {
                   DatabaseFileSystem
                     = new ApplicationFile(Path.Combine(workingDirectory.FileName, data.Files.DatabaseFileSystem.FileName)),
                   DatabaseRegistry
                     = new ApplicationFile(Path.Combine(workingDirectory.FileName, data.Files.DatabaseRegistry.FileName)),
                   Executable
                     = new ApplicationFile(Path.Combine(workingDirectory.FileName, data.Files.Executable.FileName)),
                   RootDirectory
                     = new ApplicationFile(Path.Combine(workingDirectory.FileName, data.Files.RootDirectory.FileName))
                 };
      _arguments = "";
      _workingDirectory = workingDirectory;
      _registryRuleCollection = data.Settings.RegistryEngineRuleCollection ?? RegistryRuleCollection.GetDefaultRuleCollection();
    }

    #endregion

  }
}
