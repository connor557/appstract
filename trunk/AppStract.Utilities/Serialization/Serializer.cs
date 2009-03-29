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
using System.IO;
using System.Xml.Serialization;

namespace AppStract.Utilities.Serialization
{
  public static class Serializer
  {

    /// <summary>
    /// Deserializes an object from type <typeparamref name="T"/> from the specified file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static T Deserialize<T>(string filename)
    {
      var serializer = new XmlSerializer(typeof(T));
      try
      {
        using (var stream = new StreamReader(filename))
        {
          return (T)serializer.Deserialize(stream);
        }
      }
      catch (Exception e)
      {
        throw new SerializationException("Can't deserialize an object of type " + typeof(T) + " from " + filename, e);
      }
    }

    /// <summary>
    /// Serializes the given data to the specified file.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool Serialize(string filename, object data)
    {
      var serializer = new XmlSerializer(data.GetType());
      try
      {
        using (var stream = new StreamWriter(filename))
          serializer.Serialize(stream, data);
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

  }
}
