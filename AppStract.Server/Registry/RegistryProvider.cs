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
  public class RegistryProvider : IRegistryProvider
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
      GuestCore.Log(new LogMessage(LogLevel.Debug, "Set value: {0} [HKey: {1} || Type: {2}]",
        valueName, hKey, type));
      var registryValue = new VirtualRegistryValue(valueName, data, type);
      var stateCode = _virtualRegistry.SetValue(hKey, registryValue);
      return WinError.FromStateCode(stateCode);
    }

    public uint OpenKey(uint hKey, string subKey, out uint hSubKey)
    {
      GuestCore.Log(new LogMessage(LogLevel.Debug, @"Open key: {0}\\{1}", hKey, subKey));
      return _virtualRegistry.OpenKey(hKey, subKey, out hSubKey)
               ? WinError.ERROR_SUCCESS
               /// Note: This behaviour needs to be updated!
               /// How can we know which one is the illegal value? Is it hKey or subKey?
               : WinError.ERROR_INVALID_HANDLE;
    }

    public uint CreateKey(uint hKey, string subKey, out uint hSubKey, out int lpdwDisposition)
    {
      GuestCore.Log(new LogMessage(LogLevel.Debug, "Create key {0} at HKey {1}", subKey, hKey));
      RegCreationDisposition creationDisposition;
      var stateCode = _virtualRegistry.CreateKey(hKey, subKey, out hSubKey, out creationDisposition);
      lpdwDisposition = RegistryHelper.DispositionFromRegCreationDisposition(creationDisposition);
      return WinError.FromStateCode(stateCode);
    }

    public uint QueryValue(uint hKey, string valueName, out object value, out uint valueType)
    {
      GuestCore.Log(new LogMessage(LogLevel.Debug, "Query value {0} from HKey {1}",
        valueName, hKey));
      VirtualRegistryValue virtualRegistryValue;
      NativeResultCode code = _virtualRegistry.QueryValue(hKey, valueName, out virtualRegistryValue);
      value = virtualRegistryValue.Data;
      valueType = RegistryHelper.ValueTypeIdFromValueType(virtualRegistryValue.Type);
      return WinError.FromStateCode(code);
    }

    public uint CloseKey(uint hKey)
    {
      GuestCore.Log(new LogMessage(LogLevel.Debug, "Close HKey " + hKey));
      _virtualRegistry.CloseKey(hKey);
      return WinError.ERROR_SUCCESS;
    }

    #endregion

  }
}
