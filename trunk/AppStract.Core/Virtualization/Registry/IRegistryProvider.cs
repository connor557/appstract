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
  public interface IRegistryProvider
  {

    //RegistryValue QueryValue(string valueName, uint hKey);

    //void SetValue(RegistryValue value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hkey">A handle to an open registry key.</param>
    /// <param name="subkey">
    /// The name of the registry subkey to be opened. 
    /// Key names are not case sensitive.
    /// If this parameter is NULL or a pointer to an empty string, the function will open a new handle to the key identified by the hKey parameter.
    /// </param>
    /// <param name="phkResult"></param>
    /// <returns></returns>
    //uint? OpenKey(uint hkey, string subkey, out uint phkResult);

  }
}
