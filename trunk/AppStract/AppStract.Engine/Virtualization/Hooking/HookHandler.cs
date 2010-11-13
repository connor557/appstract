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

namespace AppStract.Engine.Virtualization.Hooking
{
  /// <summary>
  /// Abstract class representing a type able to handle intercepted API hooks.
  /// </summary>
  public abstract class HookHandler
  {

    #region Protected Methods

    /// <summary>
    /// Suppresses a possible <see cref="NullReferenceException"/> when assigning <paramref name="value"/> to <paramref name="destination"/>.
    /// Such a <see cref="NullReferenceException"/> might occur when the native side provides a null value for <paramref name="destination"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="destination"></param>
    protected static void SafeWrite<T>(T value, ref T destination) where T : struct
    {
      try
      {
        destination = value;
      }
      catch (NullReferenceException)
      {

      }
    }

    /// <summary>
    /// Tries to parse the <see cref="Int64"/> value of <paramref name="pointer"/> to an <see cref="UInt32"/>
    /// </summary>
    /// <param name="pointer"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    protected static bool TryParse(UIntPtr pointer, out uint result)
    {
      result = (uint)pointer;
      return true;
    }

    #endregion

  }
}
