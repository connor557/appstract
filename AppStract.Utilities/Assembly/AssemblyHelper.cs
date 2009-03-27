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

using System;
using System.IO;
using System.Reflection;
using System.Security;

namespace AppStract.Utilities.Assembly
{
  /// <summary>
  /// A helper class for simple actions related to assemblies.
  /// </summary>
  public static class AssemblyHelper
  {

    #region Public Methods

    /// <summary>
    /// Gets the first executable that was executed in the current <see cref="AppDomain"/>.
    /// </summary>
    /// <returns></returns>
    public static System.Reflection.Assembly GetEntryAssembly()
    {
      return System.Reflection.Assembly.GetEntryAssembly();
    }

    /// <summary>
    /// Returns the type of the assembly specified.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <param name="assemblyFile">The filename of the assembly.</param>
    /// <returns></returns>
    public static AssemblyType GetAssemblyType(string assemblyFile)
    {
      try
      { 
        AssemblyName.GetAssemblyName(assemblyFile);
        return AssemblyType.Managed;
      }
      catch (BadImageFormatException)
      {
        /// The module doesn't contain an assembly-manifest.
        /// No way that this is managed code.
        return AssemblyType.Native;
      }
    }

    #endregion

  }
}
