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

using AppStract.Core.System.Logging;
using AppStract.Core.Virtualization.Registry;
using Microsoft.Win32.Interop;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// Default implementation of <see cref="IRegistryProvider"/>;
  /// Makes use of <see cref="VirtualRegistry"/> to handle requests.
  /// </summary>
  public sealed class RegistryProvider : IRegistryProvider
  {

    #region Variables

    /// <summary>
    /// The <see cref="VirtualRegistry"/> to pass the requests to.
    /// </summary>
    private readonly VirtualRegistry _virtualRegistry;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="RegistryProvider"/>,
    /// the default implementation of <see cref="IRegistryProvider"/>.
    /// </summary>
    public RegistryProvider()
    {
      _virtualRegistry = new VirtualRegistry();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Loads data from the given <see cref="IRegistryLoader"/> to the underlying resources.
    /// </summary>
    /// <param name="dataSource">The <see cref="IRegistryLoader"/> to load the data from.</param>
    public void LoadRegistry(IRegistryLoader dataSource)
    {
      _virtualRegistry.Initialize(dataSource);
    }

    #endregion

    #region IRegistryProvider Members

    public uint SetValue(uint hKey, string valueName, uint valueType, object data)
    {
      var type = RegistryHelper.ValueTypeFromId(valueType);
      var registryValue = new VirtualRegistryValue(valueName, data, type);
      var stateCode = _virtualRegistry.SetValue(hKey, registryValue);
      GuestCore.Log(new LogMessage(LogLevel.Debug, "SetValue(HKey={0} Name={1} Type={2}) => {3}",
                                   hKey, valueName, type, stateCode));
      return WinError.FromStateCode(stateCode);
    }

    public uint OpenKey(uint hKey, string subKey, out uint hSubKey)
    {
      var winError = _virtualRegistry.OpenKey(hKey, subKey, out hSubKey)
                       ? WinError.ERROR_SUCCESS
                     // Note: This behaviour needs to be updated!
                     // How can we know which one is the illegal value? Is it hKey or subKey?
                       : WinError.ERROR_INVALID_HANDLE;
      GuestCore.Log(new LogMessage(LogLevel.Debug, @"OpenKey({0}\\{1}) = {2}", hKey, subKey, hSubKey));
      return winError;
    }

    public uint CreateKey(uint hKey, string subKey, out uint hSubKey, out int lpdwDisposition)
    {
      RegCreationDisposition creationDisposition;
      var stateCode = _virtualRegistry.CreateKey(hKey, subKey, out hSubKey, out creationDisposition);
      lpdwDisposition = RegistryHelper.DispositionFromRegCreationDisposition(creationDisposition);
      GuestCore.Log(new LogMessage(LogLevel.Debug, "CreateKey(HKey={0} NewSubKey={1}) => {2} HKey={3}",
                                   hKey, subKey, creationDisposition, hSubKey));
      return WinError.FromStateCode(stateCode);
    }

    public uint QueryValue(uint hKey, string valueName, out object value, out uint valueType)
    {
      VirtualRegistryValue virtualRegistryValue;
      var hResult = _virtualRegistry.QueryValue(hKey, valueName, out virtualRegistryValue);
      GuestCore.Log(new LogMessage(LogLevel.Debug, "QueryValue(HKey={0} ValueName={1}) => {2}",
                                   hKey, valueName, hResult));
      value = virtualRegistryValue.Data;
      valueType = hResult == NativeResultCode.Succes
                    ? RegistryHelper.ValueTypeIdFromValueType(virtualRegistryValue.Type)
                    : RegistryHelper.ValueTypeIdFromValueType(ValueType.REG_NONE);
      return WinError.FromStateCode(hResult);
    }

    public uint CloseKey(uint hKey)
    {
      GuestCore.Log(new LogMessage(LogLevel.Debug, "CloseKey(HKey={0})", hKey));
      _virtualRegistry.CloseKey(hKey);
      return WinError.ERROR_SUCCESS;
    }

    #endregion

  }
}
