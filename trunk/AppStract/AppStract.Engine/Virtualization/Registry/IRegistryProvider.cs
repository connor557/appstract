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

namespace AppStract.Engine.Virtualization.Registry
{
  /// <summary>
  /// Provides access to the virtual registry.
  /// </summary>
  public interface IRegistryProvider : IVirtualizationProvider
  {

    /// <summary>
    /// Tries to open a subkey of the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle for the key to open a subkey from.</param>
    /// <param name="subKeyName">The name of the subkey to open.</param>
    /// <param name="hSubKey">The open handle for the opened subkey.</param>
    /// <returns>Whether the subkey could be opened.</returns>
    NativeResultCode OpenKey(uint hKey, string subKeyName, out uint hSubKey);

    /// <summary>
    /// Creates a subkey with the given name for the key handle specified.
    /// </summary>
    /// <param name="hKey">The open key handle to create a subkey for.</param>
    /// <param name="subKeyName">The name of the subkey to create.</param>
    /// <param name="hSubKey">The open handle to the created subkey.</param>
    /// <param name="creationDisposition">Whether the subkey is created or updated.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    NativeResultCode CreateKey(uint hKey, string subKeyName, out uint hSubKey, out RegCreationDisposition creationDisposition);

    /// <summary>
    /// Closes the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle to close.</param>
    NativeResultCode CloseKey(uint hKey);

    /// <summary>
    /// Deletes the key associated with the open key handle specified.
    /// </summary>
    /// <param name="hKey">The open handle of key to delete.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    NativeResultCode DeleteKey(uint hKey);

    /// <summary>
    /// Returns the <see cref="VirtualRegistryValue"/> associated with the given <paramref name="valueName"/>
    /// and the key handle specified.
    /// </summary>
    /// <param name="hKey">The key to get the <see cref="VirtualRegistryValue"/> from.</param>
    /// <param name="valueName">The name of the <see cref="VirtualRegistryValue"/> to return.</param>
    /// <param name="value">The returned <see cref="VirtualRegistryValue"/>.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value);

    /// <summary>
    /// Sets the given <see cref="VirtualRegistryValue"/> to the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle to set the <paramref name="value"/> for.</param>
    /// <param name="value">The <see cref="VirtualRegistryValue"/> to set.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    NativeResultCode SetValue(uint hKey, VirtualRegistryValue value);

    /// <summary>
    /// Deletes the given value from the key handle specified.
    /// </summary>
    /// <param name="hKey">The key handle to delete the value from.</param>
    /// <param name="valueName">The value to delete.</param>
    /// <returns>The error code representing the result of this operation.</returns>
    NativeResultCode DeleteValue(uint hKey, string valueName);

  }
}
