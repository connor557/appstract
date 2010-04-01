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

namespace ManagedFusion
{
  /// <summary>
  /// Defines the rules determing when an assembly must be installed in the GAC.
  /// </summary>
  public enum InstallBehaviour
  {
    /// <summary>
    /// Install the assembly only if there it has not been installed already in the GAC.
    /// </summary>
    Default = 0,
    /// <summary>
    /// If the assembly is already installed in the GAC and the file version numbers of the assembly being 
    /// installed are the same or later, the files are replaced.
    /// </summary>
    Refresh = 1,
    /// <summary>
    /// The files of an existing assembly are overwritten regardless of their version number.
    /// </summary>
    ForceRefresh = 2
  }
}
