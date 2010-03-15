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

namespace AppStract.Utilities.ManagedFusion.Fusion
{
  /// <summary>
  /// Indicates the attributes to be compared.
  /// </summary>
  /// <remarks>
  /// Native name: AssemblyCompareFlags
  /// </remarks>
  [Flags]
  internal enum AssemblyCompareFlags
  {
    NAME = 0x1,
    MAJOR_VERSION = 0x2,
    MINOR_VERSION = 0x4,
    BUILD_NUMBER = 0x8,
    REVISION_NUMBER = 0x10,
    PUBLIC_KEY_TOKEN = 0x20,
    CULTURE = 0x40,
    CUSTOM = 0x80,
    ALL = NAME | MAJOR_VERSION | MINOR_VERSION |
      REVISION_NUMBER | BUILD_NUMBER |
      PUBLIC_KEY_TOKEN | CULTURE | CUSTOM,
    DEFAULT = 0x100
  }
}
