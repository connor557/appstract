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

using AppStract.Core.Virtualization.Engine;
using Microsoft.Win32;

namespace AppStract.Server.Registry
{
  /// <summary>
  /// Represents a request to the virtual registry.
  /// </summary>
  public class RegistryRequest
  {

    #region Variables

    private VirtualizationType _accessMechanism;
    private uint _handle;
    private RegistryHive _hive;
    private string _keyFullName;
    private string _keyFullPath;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the <see cref="VirtualizationType"/> to use for the current request.
    /// </summary>
    public VirtualizationType VirtualizationType
    {
      get { return _accessMechanism; }
      set { _accessMechanism = value; }
    }

    /// <summary>
    /// Gets or sets the key handle associated with the current request.
    /// </summary>
    public uint Handle
    {
      get { return _handle; }
      set { _handle = value; }
    }

    /// <summary>
    /// Gets the <see cref="RegistryHive"/> associated with the current request.
    /// </summary>
    public RegistryHive Hive
    {
      get { return _hive; }
    }

    /// <summary>
    /// Gets the full key name.
    /// This is the <see cref="KeyFullPath"/> without the hive's name included.
    /// </summary>
    public string KeyFullName
    {
      get { return _keyFullName; }
    }

    /// <summary>
    /// Gets or sets the full key path associated with the current request.
    /// This path includes both the name of the registry hive and the subkey's full name.
    /// </summary>
    public string KeyFullPath
    {
      get { return _keyFullPath; }
      set
      {
        _keyFullPath = value;
        _hive = value != null
                  ? HiveHelper.GetHive(value, out _keyFullName)
                  : default(RegistryHive);
      }
    }

    #endregion

    #region Constructors

    public RegistryRequest()
    {
      _handle = 0;
      _keyFullPath = string.Empty;
      _accessMechanism = VirtualizationType.Virtual;
    }

    public RegistryRequest(RegistryRequest request)
    {
      _accessMechanism = request.VirtualizationType;
      _handle = request.Handle;
      _hive = request.Hive;
      _keyFullName = request.KeyFullName;
      _keyFullPath = request.KeyFullPath;
    }

    #endregion

  }
}
