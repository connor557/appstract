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

namespace AppStract.Utilities.Interop
{
  /// <summary>
  /// Enumeration of the different ways of showing a window using <see cref="ProcessHelpers.SetWindowState"/>.
  /// </summary>
  public enum WindowShowStyle : uint
  {
    /// <summary>Hides the window and activates another window.</summary>
    /// <remarks>See SW_HIDE</remarks>
    Hide = 0,
    /// <summary>Activates and displays a window. If the window is minimized 
    /// or maximized, the system restores it to its original size and 
    /// position. An application should specify this flag when displaying 
    /// the window for the first time.</summary>
    /// <remarks>See SW_SHOWNORMAL</remarks>
    ShowNormal = 1,
    /// <summary>Activates the window and displays it as a minimized window.</summary>
    /// <remarks>See SW_SHOWMINIMIZED</remarks>
    ShowMinimized = 2,
    /// <summary>Activates the window and displays it as a maximized window.</summary>
    /// <remarks>See SW_SHOWMAXIMIZED</remarks>
    ShowMaximized = 3,
    /// <summary>Maximizes the specified window.</summary>
    /// <remarks>See SW_MAXIMIZE</remarks>
    Maximize = 3,
    /// <summary>Displays a window in its most recent size and position. 
    /// This value is similar to "ShowNormal", except the window is not 
    /// actived.</summary>
    /// <remarks>See SW_SHOWNOACTIVATE</remarks>
    ShowNormalNoActivate = 4,
    /// <summary>Activates the window and displays it in its current size 
    /// and position.</summary>
    /// <remarks>See SW_SHOW</remarks>
    Show = 5,
    /// <summary>Minimizes the specified window and activates the next 
    /// top-level window in the Z order.</summary>
    /// <remarks>See SW_MINIMIZE</remarks>
    Minimize = 6,
    /// <summary>Displays the window as a minimized window. This value is 
    /// similar to "ShowMinimized", except the window is not activated.</summary>
    /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
    ShowMinNoActivate = 7,
    /// <summary>Displays the window in its current size and position. This 
    /// value is similar to "Show", except the window is not activated.</summary>
    /// <remarks>See SW_SHOWNA</remarks>
    ShowNoActivate = 8,
    /// <summary>Activates and displays the window. If the window is 
    /// minimized or maximized, the system restores it to its original size 
    /// and position. An application should specify this flag when restoring 
    /// a minimized window.</summary>
    /// <remarks>See SW_RESTORE</remarks>
    Restore = 9,
    /// <summary>Sets the show state based on the SW_ value specified in the 
    /// STARTUPINFO structure passed to the CreateProcess function by the 
    /// program that started the application.</summary>
    /// <remarks>See SW_SHOWDEFAULT</remarks>
    ShowDefault = 10,
    /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
    /// that owns the window is hung. This flag should only be used when 
    /// minimizing windows from a different thread.</summary>
    /// <remarks>See SW_FORCEMINIMIZE</remarks>
    ForceMinimized = 11
  }
}
