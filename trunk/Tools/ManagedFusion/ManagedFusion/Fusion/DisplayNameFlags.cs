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

namespace ManagedFusion.Fusion
{
  /// <summary>
  /// Indicates the version, build, culture, signature, and so on, of the assembly whose display name will be retrieved.
  /// </summary>
  [Flags]
  public enum DisplayNameFlags
  {
    All = 0x0,
    /// <summary>
    /// Includes the version number as part of the display name.
    /// </summary>
    Version = 0x1,
    /// <summary>
    /// Includes the culture.
    /// </summary>
    Culture = 0x2,
    /// <summary>
    /// Includes the internal key token.
    /// </summary>
    PublicKeyToken = 0x4,
    /// <summary>
    /// Includes the internal key.
    /// </summary>
    PublicKey = 0x8,
    /// <summary>
    /// Includes the custom part of the assembly name.
    /// </summary>
    Custom = 0x10,
    /// <summary>
    /// Includes the processor architecture.
    /// </summary>
    ProcessArchitecture = 0x20,
    /// <summary>
    /// Includes the language ID.
    /// </summary>
    LanguageId = 0x40
  }
}
