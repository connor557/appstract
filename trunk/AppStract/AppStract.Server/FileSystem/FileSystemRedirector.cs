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
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Utilities.Extensions;

namespace AppStract.Server.FileSystem
{
  /// <summary>
  /// Redirects calls to the host's file system to locations as used in the virtual file system.
  /// </summary>
  public class FileSystemRedirector
  {

    #region Variables

    /// <summary>
    /// Contains all known system variables.
    /// The keys are the variables used in the real file system,
    /// while the associated values are the variables used by the virtual file system.
    /// </summary>
    private readonly IDictionary<string, VirtualFolder> _systemVariables;

    #endregion

    #region Constructors

    public FileSystemRedirector()
    {
      _systemVariables = InitializeSystemVariables();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the replacement path for the specified <paramref name="request"/>.
    /// The result is relative to the virtual file system's root directory.
    /// </summary>
    /// <param name="request">The <see cref="FileRequest"/> to redirect.</param>
    /// <param name="root"></param>
    /// <returns>The replacement path, used for redirection.</returns>
    public FileRequestResult Redirect(FileRequest request, string root)
    {
      var result = new FileRequestResult {Request = request};
      string systemFolder;
      if (request.Path.StartsWithAny(_systemVariables.Keys, out systemFolder, true))
      {
        result.SystemFolder = _systemVariables[systemFolder];
        result.Path = _systemVariables[systemFolder].ToPath() +
                                (request.Path.Length > systemFolder.Length
                                   ? request.Path.Substring(systemFolder.Length + 1).ToLowerInvariant()
                                   : "");
      }
      else
      {
        result.SystemFolder = VirtualFolder.Other;
        result.Path = RedirectToDefaultFolder(request.Path);
      }
      result.Path = Path.Combine(root, result.Path);
      return result;
    }

    /// <summary>
    /// Returns a string representation of the current static instance of <see cref="FileSystemRedirector"/>.
    /// </summary>
    /// <remarks>
    /// Intended for debug use only.
    /// </remarks>
    /// <returns></returns>
    public override string ToString()
    {
      string result = "#Entries: " + _systemVariables.Count;
      foreach (var pair in _systemVariables)
        result += "\n\r" + pair.Key + "   -   " + pair.Value;
      return result;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns an initialized <see cref="IDictionary{TKey,TValue}"/>,
    /// filled with all known system variables.
    /// </summary>
    /// <returns></returns>
    private static IDictionary<string, VirtualFolder> InitializeSystemVariables()
    {
      IDictionary<string, VirtualFolder> systemVariables = new Dictionary<string, VirtualFolder>();
      string tmp; // Will contain the temporary values used in this method.

      // Always check if the dictionary doesn't already contain the same key.
      // The users might have configured the specialfolders to use the same folder.
      // BUG: Such configurations might lead to inconsistencies between different host systems.
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserDocuments);

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserPictures);

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserMusic);

      // UserData
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.UserData);

      // Application Data
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.ApplicationData);

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
        systemVariables.Add(tmp, VirtualFolder.ApplicationData);

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.ApplicationData);

      // Temporary Folder
      tmp = Path.GetTempPath();
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.Temporary);


      // Program Files
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).ToLowerInvariant();
      if (!systemVariables.ContainsKey(tmp))
        systemVariables.Add(tmp, VirtualFolder.ProgramFiles);

      // System
      tmp = Environment.GetEnvironmentVariable("systemroot");
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.System);

      tmp = Environment.GetFolderPath(Environment.SpecialFolder.System).ToLowerInvariant();
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp))
        systemVariables.Add(tmp, VirtualFolder.System32);

      // Start Menu
      tmp = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.StartMenu);

      tmp = GetCommonMenuFolder();
      if (!string.IsNullOrEmpty(tmp) && !systemVariables.ContainsKey(tmp.ToLowerInvariant()))
        systemVariables.Add(tmp.ToLowerInvariant(), VirtualFolder.StartMenu);

      return systemVariables;
    }

    /// <summary>
    /// Returns the replacement path to the default folder, for the specified <paramref name="path"/>.
    /// The default path is the value for <see cref="VirtualFolder.Other"/>.
    /// </summary>
    /// <param name="path">Path to redirect to the default folder.</param>
    /// <returns>Redirected path.</returns>
    private static string RedirectToDefaultFolder(string path)
    {
      string otherFolder = VirtualFolder.Other.ToPath();
      string fileExtension = Path.GetExtension(path);
      int cnt = 0; // Used as a counter 'till a unique filename is constructed.
      string uniqueValue = ""; // The value to be added at the end of the filename, in order to get a unique path.
      // Is the path a directory?
      if (fileExtension == null)
      {
        int index = path.LastIndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
        if (index == -1 || index == path.Length - 1)
          // It's a root path, return the path of VirtualFolder.Other
          return otherFolder;
        string directory = path.Substring(index);
        while (Directory.Exists(otherFolder + directory + uniqueValue))
          uniqueValue = cnt++.ToString();
        return otherFolder + directory + uniqueValue;
      }
      // Else, the path is a file.
      string filename = Path.GetFileNameWithoutExtension(path);
      while (File.Exists(otherFolder + filename + uniqueValue + fileExtension))
        uniqueValue = cnt++.ToString();
      return otherFolder + filename + uniqueValue + fileExtension;
    }

    /// <summary>
    /// Returns the location of the common menu folder.
    /// </summary>
    /// <returns></returns>
    private static string GetCommonMenuFolder()
    {
      var value
        = Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\explorer\Shell Folders",
        "Common Start Menu", null);
      if (value != null)
        return value.ToString();
      return null;
    }

    #endregion

  }
}
