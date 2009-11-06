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
  /// Provides extra information on the <see cref="AssemblyInfo"/>.
  /// </summary>
  internal enum AssemblyInfoFlags
  {
    /// <summary>
    /// No flags set.
    /// </summary>
    None = 0x0,
    /// <summary>
    /// Indicates that the assembly is actually installed.
    /// Always set in current version of the .NET Framework.
    /// </summary>
    Installed = 0x01,
    /// <summary>
    /// Never set in the current version of the .NET Framework.
    /// </summary>
    PayloadResident = 0x02
  }
}
