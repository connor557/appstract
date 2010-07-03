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

namespace AppStract.Core.Virtualization.Engine.Registry
{
  /// <summary>
  /// The required virtualization type on a key,
  /// which decides the kind of registry virtualization to be used when
  /// executing operations associated to the key.
  /// </summary>
  public enum VirtualizationType
  {
    /// <summary>
    /// All read and write actions are passed to the virtual registry.
    /// </summary>
    Virtual,
    /// <summary>
    /// All read actions on unknown keys copy the requested key and/or values from the host's registry.
    /// Write actions are always performed on the virtual registry.
    /// </summary>
    CreateAndCopy,
    /// <summary>
    /// Read actions are transparent on unknown keys,
    /// write actions are always performed on the virtual registry.
    /// </summary>
    TransparentRead,
    /// <summary>
    /// All read and write actions are passed to the host's registry.
    /// </summary>
    Transparent,
  }
}
