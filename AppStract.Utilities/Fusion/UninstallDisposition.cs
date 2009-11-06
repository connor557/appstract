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

namespace System.Reflection.GAC
{

  /// <summary>
  /// Represents the result of an attempt to uninstall an assembly from the global assembly cache.
  /// </summary>
  public enum UninstallDisposition
  {
    None = 0x00,
    /// <summary>
    /// The assembly files have been removed from the GAC.
    /// </summary>
    Uninstalled = 0x01,
    /// <summary>
    /// [Windows 9x] An application is using the assembly.
    /// </summary>
    StillInUse = 0x02,
    /// <summary>
    /// The assembly does not exist in the GAC.
    /// </summary>
    AlreadyUninstalled = 0x03,
    [Obsolete("This value is not used by the Fusion API")]
    DeletePending = 0x04,
    /// <summary>
    /// The assembly has not been removed from the GAC because another application reference exists.
    /// </summary>
    HasReferences = 0x05,
    /// <summary>
    /// The reference is not found in the GAC.
    /// </summary>
    ReferenceNotFound = 0x06
  }
}
