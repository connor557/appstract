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
using System.IO;
using System.Runtime.InteropServices;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Interop;
using AppStract.Utilities.Helpers;

namespace AppStract.Server.Hooking
{
  public partial class HookImplementations
  {

    #region Constants

    /// <summary>
    /// If a file management function fails, the return value is INVALID_HANDLE_VALUE
    /// </summary>
    /// Bug? Isn't 6 the correct value?
    private const int INVALID_HANDLE_VALUE = -1;
    /// <summary>
    /// If the <see cref="CreateDirectory"/> function, the return value is zero.
    /// </summary>
    private const int CREATE_DIRECTORY_FAILED = 0;

    #endregion

    #region Public Methods - FileSystem

    /// <summary>
    /// Handles intercepted file access.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <param name="InDesiredAccess"></param>
    /// <param name="InShareMode"></param>
    /// <param name="InSecurityAttributes"></param>
    /// <param name="InCreationDisposition"></param>
    /// <param name="InFlagsAndAttributes"></param>
    /// <param name="InTemplateFile"></param>
    /// <returns></returns>
    public IntPtr DoCreateFile(String InFileName, UInt32 InDesiredAccess, UInt32 InShareMode,
      IntPtr InSecurityAttributes, UInt32 InCreationDisposition, UInt32 InFlagsAndAttributes, IntPtr InTemplateFile)
    {
      FileCreationDisposition creationDisposition;
      ParserHelper.TryParseEnum(InCreationDisposition, out creationDisposition);
      var request = new FileRequest(InFileName, ResourceKind.FileOrDirectory, creationDisposition);
      using (HookManager.ACL.GetHookingExclusion())
      {
        var entry = _fileSystem.GetFile(request);
        var ptr = CreateFile(entry.Value, InDesiredAccess, InShareMode, InSecurityAttributes, InCreationDisposition,
                             InFlagsAndAttributes, InTemplateFile);
        if (ptr.ToInt32() == INVALID_HANDLE_VALUE)
          HandleFailedCreation(entry);
        return ptr;
      }
    }

    /// <summary>
    /// Handles intercepted requests to delete a file.
    /// </summary>
    /// <param name="lpFileName"></param>
    /// <returns></returns>
    public bool DoDeleteFile(String lpFileName)
    {
      var request = new FileRequest(lpFileName, ResourceKind.FileOrDirectory, FileCreationDisposition.UNSPECIFIED);
      using (HookManager.ACL.GetHookingExclusion())
      {
        var entry = _fileSystem.GetFile(request);
        try
        {
          if ((entry.FileKind == FileKind.File || entry.FileKind == FileKind.Unspecified)
              && File.Exists(entry.Value))
            File.Delete(entry.Value);
          else if ((entry.FileKind == FileKind.File || entry.FileKind == FileKind.Unspecified)
                   && File.Exists(entry.Key))
            File.Delete(entry.Key);
          else if ((entry.FileKind == FileKind.Directory || entry.FileKind == FileKind.Unspecified)
                   && Directory.Exists(entry.Value))
            Directory.Delete(entry.Value);
          else if ((entry.FileKind == FileKind.Directory || entry.FileKind == FileKind.Unspecified)
                   && Directory.Exists(entry.Key))
            Directory.Delete(entry.Key);
          _fileSystem.DeleteFile(entry);
          return true;
        }
        catch
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Handles intercepted directory access.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <param name="InSecurityAttributes"></param>
    /// <returns></returns>
    public IntPtr DoCreateDirectory(String InFileName, IntPtr InSecurityAttributes)
    {
      var request = new FileRequest(InFileName, ResourceKind.FileOrDirectory, FileCreationDisposition.CREATE_NEW);
      using (HookManager.ACL.GetHookingExclusion())
      {
        var entry = _fileSystem.GetFile(request);
        var resultPtr = CreateDirectory(entry.Value, InSecurityAttributes);
        if (resultPtr.ToInt32() == CREATE_DIRECTORY_FAILED)
          HandleFailedCreation(entry);
        return resultPtr;
      }
    }

    /// <summary>
    /// Handles intercepted library access.
    /// </summary>
    /// <param name="dllFileName"></param>
    /// <param name="handel"></param>
    /// <param name="mozart"></param>
    /// <returns></returns>
    public IntPtr DoLoadLibraryEx(String dllFileName, IntPtr handel, uint mozart)
    {
      var request = new FileRequest(dllFileName, ResourceKind.Library, FileCreationDisposition.OPEN_EXISTING);
      using (HookManager.ACL.GetHookingExclusion())
      {
        var entry = _fileSystem.GetFile(request);
        return LoadLibraryEx(entry.Value, handel, mozart);
      }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Must be called if the creation of a file or directory failed.
    /// This method checks the last Win32 error and performs the required action.
    /// </summary>
    /// <param name="fileTableEntry"></param>
    private void HandleFailedCreation(FileTableEntry fileTableEntry)
    {
      var error = (NativeResultCode) Marshal.GetLastWin32Error();
      if (error != NativeResultCode.FileAlreadyExists)
        _fileSystem.DeleteFile(fileTableEntry);
    }

    #endregion

    #region Imports - FileSystem

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr CreateFile(
        String InFileName,
        UInt32 InDesiredAccess,
        UInt32 InShareMode,
        IntPtr InSecurityAttributes,
        UInt32 InCreationDisposition,
        UInt32 InFlagsAndAttributes,
        IntPtr InTemplateFile);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    private static extern IntPtr CreateDirectory(
        String InFileName,
        IntPtr InSecurityAttributes);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    static extern IntPtr LoadLibraryEx(String dllFileName, IntPtr handel, uint mozart);

    #endregion

  }
}