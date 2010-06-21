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

using AppStract.Utilities.Observables;

namespace AppStract.Core.Virtualization.Engine.FileSystem
{
  /// <summary>
  /// Defines a method to activate the synchronization of the file table used by the virtual file system,
  /// with an <see cref="ObservableDictionary{TKey,TValue}"/>.
  /// </summary>
  public interface IFileSystemSynchronizer
  {

    /// <summary>
    /// Loads all known file table entries to the given <see cref="ObservableDictionary{TKey,TValue}"/>,
    /// and ensures continuous registry synchronization by attaching listeners to <paramref name="fileTable"/>.
    /// </summary>
    /// <param name="fileTable">The <see cref="ObservableDictionary{TKey,TValue}"/> to load the file table to.</param>
    void SynchronizeFileSystemTableWith(ObservableDictionary<string, string> fileTable);

  }
}
