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

namespace AppStract.Server.FileSystem
{
  /// <summary>
  /// Provides information about, and means to manipulate,
  /// the current virtual environment and platform.
  /// </summary>
  public static class VirtualEnvironment
  {

    #region Public Methods

    /// <summary>
    /// Tries to create all system-folders, as defined in <see cref="VirtualFolder"/>.
    /// </summary>
    /// <param name="rootFolder">Rootfolder for the virtual folders.</param>
    /// <returns>True if all folders are created; False if the creation of one or more folders failed.</returns>
    public static bool CreateVirtualFolders(string rootFolder)
    {
      GuestCore.Log.Message("Creating system folders for a virtual environment with root \"{0}\"", rootFolder);
      bool succeeded = true;
      foreach (VirtualFolder virtualFolder in Enum.GetValues(typeof(VirtualFolder)))
        if (!TryCreateDirectory(Path.Combine(rootFolder, virtualFolder.ToPath())))
        {
          GuestCore.Log.Critical("Failed to create virtual system folder: " + virtualFolder);
          succeeded = false;
        }
      return succeeded;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tries to create the directory, specified by <paramref name="path"/>.
    /// </summary>
    /// <param name="path">Directory to create.</param>
    /// <returns>True if the directory is created; False, otherwise.</returns>
    private static bool TryCreateDirectory(string path)
    {
      try
      {
        if (!Directory.Exists(path))
          Directory.CreateDirectory(path);
        return true;
      }
      catch (IOException)
      {
        return false;
      }
    }

    #endregion

  }
}