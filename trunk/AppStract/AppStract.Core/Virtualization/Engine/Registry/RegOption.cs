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

namespace AppStract.Core.Virtualization.Engine.Registry
{
  /// <summary>
  /// Registry key options.
  /// </summary>
  [Flags]
  public enum RegOption
  {
    /// <summary>
    /// This key is not volatile; this is the default.
    /// The information is stored in a file and is preserved when the system is restarted.
    /// The RegSaveKey function saves keys that are not volatile.
    /// </summary>
    NonVolatile = 0x0,
    /// <summary>
    /// All keys created by the function are volatile.
    /// The information is stored in memory and is not preserved when the corresponding registry hive is unloaded. For HKEY_LOCAL_MACHINE, this occurs when the system is shut down.
    /// For registry keys loaded by the RegLoadKey function, this occurs when the corresponding RegUnLoadKey is performed.
    /// The RegSaveKey function does not save volatile keys. This flag is ignored for keys that already exist.
    /// </summary>
    Volatile = 0x1,
    /// <summary>
    /// This key is a symbolic link. The target path is assigned to the L"SymbolicLinkValue" value of the key. The target path must be an absolute registry path. 
    /// Registry symbolic links should be used only when absolutely necessary for application compatibility.
    /// </summary>
    CreateLink = 0x2,
    /// <summary>
    /// If this flag is set, the function ignores the samDesired parameter and attempts to open the key with the access required to backup or restore the key.
    /// </summary>
    BackupRestore = 0x4,
    /// <summary>
    /// Opens the symbolic link, rather than the registry key pointed to by the symbolic link.
    /// </summary>
    OpenLink = 0x8
  }
}
