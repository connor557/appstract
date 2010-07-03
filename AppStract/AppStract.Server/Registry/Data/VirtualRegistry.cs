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
using AppStract.Core.Data.Databases;
using AppStract.Core.Virtualization.Engine;
using AppStract.Core.Virtualization.Engine.Registry;
using AppStract.Utilities.Extensions;
using ValueType = AppStract.Core.Virtualization.Engine.Registry.ValueType;

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

    public override NativeResultCode OpenKey(RegistryRequest request)
    {
      var virtualKeyPath = RegistryTranslator.ToVirtualPath(request.KeyFullPath);
      var virtReq = new RegistryRequest(request) { KeyFullPath = virtualKeyPath };
      if (base.OpenKey(virtReq) == NativeResultCode.Success)
      {
        request.Handle = virtReq.Handle;
        return NativeResultCode.Success;
      }
      if (request.VirtualizationType == VirtualizationType.Virtual
          || !HostRegistry.KeyExists(request.KeyFullPath))
        return NativeResultCode.FileNotFound;
      var virtualRegistryKey = ConstructRegistryKey(virtualKeyPath);
      WriteKey(virtualRegistryKey, true);
      request.Handle = virtualRegistryKey.Handle;
      return NativeResultCode.Success;
    }

    public override NativeResultCode CreateKey(RegistryRequest request, out RegCreationDisposition creationDisposition)
    {
      var virtualKeyPath = RegistryTranslator.ToVirtualPath(request.KeyFullPath);
      var virtReq = new RegistryRequest(request) {KeyFullPath = virtualKeyPath};
      var result = base.CreateKey(virtReq, out creationDisposition);
      request.Handle = virtReq.Handle;
      return result;
    }

    public override NativeResultCode QueryValue(RegistryValueRequest request)
    {
      var resultCode = base.QueryValue(request);
      if (resultCode != NativeResultCode.FileNotFound)
        return resultCode;                      // Base knows the value
      if (!IsKnownKey(request))
        return NativeResultCode.InvalidHandle;  // Base does not know the handle
      if (request.VirtualizationType == VirtualizationType.Virtual)
        return NativeResultCode.FileNotFound;   // Not allowed to retrieve value from host registry
      // Query the value from the real registry.
      try
      {
        ValueType valueType;
        var realKeyPath = RegistryTranslator.ToRealPath(request.KeyFullPath);
        var data = HostRegistry.QueryValue(realKeyPath, request.Value.Name, out valueType);
        if (data == null)
          return NativeResultCode.FileNotFound;
        request.Value = new VirtualRegistryValue(request.Value.Name, data.ToByteArray(), valueType);
      }
      catch
      {
        return NativeResultCode.AccessDenied;
      }
      // Determine whether the newly acquired value needs to be written to the base.
      if (request.VirtualizationType < VirtualizationType.TransparentRead)
      {
        var key = new VirtualRegistryKey(request.Handle, request.KeyFullPath);
        key.Values.Add(request.Value.Name, request.Value);
        WriteKey(key, false);
      }
      return NativeResultCode.Success;
    }

    #endregion

  }
}
