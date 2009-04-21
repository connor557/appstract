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
using System.Collections.Generic;
using System.IO;
using AppStract.Utilities.Extensions;

namespace AppStract.Server.FileSystem
{
  /// <summary>
  /// Redirects calls to the host's file system
  /// to locations as used in the virtual file system.
  /// </summary>
  public static class FileAccessRedirector
  {

    #region Variables

    /// <summary>
    /// Contains all known system variables.
    /// The keys are the variables used in the real file system,
    /// while the associated values are the variables used by the virtual file system.
    /// </summary>
    private static readonly IDictionary<string, string> _systemVariables;

    #endregion

    #region Constructors

    static FileAccessRedirector()
    {
      _systemVariables = InitializeSystemVariables();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the replacement path for the specified <paramref name="path"/>.
    /// The result is relative to the virtual file system's root directory.
    /// </summary>
    /// <param name="path">The path to redirect.</param>
    /// <returns>The replacement path, used for redirection.</returns>
    public static string Redirect(string path)
    {
      string newPath;
      if (path.StartsWithAny(_systemVariables.Keys, out newPath, true))
        return (newPath + path.Substring(newPath.Length)).ToLowerInvariant();
      return RedirectToDefaultFolder(path).ToLowerInvariant();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns an initialized <see cref="IDictionary{TKey,TValue}"/>,
    /// filled with all known system variables.
    /// </summary>
    /// <returns></returns>
    private static IDictionary<string, string> InitializeSystemVariables()
    {
      IDictionary<string, string> systemVariables = new Dictionary<string, string>();
      string tmp; // Will contain the temporary values used in this method.

      /// UserData
      systemVariables.Add(
        Environment.GetFolderPath(Environment.SpecialFolder.Personal).ToLowerInvariant(),
        VirtualEnvironment.GetFolderPath(VirtualFolder.UserData));

      /// From now on, always check if the dictionary doesn't already contain the same key.
      /// The users might have configured the specialfolders to use the same folder.
      /// BUG: Such configurations might lead to inconsistencies between different host systems.
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.UserData) + @"Documents\");
      }

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.UserData) + @"Pictures\");
      }

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.UserData) + @"Music\");
      }

      /// Application Data
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.ApplicationData));
      }

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.ApplicationData));
      }

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.ApplicationData));
      }

      /// Program Files
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.ProgramFiles));
      }

      /// System
      tmp = Environment.GetEnvironmentVariable("systemroot");
      if (tmp != null && !systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(
          tmp.ToLowerInvariant(),
          VirtualEnvironment.GetFolderPath(VirtualFolder.System));
      }
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.System).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.System) + @"System32\");
      }
      /// Start Menu
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
      {
        systemVariables.Add(tmp,
          VirtualEnvironment.GetFolderPath(VirtualFolder.StartMenu));
      }
      if (EnvironmentExtender.TryGetAllUsersMenuFolder(out tmp)
        && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
      {
        systemVariables.Add(tmp.ToLowerInvariant(),
          VirtualEnvironment.GetFolderPath(VirtualFolder.StartMenu));
      }

      return systemVariables;
    }

    /// <summary>
    /// Returns the replacement path to the default folder, for the specified <paramref name="path"/>.
    /// The default path is the value for <see cref="VirtualFolder.Other"/>,
    /// retrieved with <see cref="VirtualEnvironment.GetFolderPath"/>.
    /// </summary>
    /// <param name="path">Path to redirect to the default folder.</param>
    /// <returns>Redirected path.</returns>
    private static string RedirectToDefaultFolder(string path)
    {
      string otherFolder = VirtualEnvironment.GetFolderPath(VirtualFolder.Other);
      string fileExtension = Path.GetExtension(path);
      int cnt = 0; // Used as a counter 'till a unique filename is constructed.
      string uniqueValue = ""; // The value to be added at the end of the filename, in order to get a unique path.
      /// Is the path a directory?
      if (fileExtension == null)
      {
        int index = path.LastIndexOfAny(new[] {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar});
        if (index == -1 || index == path.Length - 1)
          // It's a root path, return the path of VirtualFolder.Other
          return otherFolder;
        string directory = path.Substring(index);
        while (Directory.Exists(otherFolder + directory + uniqueValue))
          uniqueValue = cnt++.ToString();
        return otherFolder + directory + uniqueValue;
      }
      /// Else, the path is a file.
      string filename = Path.GetFileNameWithoutExtension(path);
      while (File.Exists(otherFolder + filename + uniqueValue + fileExtension))
        uniqueValue = cnt++.ToString();
      return otherFolder + filename + uniqueValue + fileExtension;
    }

    #endregion

  }
}
