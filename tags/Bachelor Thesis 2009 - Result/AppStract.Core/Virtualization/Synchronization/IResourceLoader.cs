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

using System.Collections.Generic;
using AppStract.Core.Virtualization.FileSystem;
using AppStract.Core.Virtualization.Registry;

namespace AppStract.Core.Virtualization.Synchronization
{
  /// <summary>
  /// Provides resources needed by the guest process in order to be initializable.
  /// </summary>
  public interface IResourceLoader
  {

    /// <summary>
    /// The root directory as used by the file system.
    /// </summary>
    /// <remarks>
    /// The file table only contains relative paths, while the file system should only provide absolute paths.
    /// These relative paths must be combined with the root directory, which results in usable absolute paths.
    /// </remarks>
    string FileSystemRoot
    {
      get;
    }

    /// <summary>
    /// Returns all known <see cref="FileTableEntry"/> as an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns></returns>
    IEnumerable<FileTableEntry> LoadFileSystemTable();

    /// <summary>
    /// Returns all known <see cref="VirtualRegistryKey"/> as an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns></returns>
    IEnumerable<VirtualRegistryKey> LoadRegistry();

  }
}
