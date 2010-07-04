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
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Server.FileSystem;

namespace AppStract.Server.Hooking
{
  public partial class HookImplementations
  {

    #region Public Methods - FileSystem

    /// <summary>
    /// Handles intercepted file access.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="desiredAccess"></param>
    /// <param name="shareMode"></param>
    /// <param name="securityAttributes"></param>
    /// <param name="creationDisposition"></param>
    /// <param name="flagsAndAttributes"></param>
    /// <param name="templateFile"></param>
    /// <returns></returns>
    public IntPtr DoCreateFile(string fileName, FileAccessRightFlags desiredAccess, FileShareModeFlags shareMode,
                               NativeSecurityAttributes securityAttributes, FileCreationDisposition creationDisposition,
                               FileFlagsAndAttributes flagsAndAttributes, IntPtr templateFile)
    {
      var request = new FileRequest
      {
        CreationDisposition = creationDisposition,
        Path = fileName,
        ResourceType = ResourceType.File
      };
      using (HookManager.ACL.GetHookingExclusion())
      {
        var virtualPath = _fileSystem.GetVirtualPath(request);
        return NativeAPI.CreateFile(virtualPath, desiredAccess, shareMode, securityAttributes,
                                    creationDisposition, flagsAndAttributes, templateFile);
      }
    }

    /// <summary>
    /// Handles intercepted requests to delete a file.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public bool DoDeleteFile(string fileName)
    {
      var request = new FileRequest
      {
        CreationDisposition = FileCreationDisposition.OpenExisting,
        Path = fileName,
        ResourceType = ResourceType.File
      };
      using (HookManager.ACL.GetHookingExclusion())
      {
        var virtualPath = _fileSystem.GetVirtualPath(request);
        return NativeAPI.DeleteFile(virtualPath);
      }
    }

    /// <summary>
    /// Handles intercepted directory access.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="securityAttributes"></param>
    /// <returns></returns>
    public bool DoCreateDirectory(string fileName, NativeSecurityAttributes securityAttributes)
    {
      var request = new FileRequest
      {
        CreationDisposition = FileCreationDisposition.OpenAlways,
        Path = fileName,
        ResourceType = ResourceType.Directory
      };
      using (HookManager.ACL.GetHookingExclusion())
      {
        var virtualPath = _fileSystem.GetVirtualPath(request);
        return NativeAPI.CreateDirectory(virtualPath, securityAttributes);
      }
    }

    /// <summary>
    /// Handles intercepted library access.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="file"></param>
    /// <param name="flags"></param>
    /// <returns></returns>
    public IntPtr DoLoadLibraryEx(string fileName, IntPtr file, ModuleLoadFlags flags)
    {
      var request = new FileRequest
      {
        CreationDisposition = FileCreationDisposition.OpenExisting,
        Path = fileName,
        ResourceType = ResourceType.Library
      };
      using (HookManager.ACL.GetHookingExclusion())
      {
        var virtualPath = _fileSystem.GetVirtualPath(request);
        return NativeAPI.LoadLibraryEx(virtualPath, file, flags);
      }
    }

    #endregion

  }
}