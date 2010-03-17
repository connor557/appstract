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

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AppStract.Utilities.Helpers
{

  /// <summary>
  /// A generic implementation of <see cref="IEqualityComparer{T}"/> for enumeration types.
  /// </summary>
  /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
  public sealed class EnumComparer<TEnum> : IEqualityComparer<TEnum>
      where TEnum : struct, IComparable, IConvertible, IFormattable
  {

    #region Variables

    /// <summary>
    /// Determins the equality of two instances of <see cref="TEnum"/>.
    /// </summary>
    private static Func<TEnum, TEnum, bool> _equals;
    /// <summary>
    /// Returns the hash code for the specified <see cref="TEnum"/> value.
    /// </summary>
    private static Func<TEnum, int> _getHashCode;

    #endregion

    #region Constructors

    /// <summary>
    /// Initiaizes a new instance of <see cref="EnumComparer{TEnum}"/>.
    /// </summary>
    /// <exception cref="NotSupportedException">
    /// A <see cref="NotSupportedException"/> is thrown if type parameter <see cref="TEnum"/> does not represent an enumeration.
    /// </exception>
    public EnumComparer()
    {
      if (_equals != null && _getHashCode != null)
        return; // Static variables are already instantiated for this specific type of EnumComparer.
      if (!typeof(TEnum).IsEnum)
        throw new NotSupportedException("Type parameter TEnum is expected to be an Enum. "
                                        + typeof(TEnum) + " is not supported.");
      _equals = CreateEqualsMethod();
      _getHashCode = GreateGetHashCodeMethod();
    }

    #endregion

    #region Static Function Generators

    /// <summary>
    /// Returns a method that determines the equality of two instances of <see cref="TEnum"/>.
    /// </summary>
    /// <returns></returns>
    private static Func<TEnum, TEnum, bool> CreateEqualsMethod()
    {
      var xParam = Expression.Parameter(typeof(TEnum), "x");
      var yParam = Expression.Parameter(typeof(TEnum), "y");
      var equalExpression = Expression.Equal(xParam, yParam);
      return Expression.Lambda<Func<TEnum, TEnum, bool>>(equalExpression, new[] { xParam, yParam }).Compile();
    }

    /// <summary>
    /// Returns a method that generates the hash code of an instance of <see cref="TEnum"/>.
    /// </summary>
    /// <returns></returns>
    private static Func<TEnum, int> GreateGetHashCodeMethod()
    {
      var objParam = Expression.Parameter(typeof(TEnum), "obj");
      var underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
      var convertExpression = Expression.Convert(objParam, underlyingType);
      var getHashCodeMethod = underlyingType.GetMethod("GetHashCode");
      var getHashCodeExpression = Expression.Call(convertExpression, getHashCodeMethod);
      return Expression.Lambda<Func<TEnum, int>>(getHashCodeExpression, new[] { objParam }).Compile();
    }

    #endregion

    #region IEqualityComparer<TEnum> Members

    public bool Equals(TEnum x, TEnum y)
    {
      return _equals(x, y);
    }

    public int GetHashCode(TEnum obj)
    {
      return _getHashCode(obj);
    }

    #endregion

  }
}