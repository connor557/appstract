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

namespace AppStract.Server.Providers.Registry
{
  /// <summary>
  /// The required access mechanism on a key.
  /// </summary>
  public enum AccessMechanism
  {
    /// <summary>
    /// All read and write actions are passed to the host's registry.
    /// </summary>
    Transparent,
    /// <summary>
    /// Read actions are transparant on unknown keys,
    /// write actions are always performed on the virtual registry.
    /// </summary>
    TransparentRead,
    /// <summary>
    /// The first read action copies the requested values of the key from the host's registry.
    /// Write actions are always performed on the virtual registry.
    /// </summary>
    CreateAndCopy
  }
}
