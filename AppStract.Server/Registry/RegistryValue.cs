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

using System.Runtime.Serialization;


namespace AppStract.Server.Registry
{

  public sealed class RegistryValue : ISerializable
  {

    #region Enums

    /// <summary>
    /// The registry data type of the value.
    /// </summary>
    public enum ValueKind
    {
      /// <summary>
      /// No type.
      /// </summary>
      REG_NONE = 0,
      /// <summary>
      /// String value.
      /// </summary>
      REG_SZA = 1,
      /// <summary>
      /// An expandable string value that can contain environment variables.
      /// </summary>
      REG_EXPAND_SZ = 2,
      /// <summary>
      /// Binary data (any arbitrary data).
      /// </summary>
      REG_BINARY = 3,
      /// <summary>
      /// A DWORD value, a 32-bit unsigned integer.
      /// </summary>
      REG_DWORD_LITTLE_ENDIAN = 4,
      /// <summary>
      /// A DWORD value, a 32-bit unsigned integer.
      /// </summary>
      REG_DWORD_BIG_ENDIAN = 5,
      /// <summary>
      /// Symbolic link (UNICODE)
      /// </summary>
      REG_LINK = 6,
      /// <summary>
      /// Multi-string value, an array of unique strings.
      /// </summary>
      REG_MULTI_SZA = 7,
      /// <summary>
      /// Resource List.
      /// </summary>
      REG_RESOURCE_LIST = 8,
      /// <summary>
      /// Resource Descriptor.
      /// </summary>
      REG_FULL_RESOURCE_DESCRIPTOR = 9,
      /// <summary>
      /// Resource Requirements List.
      /// </summary>
      REG_RESOURCE_REQUIREMENTS_LIST = 10,
      /// <summary>
      /// A QWORD value, a 64-bit integer.
      /// </summary>
      REG_QWORD = 11
    }

    #endregion

    #region Variables

    private readonly int _key;
    private readonly string _name;
    private readonly object _data;
    private readonly ValueKind _type;

    #endregion

    #region Properties

    public int Key
    {
      get { return _key; }
    }

    public string Name
    {
      get { return _name; }
    }

    public object Data
    {
      get { return _data; }
    }

    public ValueKind Type
    {
      get { return _type; }
    }

    #endregion

    #region Constructors

    public RegistryValue(int key, string name, object data, ValueKind type)
    {
      _key = key;
      _name = name;
      _data = data;
      _type = type;
    }

    private RegistryValue(SerializationInfo info, StreamingContext context)
    {
      _key = info.GetInt32("key");
      _name = info.GetString("name");
      _data = info.GetValue("data", typeof(object));
      _type = (ValueKind)info.GetValue("type", typeof(ValueKind));
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("key", _key);
      info.AddValue("name", _name);
      info.AddValue("data", _data);
      info.AddValue("type", _type);
    }

    #endregion

  }
}
