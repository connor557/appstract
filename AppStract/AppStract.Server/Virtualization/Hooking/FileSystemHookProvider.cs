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

using System.Collections.Generic;
using AppStract.Core.Virtualization.Engine.FileSystem;

namespace AppStract.Engine.Virtualization.Hooking
{
  /// <summary>
  /// Provides all API hooks regarding the file system.
  /// </summary>
  public partial class FileSystemHookProvider : HookProvider
  {

    #region Variables

    /// <summary>
    /// The object to use as callback object for the API hooks.
    /// </summary>
    private readonly object _callback;
    /// <summary>
    /// The <see cref="HookHandler"/> containing all methods needed by the API hooks.
    /// </summary>
    private readonly Handler _hookHandler;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="FileSystemHookProvider"/>.
    /// </summary>
    /// <param name="fileSystemProvider">The <see cref="IFileSystemProvider"/> to use for the processing of intercepted API calls.</param>
    public FileSystemHookProvider(IFileSystemProvider fileSystemProvider)
    {
      _callback = fileSystemProvider;
      _hookHandler = new Handler(fileSystemProvider);
    }

    #endregion

    #region Protected Methods

    protected override IEnumerable<HookData> GetHooks()
    {
      var hooks = new List<HookData>(10);
      hooks.Add(new HookData("Create Directory [Unicode]",
                             "kernel32.dll", "CreateDirectoryW",
                             new Delegates.CreateDirectoryW(_hookHandler.CreateDirectory),
                             _callback));
      hooks.Add(new HookData("Create Directory [Ansi]",
                             "kernel32.dll", "CreateDirectoryA",
                             new Delegates.CreateDirectoryA(_hookHandler.CreateDirectory),
                             _callback));
      hooks.Add(new HookData("Create File [Unicode]",
                             "kernel32.dll", "CreateFileW",
                             new Delegates.CreateFileW(_hookHandler.CreateFile),
                             _callback));
      hooks.Add(new HookData("Create File [Ansi]",
                             "kernel32.dll", "CreateFileA",
                             new Delegates.CreateFileA(_hookHandler.CreateFile),
                             _callback));
      hooks.Add(new HookData("Delete File [Unicode]",
                             "kernel32.dll", "DeleteFileW",
                             new Delegates.DeleteFileW(_hookHandler.DeleteFile),
                             _callback));
      hooks.Add(new HookData("Delete File [Ansi]",
                             "kernel32.dll", "DeleteFileA",
                             new Delegates.DeleteFileA(_hookHandler.DeleteFile),
                             _callback));
      hooks.Add(new HookData("Remove Directory [Unicode]",
                             "kernel32.dll", "RemoveDirectoryW",
                             new Delegates.RemoveDirectoryW(_hookHandler.RemoveDirectory),
                             _callback));
      hooks.Add(new HookData("Remove Directory [Ansi]",
                             "kernel32.dll", "RemoveDirectoryA",
                             new Delegates.RemoveDirectoryA(_hookHandler.RemoveDirectory),
                             _callback));
      hooks.Add(new HookData("Load Library [Unicode]",
                             "kernel32.dll", "LoadLibraryExW",
                             new Delegates.LoadLibraryExW(_hookHandler.LoadLibraryEx),
                             _callback));
      hooks.Add(new HookData("Load Library [Ansi]",
                             "kernel32.dll", "LoadLibraryExA",
                             new Delegates.LoadLibraryExA(_hookHandler.LoadLibraryEx),
                             _callback));
      hooks.Add(new HookData("Get Temp Path [Unicode]",
                             "kernel32.dll", "GetTempPathW",
                             new Delegates.GetTempPathW(_hookHandler.GetTempPath),
                             _callback));
      hooks.Add(new HookData("Get Temp Path [Ansi]",
                             "kernel32.dll", "GetTempPathA",
                             new Delegates.GetTempPathA(_hookHandler.GetTempPath),
                             _callback));
      return hooks;
    }

    #endregion

  }
}
