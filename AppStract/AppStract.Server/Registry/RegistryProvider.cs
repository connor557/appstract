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
using AppStract.Core.Virtualization.Interop;
using AppStract.Server.Registry.Data;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Observables;
using ValueType=AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// Implementation of a virtualized Windows registry. <see cref="RegistryProvider"/> functions as a switch between
  /// the caller and <see cref="VirtualRegistry"/> &amp; <see cref="TransparentRegistry"/>.
  /// </summary>
  public sealed class RegistryProvider : IRegistryProvider
  {

    #region Variables

    /// <summary>
    /// The virtual registry.
    /// </summary>
    private readonly VirtualRegistry _virtualRegistry;
    /// <summary>
    /// Contains the open keys leading to hives that are not portable.
    /// </summary>
    private readonly TransparentRegistry _transparentRegistry;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the virtual Windows registry.
    /// </summary>
    /// <param name="dataSource">The <see cref="IRegistrySynchronizer"/> to use as data source for the already known registry keys.</param>
    public RegistryProvider(IRegistrySynchronizer dataSource)
    {
      var indexGenerator = new IndexGenerator();
      // Reserve the first 20 indices for static virtual keys.
      indexGenerator.ExcludedRanges.Add(new IndexRange(0, 20));
      // Reserved indices for registry rootkeys.
      indexGenerator.ExcludedRanges.Add(new IndexRange(0x80000000, 0x80000006));
      var knownKeys = new ObservableDictionary<uint, VirtualRegistryKey>();
      dataSource.SynchronizeRegistryWith(knownKeys);
      _virtualRegistry = new VirtualRegistry(indexGenerator, knownKeys);
      _transparentRegistry = new TransparentRegistry(indexGenerator);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Returns the <see cref="RegistryBase"/> able to process requests for the given <paramref name="hKey"/>.
    /// </summary>
    /// <param name="hKey">The key handle to get the target registry for.</param>
    /// <returns></returns>
    private RegistryBase GetTargetRegistry(uint hKey)
    {
      string keyName;
      return GetTargetRegistry(hKey, out keyName);
    }

    /// <summary>
    /// Returns the <see cref="RegistryBase"/> able to process requests for the given <paramref name="hKey"/>.
    /// </summary>
    /// <param name="hKey">The key handle to get the target registry for.</param>
    /// <param name="keyName">
    /// The string representation for <paramref name="hKey"/>, as used by the returned <see cref="RegistryBase"/>.
    /// </param>
    /// <returns></returns>
    private RegistryBase GetTargetRegistry(uint hKey, out string keyName)
    {
      if (_virtualRegistry.IsKnownKey(hKey, out keyName))
        return _virtualRegistry;
      if (_transparentRegistry.IsKnownKey(hKey, out keyName))
        return _transparentRegistry;
      if (HiveHelper.IsHiveHandle(hKey, out keyName))
      {
        return HiveHelper.GetHive(keyName).GetAccessMechanism() == AccessMechanism.Transparent
                 ? (RegistryBase) _transparentRegistry
                 : _virtualRegistry;
      }
      // hKey is an unknown handle, try to virtualize this handle.
      return TryRecoverUnknownHandle(hKey, out keyName);
    }

    /// <summary>
    /// Tries to recover from an unknown handle.
    /// </summary>
    /// <param name="hKey">The unknown key handle to try to virtualize.</param>
    /// <param name="keyName">
    /// The string representation for <paramref name="hKey"/> as retrieved during recovery
    /// and as used by the returned <see cref="RegistryBase"/>.
    /// </param>
    /// <returns></returns>
    private RegistryBase TryRecoverUnknownHandle(uint hKey, out string keyName)
    {
      keyName = HostRegistry.GetKeyNameByHandle(hKey);
      if (keyName == null)
      {
        GuestCore.Log.Error("Unknown registry key handle => {0}", hKey);
        return null;
      }
      GuestCore.Log.Warning("Recovering from unknown registry key handle => {0} => {1}", hKey, keyName);
      // Find the targeted virtual registry.
      var hive = HiveHelper.GetHive(keyName);
      var target = hive.GetAccessMechanism() == AccessMechanism.Transparent
                     ? (RegistryBase) _transparentRegistry
                     : _virtualRegistry;
      // Teach target about this unknown key handle.
      uint handle;
      if (target.OpenKey(keyName, out handle) != NativeResultCode.Success)
      {
        GuestCore.Log.Error("Unable to recover from unknown registry key handle => {0}", hKey, keyName);
        return null;
      }
      try
      {
        target.AddAlias(handle, hKey);
      }
      catch (ApplicationException e)
      {
        GuestCore.Log.Error("Unable to recover from unknown registry key handle => {0}", e, hKey, keyName);
        return null;
      }
      HostRegistry.CloseKey(hKey);
      return target;
    }

    #endregion

    #region IRegistryProvider Members

    public NativeResultCode OpenKey(uint hKey, string subKeyName, out uint hSubKey)
    {
      string keyName;
      var registry = GetTargetRegistry(hKey, out keyName);
      if (registry == null)
      {
        hSubKey = 0;
        return NativeResultCode.InvalidHandle;
      }
      keyName = HostRegistry.CombineKeyNames(keyName, subKeyName);
      return registry.OpenKey(keyName, out hSubKey);
    }

    public NativeResultCode CreateKey(uint hKey, string subKeyName, out uint hSubKey, out RegCreationDisposition creationDisposition)
    {
      string keyName;
      var registry = GetTargetRegistry(hKey, out keyName);
      if (registry == null)
      {
        hSubKey = 0;
        creationDisposition = RegCreationDisposition.NoKeyCreated;
        return NativeResultCode.InvalidHandle;
      }
      keyName = HostRegistry.CombineKeyNames(keyName, subKeyName);
      return registry.CreateKey(keyName, out hSubKey, out creationDisposition);
    }

    public NativeResultCode CloseKey(uint hKey)
    {
      var resultCode = _transparentRegistry.CloseKey(hKey);
      if (resultCode != NativeResultCode.InvalidHandle)
        return resultCode;
      resultCode = _virtualRegistry.CloseKey(hKey);
      if (resultCode != NativeResultCode.InvalidHandle)
        return resultCode;
      // None of both registries knows the handle, pass the call to the host registry.
      GuestCore.Log.Warning("Unknown registry key handle => {0}", hKey);
      return NativeAPI.RegCloseKey(hKey);
    }

    public NativeResultCode DeleteKey(uint hKey)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = GetTargetRegistry(hKey);
      return registry != null
               ? registry.DeleteKey(hKey)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = GetTargetRegistry(hKey);
      return registry != null
               ? registry.QueryValue(hKey, valueName, out value)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = GetTargetRegistry(hKey);
      return registry != null
               ? registry.SetValue(hKey, value)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = GetTargetRegistry(hKey);
      return registry != null
               ? registry.DeleteValue(hKey, valueName)
               : NativeResultCode.InvalidHandle;
    }

    #endregion

  }
}
