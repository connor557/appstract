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
using System.Collections.Generic;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.Registry;
using AppStract.Server.Registry.Data;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// This class functions as a switch for <see cref="VirtualRegistry"/> and <see cref="TransparentRegistry"/>.
  /// The switching itself is based on various terms and conditions.
  /// </summary>
  public class RegistrySwitch
  {

    #region Variables

    /// <summary>
    /// The virtual registry.
    /// </summary>
    private readonly RegistryBase _virtualRegistry;
    /// <summary>
    /// Contains the open keys leading to hives that are not portable.
    /// </summary>
    private readonly RegistryBase _transparentRegistry;

    #endregion

    #region Constructors

    public RegistrySwitch(IndexGenerator indexGenerator, IDictionary<uint, VirtualRegistryKey> knownKeys)
    {
      _transparentRegistry = new TransparentRegistry(indexGenerator);
      _virtualRegistry = new VirtualRegistry(indexGenerator, knownKeys);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the <see cref="RegistryBase"/> able to process requests for the given <paramref name="hKey"/>.
    /// </summary>
    /// <param name="hKey">The key handle to get the target registry for.</param>
    /// <returns></returns>
    public RegistryBase GetRegistryFor(uint hKey)
    {
      string keyFullPath;
      return GetRegistryFor(hKey, out keyFullPath);
    }

    /// <summary>
    /// Returns the <see cref="RegistryBase"/> able to process requests for the given <paramref name="hKey"/>.
    /// </summary>
    /// <param name="hKey">The key handle to get the target registry for.</param>
    /// <param name="keyFullPath">
    /// The string representation for <paramref name="hKey"/>, as used by the returned <see cref="RegistryBase"/>.
    /// </param>
    /// <returns></returns>
    public RegistryBase GetRegistryFor(uint hKey, out string keyFullPath)
    {
      return GetRegistryFor(hKey, out keyFullPath, true);
    }

    /// <summary>
    /// Returns the <see cref="RegistryBase"/> able to process requests for the given <paramref name="hKey"/>.
    /// </summary>
    /// <param name="hKey">The key handle to get the target registry for.</param>
    /// <param name="keyFullPath">
    /// The string representation for <paramref name="hKey"/>, as used by the returned <see cref="RegistryBase"/>.
    /// </param>
    /// <param name="recoverHandle">
    /// Whether or not hKey should be recovered if it can't be found in the virtual registry.
    /// </param>
    /// <returns></returns>
    public RegistryBase GetRegistryFor(uint hKey, out string keyFullPath, bool recoverHandle)
    {
      if (_virtualRegistry.IsKnownKey(hKey, out keyFullPath))
        return _virtualRegistry;
      if (_transparentRegistry.IsKnownKey(hKey, out keyFullPath))
        return _transparentRegistry;
      if (HiveHelper.IsHiveHandle(hKey, out keyFullPath))
        return GetRegistryFor(keyFullPath);
      // hKey is an unknown handle, try to virtualize this handle.
      if (recoverHandle)
        return TryRecoverUnknownHandle(hKey, out keyFullPath);
      GuestCore.Log.Error("Unknown registry key handle => {0}", hKey);
      return null;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the <see cref="RegistryBase"/> able to process requests for the given <paramref name="keyFullPath"/>.
    /// </summary>
    /// <param name="keyFullPath">The string representation of the key to get the target registry for.</param>
    /// <returns></returns>
    private RegistryBase GetRegistryFor(string keyFullPath)
    {
      var hive = HiveHelper.GetHive(keyFullPath);
      return hive.GetAccessMechanism() == AccessMechanism.Transparent
               ? _transparentRegistry
               : _virtualRegistry;
    }

    /// <summary>
    /// Tries to recover from an unknown handle.
    /// </summary>
    /// <param name="hKey">The unknown key handle to try to virtualize.</param>
    /// <param name="keyFullPath">
    /// The string representation for <paramref name="hKey"/> as retrieved during recovery
    /// and as used by the returned <see cref="RegistryBase"/>.
    /// </param>
    /// <returns></returns>
    private RegistryBase TryRecoverUnknownHandle(uint hKey, out string keyFullPath)
    {
      keyFullPath = HostRegistry.GetKeyNameByHandle(hKey);
      if (keyFullPath == null)
      {
        GuestCore.Log.Error("Unknown registry key handle => {0}", hKey);
        return null;
      }
      GuestCore.Log.Warning("Recovering from unknown registry key handle => {0} => {1}", hKey, keyFullPath);
      // Teach target about this unknown key handle.
      var target = GetRegistryFor(keyFullPath);
      uint handle;
      if (target.OpenKey(keyFullPath, out handle) != NativeResultCode.Success)
      {
        GuestCore.Log.Error("Unable to recover from unknown registry key handle => {0}", hKey, keyFullPath);
        return null;
      }
      try
      {
        target.AddAlias(handle, hKey);
      }
      catch (ApplicationException e)
      {
        GuestCore.Log.Error("Unable to recover from unknown registry key handle => {0}", e, hKey, keyFullPath);
        return null;
      }
      HostRegistry.CloseKey(hKey);
      return target;
    }

    #endregion

  }
}
