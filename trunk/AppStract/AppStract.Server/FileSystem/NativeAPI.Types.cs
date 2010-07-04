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

namespace AppStract.Server.FileSystem
{

  /// <summary>
  /// The action to be taken when loading the module.
  /// </summary>
  [Flags]
  public enum ModuleLoadFlags : uint
  {
    /// <summary>
    /// Default value, nof flags specified.
    /// </summary>
    Default = 0x00000000,
    /// <summary>
    /// DONT_RESOLVE_DLL_REFERENCES - 
    /// If this value is used, and the executable module is a DLL, the system does not call DllMain for process and thread initialization and termination.
    /// Also, the system does not load additional executable modules that are referenced by the specified module.
    /// </summary>
    [Obsolete("Do not use this value; it is provided only for backwards compatibility.")]
    DontResolve = 0x00000001,
    /// <summary>
    /// LOAD_IGNORE_CODE_AUTHZ_LEVEL - 
    /// If this value is used, the system does not check AppLocker rules or apply Software Restriction Policies for the DLL.
    /// This action applies only to the DLL being loaded and not to its dependents.
    /// This value is recommended for use in setup programs that must run extracted DLLs during installation.
    /// </summary>
    IgnoreAuthorizationLevel = 0x00000010,
    /// <summary>
    /// LOAD_LIBRARY_AS_DATAFILE - 
    /// If this value is used, the system maps the file into the calling process's virtual address space as if it were a data file.
    /// Nothing is done to execute or prepare to execute the mapped file.
    /// </summary>
    AsDatafile = 0x00000002,
    /// <summary>
    /// LOAD_LIBRARY_AS_DATAFILE_EXCLUSIVE - 
    /// Similar to <see cref="AsDatafile"/>, except that the DLL file on the disk is opened for exclusive write access.
    /// Therefore, other processes cannot open the DLL file for write access while it is in use.
    /// However, the DLL can still be opened by other processes.
    /// </summary>
    AsExclusiveDatafile = 0x00000040,
    /// <summary>
    /// LOAD_LIBRARY_ASIMAGE_RESOURCE - 
    /// If this value is used, the system maps the file into the process's virtual address space as an image file.
    /// However, the loader does not load the static imports or perform the other usual initialization steps.
    /// Use this flag when you want to load a DLL only to extract messages or resources from it.
    /// </summary>
    AsImageResource = 0x00000020,
    /// <summary>
    /// LOAD_WITH_ALTERED_SEARCH_PATH - 
    /// If this value is used, the system uses an alternate file search strategy.
    /// </summary>
    AlternateSearchStrategy = 0x00000008
  }

  /// <summary>
  /// Indicates access rights on files.
  /// </summary>
  [Flags]
  public enum FileAccessRightFlags : uint
  {
    GENERIC_ALL = 0x10000000,
    GENERIC_READ = STANDARD_RIGHTS_READ | FILE_READ_DATA | FILE_READ_ATTRIBUTES | FILE_READ_EA | SYNCHRONIZE,
    GENERIC_WRITE = STANDARD_RIGHTS_WRITE | FILE_WRITE_DATA | FILE_WRITE_ATTRIBUTES | FILE_WRITE_EA | FILE_APPEND_DATA | SYNCHRONIZE,
    GENERIC_EXECUTE = STANDARD_RIGHTS_EXECUTE | FILE_READ_ATTRIBUTES | FILE_EXECUTE | SYNCHRONIZE,
    /// <summary>
    /// For a directory, the right to create a file in the directory.
    /// </summary>
    FILE_ADD_FILE = 0x0002,
    /// <summary>
    /// For a directory, the right to create a subdirectory.
    /// </summary>
    FILE_ADD_SUBDIRECTORY = 0x0004,
    /// <summary>
    /// For a file object, the right to append data to the file.
    /// </summary>
    FILE_APPEND_DATA = 0x0004,
    /// <summary>
    /// For a named pipe, the right to create a pipe.
    /// </summary>
    FILE_CREATE_PIPE_INSTANCE = 0x0004,
    /// <summary>
    /// For a directory, the right to delete a directory and all the files it contains, including the read-only files.
    /// </summary>
    FILE_DELETE_CHILD = 0x0040,
    /// <summary>
    /// For a native code file, the right to execute the file. This access right given to scripts may cause the script to be executable, depending on the script interpreter.
    /// </summary>
    FILE_EXECUTE = 0x0020,
    /// <summary>
    /// For a directory, the right to list the contents of the directory.
    /// </summary>
    FILE_LIST_DIRECTORY = 0x0001,
    /// <summary>
    /// The right to read file attributes.
    /// </summary>
    FILE_READ_ATTRIBUTES = 0x0080,
    /// <summary>
    /// For a file object, the right to read the corresponding file data.
    /// For a directory object, the right to read the corresponding directory data.
    /// </summary>
    FILE_READ_DATA = 0x0001,
    /// <summary>
    /// The right to read extended file attributes.
    /// </summary>
    FILE_READ_EA = 0x0008,
    /// <summary>
    /// For a directory, the right to traverse the directory.
    /// </summary>
    FILE_TRAVERSE = 0x0020,
    /// <summary>
    /// The right to write file attributes.
    /// </summary>
    FILE_WRITE_ATTRIBUTES = 0x0100,
    /// <summary>
    /// For a file object, the right to write data to the file.
    /// For a directory object, the right to create a file in the directory (<see cref="FILE_ADD_FILE"/>).
    /// </summary>
    FILE_WRITE_DATA = 0x0002,
    /// <summary>
    /// The right to write extended file attributes.
    /// </summary>
    FILE_WRITE_EA = 0x0010,
    /// <summary>
    /// All possible access rights for a file.
    /// </summary>
    FILE_ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0x1FF,
    STANDARD_RIGHTS_REQUIRED = 0x000F0000,
    READ_CONTROL = 0x00020000,
    SYNCHRONIZE = 0x00100000,
    STANDARD_RIGHTS_READ = READ_CONTROL,
    STANDARD_RIGHTS_WRITE = READ_CONTROL,
    STANDARD_RIGHTS_EXECUTE = READ_CONTROL,
    STANDARD_RIGHTS_ALL = 0x001F0000,
    SPECIFIC_RIGHTS_ALL = 0x0000FFFF,
  }

  /// <summary>
  /// The requested sharing mode of the file or device, which can be read, write, both, delete, all of these, or none.
  /// Access requests to attributes or extended attributes are not affected by this flag.
  /// </summary>
  [Flags]
  public enum FileShareModeFlags : uint
  {
    /// <summary>
    /// Prevents other processes from opening a file or device if they request delete, read, or write access.
    /// </summary>
    None = 0x00000000,
    /// <summary>
    /// Enables subsequent open operations on a file or device to request read access.
    /// </summary>
    Read = 0x00000001,
    /// <summary>
    /// Enables subsequent open operations on a file or device to request write access.
    /// </summary>
    Write = 0x00000002,
    /// <summary>
    /// Enables subsequent open operations on a file or device to request delete access.
    /// </summary>
    Delete = 0x00000004
  }

  /// <summary>
  /// The file or device attributes and flags.
  /// </summary>
  [Flags]
  public enum FileFlagsAndAttributes : uint
  {
    FILE_ATTRIBUTE_READONLY = 0x00000001,
    FILE_ATTRIBUTE_HIDDEN = 0x00000002,
    FILE_ATTRIBUTE_SYSTEM = 0x00000004,
    FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
    FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
    FILE_ATTRIBUTE_DEVICE = 0x00000040,
    FILE_ATTRIBUTE_NORMAL = 0x00000080,
    FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
    FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
    FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
    FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
    FILE_ATTRIBUTE_OFFLINE = 0x00001000,
    FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
    FILE_ATTRIBUTE_ENCRYPTED = 0x00004000,

    FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
    FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
    FILE_FLAG_NO_BUFFERING = 0x20000000,
    FILE_FLAG_OPEN_NO_RECALL = 0x00100000,
    FILE_FLAG_OPEN_REPARSE_POINT = 0x00100000,
    FILE_FLAG_OVERLAPPED = 0x40000000,
    FILE_FLAG_POSIX_SEMANTICS = 0x01000000,
    FILE_FLAG_RANDOM_ACCESS = 0x10000000,
    FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
    FILE_FLAG_WRITE_THROUGH = 0x80000000,

    SECURITY_ANONYMOUS = 0x00000000,
    SECURITY_CONTEXT_TRACKING = 0x00040000,
    SECURITY_DELEGATION = 0x00030000,
    SECURITY_EFFECTIVE_ONLY = 0x00080000,
    SECURITY_IDENTIFICATION = 0x00010000,
    SECURITY_IMPERSONATION = 0x00020000,
    SECURITY_SQOS_PRESENT = 0x00100000,
    SECURITY_VALID_SQOS_FLAGS = 0x001F0000
  }

}
