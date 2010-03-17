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

using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;

namespace AppStract.Utilities.Helpers
{
  /// <summary>
  /// Provides information on the local machine.
  /// </summary>
  public static class LocalMachine
  {

    #region Variables

    private static string _identifier;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets an identifier, which is supposed to be unique, for the current machine.
    /// </summary>
    /// <returns></returns>
    public static string Identifier
    {
      get
      {
        if (_identifier == null) _identifier = /* GetCPUId() + GetMotherBoardID() +*/ GetMacAddresses().First();
        return _identifier;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the identifier of the first CPU found in the current machine.
    /// </summary>
    /// <returns></returns>
    public static string GetCPUId()
    {
      var prop = GetProperty("Win32_Processor", "ProcessorId");
      if (prop == null || prop.Value == null)
        return null;
      return prop.Value.ToString();
    }

    /// <summary>
    /// Gets the identifier of the motherboard used in the current machine.
    /// </summary>
    /// <returns></returns>
    public static string GetMotherBoardID()
    {
      var prop = GetProperty("Win32_BaseBoard", "SerialNumber");
      if (prop == null || prop.Value == null)
        return null;
      return prop.Value.ToString();
    }

    /// <summary>
    /// Gets all MAC-addresses used for the current machine.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<string> GetMacAddresses()
    {
      var interfaces = NetworkInterface.GetAllNetworkInterfaces();
      foreach (var networkInterface in interfaces)
        yield return networkInterface.GetPhysicalAddress().ToString();
    }

    #endregion

    #region Private Methods

    private static PropertyData GetProperty(string managementClassPath, string propertyName)
    {
      var moc = (new ManagementClass(managementClassPath)).GetInstances();
      foreach (var mo in moc)
        return mo.Properties[propertyName];
      return null;
    }

    #endregion

  }
}
