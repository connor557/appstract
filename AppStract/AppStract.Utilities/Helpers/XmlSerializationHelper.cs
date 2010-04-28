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
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace AppStract.Utilities.Helpers
{
  /// <summary>
  /// Serializes and deserializes objects into and from XML documents.
  /// </summary>
  public static class XmlSerializationHelper
  {

    /// <summary>
    /// Deserializes an object from type <typeparamref name="T"/> from the specified file.
    /// </summary>
    /// <exception cref="SerializationException">
    /// A <see cref="SerializationException"/> is thrown if the content of the file
    /// can't be deserialized to an object of type <see cref="T"/>.
    /// </exception>
    /// <typeparam name="T">The type of the object to deserialize from the XML-file specified.</typeparam>
    /// <param name="filename">The file containing the XML-data to deserialize.</param>
    /// <returns>The deserialized object.</returns>
    public static T Deserialize<T>(string filename)
    {
      var serializer = new XmlSerializer(typeof(T));
      try
      {
        using (var stream = new StreamReader(filename))
          return (T)serializer.Deserialize(stream);
      }
      catch (Exception e)
      {
        throw new SerializationException("Can't deserialize an object of type " + typeof(T) + " from " + filename, e);
      }
    }

    /// <summary>
    /// Serializes the given data to the specified file as XML.
    /// </summary>
    /// <exception cref="SerializationException">
    /// A <see cref="SerializationException"/> is thrown if the object's type can't be serialized to an XML-file.
    /// </exception>
    /// <param name="filename">The file to serialize the data to.</param>
    /// <param name="data">The data to serialize.</param>
    public static void Serialize(string filename, object data)
    {
      var ns = new XmlSerializerNamespaces();
      ns.Add("", "");
      var serializer = new XmlSerializer(data.GetType());
      try
      {
        using (var stream = new StreamWriter(filename))
          serializer.Serialize(stream, data, ns);
      }
      catch (Exception e)
      {
        throw new SerializationException("Can't serialize an object of type " + data.GetType() + " to " + filename, e);
      }
    }

    /// <summary>
    /// Serializes the given data to the specified file as XML.
    /// </summary>
    /// <param name="filename">The file to serialize the data to.</param>
    /// <param name="data">The data to serialize.</param>
    /// <returns>True if serialization succeeded; False, otherwise.</returns>
    public static bool TrySerialize(string filename, object data)
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

    /// <summary>
    /// Deserializes an object from type <typeparamref name="T"/> from the specified XML formatted string.
    /// </summary>
    /// <typeparam name="T">Type of the object declared in the XML formatted string.</typeparam>
    /// <param name="data">The string containing the XML to deserialize from.</param>
    /// <exception cref="SerializationException">
    /// A <see cref="SerializationException"/> is thrown if the content of the string
    /// can't be deserialized to an object of type <see cref="T"/>.
    /// </exception>
    /// <returns></returns>
    public static T DeserializeFromString<T>(string data)
    {
      var serializer = new XmlSerializer(typeof(T));
      try
      {
        using (var stream = new StringReader("myString"))
          return (T)serializer.Deserialize(stream);
      }
      catch (Exception e)
      {
        throw new SerializationException(
          "Can't deserialize an object of type " + typeof(T) + " from \"" + data + "\"", e);
      }
    }

    /// <summary>
    /// Serializes the given data to an XML formatted string.
    /// </summary>
    /// <exception cref="SerializationException">
    /// A <see cref="SerializationException"/> is thrown if the object's type can't be serialized to an XML formatted string.
    /// </exception>
    /// <param name="data">The data to serialize.</param>
    /// <returns></returns>
    public static string SerializeToString(object data)
    {
      var serializer = new XmlSerializer(data.GetType());
      try
      {
        var strBuilder = new StringBuilder();
        using (var stream = new StringWriter(strBuilder))
          serializer.Serialize(stream, data);
        strBuilder.Remove(0, strBuilder.ToString().IndexOf(Environment.NewLine));
        int i = strBuilder.ToString().IndexOf(' '); 
        strBuilder.Remove(i, strBuilder.ToString().IndexOf('>') - i);
        return strBuilder.ToString();
      }
      catch (Exception e)
      {
        throw new SerializationException(
          "Can't serialize an object of type " + data.GetType(), e);
      }
    }

  }
}