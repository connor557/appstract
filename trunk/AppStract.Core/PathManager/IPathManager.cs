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

namespace AppStract.Core.Paths
{

  /// <summary>
  /// Convertor for pathdescriptions to absolute paths.
  /// </summary>
  /// <remarks>
  /// Using the ILog service in an implementation of IPathService will result in an
  /// unavoidable circular dependency which may cause a fatal exception during the disposal
  /// of the core.
  /// </remarks>
  public interface IPathManager
  {

    /// <summary>
    /// Sets the given path for the describer.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="pathDescription"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    void SetPath(string pathDescription, string path);

    /// <summary>
    /// Determines whether the specified pathdescription exists.
    /// </summary>
    /// <param name="pathDescription"></param>
    /// <returns></returns>
    bool Exists(string pathDescription);

    /// <summary>
    /// Gets the path fitting the description.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="PathDescriptionNotFoundException"></exception>
    /// <param name="pathDescription"></param>
    /// <returns></returns>
    string GetPath(string pathDescription);

  }
}
