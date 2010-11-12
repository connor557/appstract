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

using AppStract.Core.Virtualization.Engine.Registry;

namespace AppStract.Engine.Engine.Registry
{
  /// <summary>
  /// Represents a request for a value in the virtual registry.
  /// </summary>
  public class RegistryValueRequest : RegistryRequest
  {

    #region Properties

    /// <summary>
    /// The <see cref="VirtualRegistryValue"/> associated to the current <see cref="RegistryValueRequest"/>.
    /// </summary>
    public VirtualRegistryValue Value
    { get; set; }

    #endregion

    #region Constructors

    public RegistryValueRequest()
    {
      Value = new VirtualRegistryValue(null, null, ValueType.INVALID);
    }

    public RegistryValueRequest(string valueName)
    {
      Value = new VirtualRegistryValue(valueName, null, ValueType.INVALID);
    }

    public RegistryValueRequest(RegistryRequest request)
      : base(request)
    {
      Value = new VirtualRegistryValue(null, null, ValueType.INVALID);
    }

    public RegistryValueRequest(RegistryValueRequest request)
      : base(request)
    {
      Value = request.Value;
    }

    #endregion

  }
}
