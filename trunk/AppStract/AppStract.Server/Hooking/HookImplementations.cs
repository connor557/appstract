#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
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
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Core.Virtualization.Engine.Registry;

namespace AppStract.Server.Hooking
{
  /// <summary>
  /// The methods defined by this class process all intercepted calls to hooked methods.
  /// </summary>
  public partial class HookImplementations
  {

    #region Variables

    private readonly IFileSystemProvider _fileSystem;
    private readonly IRegistryProvider _registry;

    #endregion

    #region Constructors

    public HookImplementations(IFileSystemProvider fileSystemProvider, IRegistryProvider registryProvider)
    {
      _fileSystem = fileSystemProvider;
      _registry = registryProvider;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Suppresses a possible <see cref="NullReferenceException"/> when assigning <paramref name="value"/> to <paramref name="destination"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="destination"></param>
    private static void SafeWrite<T>(T value, ref T destination) where T : struct
    {
      try
      {
        destination = value;
      }
      catch (NullReferenceException)
      {
        // Might happen when from native side null is provided for destination.
      }
    }

    /// <summary>
    /// Tries to parse the <see cref="Int64"/> value of <paramref name="pointer"/> to an <see cref="UInt32"/>
    /// </summary>
    /// <param name="pointer"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private static bool TryParse(UIntPtr pointer, out uint result)
    {
      result = (uint)pointer;
      return true;
    }

    #endregion

  }
}
