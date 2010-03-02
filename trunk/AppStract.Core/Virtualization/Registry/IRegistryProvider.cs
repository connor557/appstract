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

namespace AppStract.Core.Virtualization.Registry
{
  /// <summary>
  /// Provides access to the virtual registry.
  /// </summary>
  public interface IRegistryProvider
  {

    /// <summary>
    /// Sets the data for the specified value in the specified registry key.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="valueName"></param>
    /// <param name="valueType"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    uint SetValue(uint hKey, string valueName, uint valueType, byte[] data);

    /// <summary>
    /// Opens the specified registry key.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="subKey"></param>
    /// <param name="hSubKey">The handle to the opened key.</param>
    /// <returns></returns>
    uint OpenKey(uint hKey, string subKey, out uint hSubKey);

    /// <summary>
    /// Creates the specified registry key.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="subKey"></param>
    /// <param name="hSubKey"></param>
    /// <param name="lpdwDisposition"></param>
    /// <returns></returns>
    uint CreateKey(uint hKey, string subKey, out uint hSubKey, out int lpdwDisposition);

    /// <summary>
    /// Retrieves the type and data for a specified value name associated with an open registry key.
    /// </summary>
    /// <param name="hKey"></param>
    /// <param name="valueName"></param>
    /// <param name="data"></param>
    /// <param name="valueType"></param>
    /// <returns></returns>
    uint QueryValue(uint hKey, string valueName, out byte[] data, out uint valueType);

    /// <summary>
    /// Closes a handle to the specified registry key.
    /// </summary>
    /// <param name="hKey"></param>
    /// <returns></returns>
    uint CloseKey(uint hKey);

  }
}
