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

using System.Data;

namespace AppStract.Utilities.Data
{

  /// <summary>
  /// Represents the method that will build a {TItem} from an <see cref="IDataRecord"/>.
  /// </summary>
  /// <typeparam name="TItem">The type that will be built by the method.</typeparam>
  /// <param name="dataRecord">The <see cref="IDataRecord"/> to read the values from.</param>
  /// <returns></returns>
  public delegate TItem BuildItemFromQueryData<out TItem>(IDataRecord dataRecord);

}
