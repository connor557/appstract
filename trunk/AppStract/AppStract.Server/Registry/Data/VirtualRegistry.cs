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
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Interop;
using AppStract.Core.Virtualization.Registry;
using AppStract.Utilities.Extensions;
using ValueType = AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Server.Registry.Data
{
  /// <summary>
  /// Database for all the keys known by the virtual registry.
  /// </summary>
  public sealed class VirtualRegistry : RegistryBase
  {

    #region Constructors

    public VirtualRegistry(IndexGenerator indexGenerator, IDictionary<uint, VirtualRegistryKey> knownKeys)
      : base(indexGenerator, knownKeys)
    {
    }

    #endregion

    #region Overridden Methods

    public override bool OpenKey(string keyFullPath, out uint hResult)
    {
      var virtualKeyPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      if (base.OpenKey(virtualKeyPath, out hResult))
        return true;
      if (!HostRegistry.KeyExists(keyFullPath))
        return false;
      var virtualRegistryKey = ConstructRegistryKey(virtualKeyPath);
      WriteKey(virtualRegistryKey, true);
      hResult = virtualRegistryKey.Handle;
      return true;
    }

    public override NativeResultCode CreateKey(string keyFullPath, out uint hKey, out RegCreationDisposition creationDisposition)
    {
      keyFullPath = RegistryTranslator.ToVirtualPath(keyFullPath);
      return base.CreateKey(keyFullPath, out hKey, out creationDisposition);
    }

    public override NativeResultCode QueryValue(uint hKey, string valueName, out VirtualRegistryValue value)
    {
      var resultCode = base.QueryValue(hKey, valueName, out value);
      if (resultCode != NativeResultCode.FileNotFound)
        return resultCode;
      // Base doesn't know the requested value.
      string keyPath;
      if (!IsKnownKey(hKey, out keyPath))
        return NativeResultCode.InvalidHandle;
      // Query the value from the real registry.
      try
      {
        ValueType valueType;
        var realKeyPath = RegistryTranslator.ToRealPath(keyPath);
        var data = HostRegistry.QueryValue(realKeyPath, valueName, out valueType);
        if (data == null)
          return NativeResultCode.FileNotFound;
        value = new VirtualRegistryValue(valueName, data.ToByteArray(), valueType);
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
      // Determine whether the newly acquired value needs to be written to the base.
      var access = HiveHelper.GetHive(keyPath).GetAccessMechanism();
      if (access == AccessMechanism.CreateAndCopy)
      {
        var key = new VirtualRegistryKey(hKey, keyPath);
        key.Values.Add(valueName, value);
        WriteKey(key, false);
      }
      return NativeResultCode.Success;
    }

    #endregion

  }
}
