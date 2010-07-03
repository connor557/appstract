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

namespace AppStract.Core.Virtualization.Engine
{
  /// <summary>
  /// The type of virtualization to apply when executing operations on an associated resource.
  /// </summary>
  public enum VirtualizationType
  {
    /// <summary>
    /// All read and write actions are passed to the virtualization engine.
    /// </summary>
    Virtual,
    /// <summary>
    /// All read and write actions are passed to the virtualization engine,
    /// with the addition that read actions on resources unknown by the engine
    /// cause the resource to be copied from the host environment to the virtual environment.
    /// </summary>
    VirtualWithFallback,
    /// <summary>
    /// Read actions on unknown resources are performed on the host environment,
    /// while write actions are always performed on the virtual environment.
    /// </summary>
    TransparentRead,
    /// <summary>
    /// All read and write actions are passed to the host environment.
    /// </summary>
    Transparent
  }
}
