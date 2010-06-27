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

namespace AppStract.Server.FileSystem
{
  /// <summary>
  /// Represents a system-folder of the virtual file system.
  /// </summary>
  public enum VirtualFolder
  {
    System,
    System32,
    ProgramFiles,
    ApplicationData,
    UserData,
    UserDocuments,
    UserPictures,
    UserMusic,
    Other,
    StartMenu,
    Temporary
  }

  public static class VirtualFolderExtensions
  {
    /// <summary>
    /// Gets the path to the virtual folder identified by the <see cref="VirtualFolder"/> specified.
    /// The returned path is relative to the file system's root directory.
    /// </summary>
    /// <remarks> The returned path is not guaranteed to exist.</remarks>
    /// <param name="virtualFolder">An enumerated constant that identifies a system virtual folder.</param>
    /// <returns>The path to the specified <see cref="VirtualFolder"/>.</returns>
    public static string ToPath(this VirtualFolder virtualFolder)
    {
      switch (virtualFolder)
      {
        case VirtualFolder.ProgramFiles:
          return @"ProgramFiles\";
        case VirtualFolder.UserData:
          return @"UserData\";
        case VirtualFolder.UserDocuments:
          return @"UserData\Documents\";
        case VirtualFolder.UserMusic:
          return @"UserData\Music\";
        case VirtualFolder.UserPictures:
          return @"UserData\Pictures\";
        case VirtualFolder.Temporary:
          return @"Temporary\";
        case VirtualFolder.Other:
          return @"Other\";
        case VirtualFolder.System:
          return @"System\";
        case VirtualFolder.System32:
          return @"System\System32\";
        case VirtualFolder.StartMenu:
          return @"StartMenu\";
        case VirtualFolder.ApplicationData:
          return @"ApplicationData\";
      }
      GuestCore.Log.Critical("Unknown virtual folder: " + virtualFolder);
      return null;
    }
  }

}
