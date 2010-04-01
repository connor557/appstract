#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;

namespace ManagedFusion
{

  /// <summary>
  /// Defines the type of application manipulating the global assembly cache.
  /// </summary>
  public enum InstallerType
  {
    /// <summary>
    /// The assembly is referenced by an application that appears in Add/Remove Programs.
    /// </summary>
    /// <remarks>
    /// The <see cref="InstallerDescription._uniqueId"/> field is the token that is used to register the application with Add/Remove programs.
    /// </remarks>
    Installer,
    /// <summary>
    /// The assembly is referenced by an application that is represented by a file in the file system.
    /// </summary>
    /// <remarks>
    /// The <see cref="InstallerDescription._uniqueId"/> field is the path to the referencing file file.
    /// </remarks>
    File,
    /// <summary>
    /// The assembly is referenced by an application that is only represented by an opaque string.
    /// The GAC does not perform existence checking for opaque references when you remove this.
    /// </summary>
    /// <remarks>
    /// The <see cref="InstallerDescription._uniqueId"/> field is this opaque string.
    /// </remarks>
    OpaqueString,
    /// <summary>
    /// The assembly is referenced by an application that has been installed by using Windows Installer.
    /// </summary>
    /// <remarks>
    /// The <see cref="InstallerDescription._uniqueId"/> field is set to "MSI",
    /// and <see cref="InstallerDescription._applicationDescription"/> is set to "Windows Installer".
    /// </remarks>
    [Obsolete("Reserved. This scheme must only be used by Windows Installer itself.")]
    WindowsInstaller,
    /// <summary>
    /// The assembly is referenced by the Windows operating system.
    /// </summary>
    /// <remarks>
    /// It is not documented what values are expected in <see cref="InstallerDescription"/>.
    /// </remarks>
    [Obsolete("Reserved. This scheme must only be used by the Windows operating system itself.")]
    OsInstallGuid
  }

}
