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
  /// Indicates the source of an assembly represented by <see cref="IAssemblyCacheItem"/> in the global assembly cache.
  /// </summary>
  /// <remarks>
  /// Native name: ASM_CACHE_FLAGS
  /// </remarks>
  [Flags]
  internal enum AssemblyCacheFlags
  {
    /// <summary>
    /// Enumerates the cache of precompiled assemblies by using Ngen.exe.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_CACHE_ZAP
    /// </remarks>
    ZAP = 0x1,
    /// <summary>
    /// Enumerates the GAC.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_CACHE_GAC
    /// </remarks>
    GAC = 0x2,
    /// <summary>
    /// Enumerates the assemblies that have been downloaded on-demand or that have been shadow-copied.
    /// </summary>
    /// <remarks>
    /// Native name: ASM_CACHE_DOWNLOAD
    /// </remarks>
    Download = 0x4
  }
}
