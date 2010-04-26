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

namespace AppStract.Core.Virtualization.Engine.Registry
{
  /// <summary>
  /// Defines a method to activate the synchronization of the virtual registry with an <see cref="ObservableDictionary{TKey,TValue}"/>.
  /// </summary>
  public interface IRegistrySynchronizer
  {

    /// <summary>
    /// Loads all known registry keys and their associated values to the given <see cref="ObservableDictionary{TKey,TValue}"/>,
    /// and ensures continuous registry synchronization by attaching listeners to <paramref name="keyList"/>.
    /// </summary>
    /// <param name="keyList">The <see cref="ObservableDictionary{TKey,TValue}"/> to synchronize the registry with.</param>
    void SynchronizeRegistryWith(ObservableDictionary<uint, VirtualRegistryKey> keyList);

  }
}
