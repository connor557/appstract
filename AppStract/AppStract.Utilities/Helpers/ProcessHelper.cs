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
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AppStract.Utilities.Helpers
{

  /// <summary>
  /// Helper class for process related actions.
  /// </summary>
  public static class ProcessHelper
  {

    #region DLL Imports

    /// <summary>Shows a Window</summary>
    /// <remarks>
    /// <para>To perform certain special effects when showing or hiding a 
    /// window, use AnimateWindow.</para>
    ///<para>The first time an application calls ShowWindow, it should use 
    ///the WinMain function's nCmdShow parameter as its nCmdShow parameter. 
    ///Subsequent calls to ShowWindow must use one of the values in the 
    ///given list, instead of the one specified by the WinMain function's 
    ///nCmdShow parameter.</para>
    ///<para>As noted in the discussion of the nCmdShow parameter, the 
    ///nCmdShow value is ignored in the first call to ShowWindow if the 
    ///program that launched the application specifies startup information 
    ///in the structure. In this case, ShowWindow uses the information 
    ///specified in the STARTUPINFO structure to show the window. On 
    ///subsequent calls, the application must call ShowWindow with nCmdShow 
    ///set to SW_SHOWDEFAULT to use the startup information provided by the 
    ///program that launched the application. This behavior is designed for 
    ///the following situations: </para>
    ///<list type="">
    ///    <item>Applications create their main window by calling CreateWindow 
    ///    with the WS_VISIBLE flag set. </item>
    ///    <item>Applications create their main window by calling CreateWindow 
    ///    with the WS_VISIBLE flag cleared, and later call ShowWindow with the 
    ///    SW_SHOW flag set to make it visible.</item>
    ///</list></remarks>
    /// <param name="hWnd">Handle to the window.</param>
    /// <param name="nCmdShow">Specifies how the window is to be shown. 
    /// This parameter is ignored the first time an application calls 
    /// ShowWindow, if the program that launched the application provides a 
    /// STARTUPINFO structure. Otherwise, the first time ShowWindow is called, 
    /// the value should be the value obtained by the WinMain function in its 
    /// nCmdShow parameter. In subsequent calls, this parameter can be one of 
    /// the WindowShowStyle members.</param>
    /// <returns>
    /// If the window was previously visible, the return value is nonzero. 
    /// If the window was previously hidden, the return value is zero.
    /// </returns>
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the <see cref="WindowShowStyle"/> for the current process' main window.
    /// </summary>
    /// <param name="windowShowStyle">Specifies how the window is to be shown.</param>
    public static void SetWindowState(WindowShowStyle windowShowStyle)
    {
      SetWindowState(Process.GetCurrentProcess().MainWindowHandle, windowShowStyle);
    }

    /// <summary>
    /// Sets the <see cref="WindowShowStyle"/> for the window with the specified <paramref name="windowHandle"/>.
    /// </summary>
    /// <remarks>
    /// Do Not give this function the handle of the desktop!
    /// </remarks>
    /// <exception cref="ArgumentException">The <paramref name="windowHandle"/> parameter is an invalid pointer.</exception>
    /// <param name="windowHandle">Handle to the window.</param>
    /// <param name="windowShowStyle">Specifies how the window is to be shown.</param>
    public static void SetWindowState(IntPtr windowHandle, WindowShowStyle windowShowStyle)
    {
      if (windowHandle == IntPtr.Zero)
        throw new ArgumentException("The windowHandle parameter is an invalid pointer.", "windowHandle");
      ShowWindow(windowHandle, windowShowStyle);
    }

    #endregion

  }

}
