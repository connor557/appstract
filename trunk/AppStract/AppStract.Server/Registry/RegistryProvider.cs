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

using AppStract.Core.Virtualization.Interop;
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Observables;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// The default implementation of <see cref="IRegistryProvider"/>, providing a virtualized Windows registry.
  /// </summary>
  public sealed class RegistryProvider : IRegistryProvider
  {

    #region Variables

    private readonly RegistrySwitch _switch;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the virtual Windows registry.
    /// </summary>
    /// <param name="dataSource">
    /// The <see cref="IRegistrySynchronizer"/> to use as data source an synchronization context for known virtual registry keys.
    /// </param>
    public RegistryProvider(IRegistrySynchronizer dataSource)
    {
      var indexGenerator = new IndexGenerator();
      // Reserve the first 20 indices for static virtual keys.
      indexGenerator.ExcludedRanges.Add(new IndexRange(0, 20));
      // Reserved indices for registry rootkeys.
      indexGenerator.ExcludedRanges.Add(new IndexRange(0x80000000, 0x80000006));
      var knownKeys = new ObservableDictionary<uint, VirtualRegistryKey>();
      dataSource.SynchronizeRegistryWith(knownKeys);
      _switch = new RegistrySwitch(indexGenerator, knownKeys);
    }

    #endregion

    #region IRegistryProvider Members

    public NativeResultCode OpenKey(uint hKey, string subKeyName, out uint hSubKey)
    {
      string keyName;
      var registry = _switch.GetRegistryFor(hKey, out keyName);
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
      var registry = _switch.GetRegistryFor(hKey, out keyName);
      keyName = HostRegistry.CombineKeyNames(keyName, subKeyName);
      if (registry != null)
        return registry.CreateKey(keyName, out hSubKey, out creationDisposition);
      hSubKey = 0;
      creationDisposition = RegCreationDisposition.NoKeyCreated;
      return NativeResultCode.InvalidHandle;
    }

    public NativeResultCode CloseKey(uint hKey)
    {
      string keyPath;
      var registry = _switch.GetRegistryFor(hKey, out keyPath, false);
      return registry != null ? registry.CloseKey(hKey) : NativeAPI.RegCloseKey(hKey);
    }

    public NativeResultCode DeleteKey(uint hKey)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = _switch.GetRegistryFor(hKey);
      return registry != null
               ? registry.DeleteKey(hKey)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = _switch.GetRegistryFor(hKey);
      return registry != null
               ? registry.QueryValue(hKey, valueName, out value)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = _switch.GetRegistryFor(hKey);
      return registry != null
               ? registry.SetValue(hKey, value)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var registry = _switch.GetRegistryFor(hKey);
      return registry != null
               ? registry.DeleteValue(hKey, valueName)
               : NativeResultCode.InvalidHandle;
    }

    #endregion

  }
}
