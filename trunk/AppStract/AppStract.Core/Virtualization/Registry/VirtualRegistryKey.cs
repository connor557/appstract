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
using System.Collections.Generic;

namespace AppStract.Core.Virtualization.Registry
{
  [Serializable]
  public class VirtualRegistryKey
  {

    #region Variables

    private uint _keyIndex;
    private string _keyPath;
    private IDictionary<string, VirtualRegistryValue> _values;

    #endregion

    #region Properties

    /// <summary>
    /// The handle of the key, an index used in the virtual registry.
    /// </summary>
    public uint Handle
    {
      get { return _keyIndex; }
      protected set { _keyIndex = value;}
    }

    /// <summary>
    /// The full path of the key.
    /// </summary>
    public string Path
    {
      get { return _keyPath; }
      protected set { _keyPath = value; }
    }
    
    /// <summary>
    /// The name of the key.
    /// </summary>
    public string Name
    {
      get
      {
        int index = _keyPath.LastIndexOf('\\');
        if (index == -1)
          return _keyPath;
        return _keyPath.Substring(index);
      }
    }

    /// <summary>
    /// The loaded values of the key.
    /// This holds not necessairly all known values for the key.
    /// </summary>
    public IDictionary<string, VirtualRegistryValue> Values
    {
      get { return _values; }
      protected set { _values = value; }
    }

    #endregion

    #region Constructors

    protected VirtualRegistryKey() { }

    public VirtualRegistryKey(uint hKey, string fullPath)
    {
      _keyIndex = hKey;
      _keyPath = fullPath;
      _values = new Dictionary<string, VirtualRegistryValue>(3);
    }

    public VirtualRegistryKey(uint hKey, string fullPath, IDictionary<string, VirtualRegistryValue> values)
    {
      _keyIndex = hKey;
      _keyPath = fullPath;
      _values = values;
    }

    #endregion

  }
}
