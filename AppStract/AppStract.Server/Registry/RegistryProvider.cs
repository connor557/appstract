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

using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.Registry;
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
      var request = new RegistryRequest {Handle = hKey};
      var registry = _switch.GetRegistryFor(request);
      if (registry != null)
      {
        request.KeyFullPath = HostRegistry.CombineKeyNames(request.KeyFullPath, subKeyName);
        var result = registry.OpenKey(request);
        hSubKey = request.Handle;
        return result;
      }
      hSubKey = 0;
      return NativeResultCode.InvalidHandle;
    }

    public NativeResultCode CreateKey(uint hKey, string subKeyName, out uint hSubKey, out RegCreationDisposition creationDisposition)
    {
      var request = new RegistryRequest { Handle = hKey };
      var registry = _switch.GetRegistryFor(request);
      request.KeyFullPath = HostRegistry.CombineKeyNames(request.KeyFullPath, subKeyName);
      if (registry != null)
      {
        var result = registry.CreateKey(request, out creationDisposition);
        hSubKey = request.Handle;
        return result;
      }
      hSubKey = 0;
      creationDisposition = RegCreationDisposition.NoKeyCreated;
      return NativeResultCode.InvalidHandle;
    }

    public NativeResultCode CloseKey(uint hKey)
    {
      var request = new RegistryRequest {Handle = hKey};
      var registry = _switch.GetRegistryFor(request, false);
      return registry != null ? registry.CloseKey(request) : NativeAPI.RegCloseKey(hKey);
    }

    public NativeResultCode DeleteKey(uint hKey)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var request = new RegistryRequest {Handle = hKey};
      var registry = _switch.GetRegistryFor(request);
      return registry != null
               ? registry.DeleteKey(request)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      NativeResultCode result;
      var request = new RegistryValueRequest(valueName) { Handle = hKey };
      if (!HiveHelper.IsHiveHandle(hKey))
      {
        var registry = _switch.GetRegistryFor(request);
        result = registry != null
                   ? registry.QueryValue(request)
                   : NativeResultCode.InvalidHandle;
      }
      else
        result = NativeResultCode.AccessDenied;
      value = request.Value;
      return result;
    }

    public NativeResultCode SetValue(uint hKey, VirtualRegistryValue value)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var request = new RegistryValueRequest {Handle = hKey, Value = value};
      var registry = _switch.GetRegistryFor(request);
      return registry != null
               ? registry.SetValue(request)
               : NativeResultCode.InvalidHandle;
    }

    public NativeResultCode DeleteValue(uint hKey, string valueName)
    {
      if (HiveHelper.IsHiveHandle(hKey))
        return NativeResultCode.AccessDenied;
      var value = new VirtualRegistryValue {Name = valueName};
      var request = new RegistryValueRequest { Handle = hKey, Value = value };
      var registry = _switch.GetRegistryFor(request);
      return registry != null
               ? registry.DeleteValue(request)
               : NativeResultCode.InvalidHandle;
    }

    #endregion

  }
}
