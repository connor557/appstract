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
using System.Linq;
using AppStract.Host.Data.Application;
using AppStract.Host.Virtualization.Connection;
using AppStract.Engine.Data.Connection;

namespace AppStract.Host.Virtualization.Process.Packaging
{
  /// <summary>
  /// Derived from <see cref="VirtualizedProcess"/>
  /// and extended with functionality specific to the packaging of applications.
  /// </summary>
  internal sealed class PackagingProcess : VirtualizedProcess
  {

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="PackagingProcess"/>.
    /// </summary>
    /// <param name="startInfo">
    /// The <see cref="VirtualProcessStartInfo"/> containing the information used to start the process with.
    /// </param>
    /// <param name="synchronizer">
    /// The <see cref="ProcessSynchronizer"/> to use for data synchronization with the <see cref="VirtualizedProcess"/>.
    /// </param>
    private PackagingProcess(VirtualProcessStartInfo startInfo, IProcessSynchronizer synchronizer)
      : base(startInfo, synchronizer)
    {

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Starts a new <see cref="PackagingProcess"/> from the <see cref="VirtualProcessStartInfo"/> specified.
    /// </summary>
    /// <param name="startInfo">
    /// The <see cref="VirtualProcessStartInfo"/> containing the information used to start the process with.
    /// </param>
    /// <returns>
    /// A new <see cref="PackagingProcess"/> component that is associated with the process resource.
    /// </returns>
    public new static PackagingProcess Start(VirtualProcessStartInfo startInfo)
    {
      if (startInfo.Files.RegistryDatabase.Type != FileType.Database)
        throw new ArgumentException("The destination file specified for the registry database is not valid.",
                                    "startInfo");
      var synchronizer = new ProcessSynchronizer(startInfo.WorkingDirectory, startInfo.FileSystemRuleCollection,
                                                 startInfo.Files.RegistryDatabase, startInfo.RegistryRuleCollection);
      var process = new PackagingProcess(startInfo, synchronizer);
      process.Start();
      return process;
    }

    /// <summary>
    /// Returns all the executables that were detected during packaging.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetExecutables()
    {
      try
      {
        var root = _connection.ProcessSynchronizer.ConnectionStrings[ConfigurationDataType.FileSystemRoot];
        var executables = Directory.GetFiles(root, @"*.exe", SearchOption.AllDirectories);
        if (executables.Length == 0)
          HostCore.Log.Critical("Unable to retrieve a list of executables!");
        return executables.Select(value => value.Substring(root.Length));
      }
      catch
      {
        return new string[0];
      }
    }

    #endregion

  }
}
