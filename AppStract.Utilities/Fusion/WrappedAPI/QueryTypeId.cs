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

namespace System.Runtime.Interop.Fusion
{
  /// <summary>
  /// Specifies the information to retrieve when querying assembly information.
  /// </summary>
  internal enum QueryTypeId
  {
    /// <summary>
    /// No information type specified.
    /// </summary>
    None = 0x0,
    /// <summary>
    /// Validates the assembly files in the side-by-side assembly store against the assembly manifest.
    /// This includes the verification of the assembly's hash and strong name signature.
    /// </summary>
    Validate = 0x1,
    /// <summary>
    /// Returns the size of all files in the assembly.
    /// </summary>
    Size = 0x2
  }
}
