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

using System.Collections.Generic;
using AppStract.Server.Communication;
using AppStract.Server.Registry;


namespace AppStract.Server.Providers.Registry
{
  public class RegistryProvider : IRegistryProvider
  {

    #region Variables

    private VirtualRegistry _virtualRegistry;
    private IResourceSynchronizer _resourceSynchronizer;

    #endregion

    #region Constructors

    public RegistryProvider(IResourceSynchronizer resourceSynchronizer)
    {
      _resourceSynchronizer = resourceSynchronizer;
    }

    #endregion

    #region Public Methods

    public void LoadRegistry()
    {
      //_resourceSynchronizer.LoadRegistryTo(registryKeys);
    }

    #endregion

    #region IRegistryProvider Members

    //public RegistryValue QueryValue(string valueName, uint hKey)
    //{
    //  throw new System.NotImplementedException();
    //}

    //public void SetValue(RegistryValue value)
    //{
    //  throw new System.NotImplementedException();
    //}

    //public uint? OpenKey(uint hkey, string subkey, out uint phkResult)
    //{
    //  /// Query virtual registry.
    //  if (_registry.ContainsKey(hkey))
    //  {
    //    VirtualRegistryHive key = _registry[hkey];
    //    if (string.IsNullOrEmpty(subkey))
    //    {
    //      phkResult = key.Key;
    //      return 0;
    //    }
    //    subkey = subkey.ToLowerInvariant();
    //    foreach (VirtualRegistryHive keySubKey in key.SubKeys)
    //    {
    //      if (keySubKey.Name == subkey)
    //      {
    //        phkResult = keySubKey.Key;
    //        return 0;
    //      }
    //    }
    //  }
    //  /// Query real registry.
    //  else
    //  {
        
    //  }
    //  // Else return error: The configuration registry key could not be opened.
    //  phkResult = 0;
    //  return 1011;
    //}

    #endregion
  }
}
