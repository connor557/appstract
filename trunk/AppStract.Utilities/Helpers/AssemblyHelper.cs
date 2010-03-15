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
using SystemAssembly = System.Reflection.Assembly;

namespace AppStract.Utilities.Helpers
{
  /// <summary>
  /// A helper class for simple actions related to assemblies.
  /// </summary>
  public static class AssemblyHelper
  {

    #region Public Methods

    /// <summary>
    /// Returns whether the specified file is a managed assembly.
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="FileLoadException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <param name="assemblyFile">The filename of the assembly.</param>
    /// <returns></returns>
    public static bool IsManagedAssembly(string assemblyFile)
    {
      try
      { 
        AssemblyName.GetAssemblyName(assemblyFile);
        return true;
      }
      catch (BadImageFormatException)
      {
        /// The module doesn't contain an assembly-manifest.
        /// No way that this is managed code.
        return false;
      }
    }

    /// <summary>
    /// Runs the main method from the <paramref name="executable"/> specified.
    /// </summary>
    /// <param name="executable">The executable to run the main method from.</param>
    /// <param name="args">The arguments to pass to the main method.</param>
    /// <returns>Exit code returned by the main method.</returns>
    public static int RunMainMethod(string executable, string[] args)
    {
      var assembly = SystemAssembly.LoadFrom(executable);
      var parameters = assembly.EntryPoint.GetParameters();
      if (parameters.Length > 1)
        // Main methods are expected to have zero or one parameter
        return 87; // ERROR_INVALID_PARAMETER;
      var invokeParams = parameters.Length == 0
                           ? null
                           : (args == null || args.Length == 0 ? new object[0] : new object[] {args});
      var o = assembly.EntryPoint.Invoke(null, invokeParams);
      int exitCode;
      return o != null && int.TryParse(o.ToString(), out exitCode)
               ? exitCode
               : -1;
    }

    #endregion

  }
}
