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

using AppStract.Utilities.Observables;

namespace AppStract.Core.Virtualization.FileSystem
{
  /// <summary>
  /// Defines a method to load the file table to an <see cref="ObservableDictionary{TKey,TValue}"/>.
  /// </summary>
  public interface IFileSystemLoader
  {

    /// <summary>
    /// Loads all known key/value pairs of the file table to an <see cref="ObservableDictionary{TKey,TValue}"/>.
    /// </summary>
    /// <param name="fileTable">The <see cref="ObservableDictionary{TKey,TValue}"/> to load the file table to.</param>
    void LoadFileSystemTableTo(ObservableDictionary<string, string> fileTable);

  }
}
