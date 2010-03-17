#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
    This file is part of AppStract

    AppStract is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    AppStract is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with AppStract.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

namespace AppStract.Utilities.ManagedFusion.Fusion
{

  /// <summary>
  /// Specifies the attributes of an <see cref="IAssemblyName"/> object 
  /// when it is constructed by the CreateAssemblyNameObject function.
  /// </summary>
  internal enum CreateDisposition
  {
    /// <summary>
    /// Indicates that the parameter passed is a textual identity.
    /// </summary>
    /// <remarks>
    /// If this flag is specified, the szAssemblyName parameter is a full assembly name and is parsed to 
    /// the individual properties. If the flag is not specified, szAssemblyName is the "Name" portion of the assembly name.
    /// </remarks>
    ParseDisplayName = 0x1,
    /// <summary>
    /// Sets a few default values.
    /// </summary>
    /// <remarks>
    /// If this flag is specified, certain properties, such as processor architecture, are set to 
    ///	their default values.
    /// </remarks>
    SetDefaultValues = 0x2,
  }

}
