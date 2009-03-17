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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AppStract.Core.Virtualization.FileSystem
{

  /// <summary>
  /// FileTableEntry is a key/value pair used to link a path used in the real file system
  /// to a path used in the virtual file system.
  /// </summary>
  public struct FileTableEntry : ISerializable, IEquatable<FileTableEntry>
  {

    #region Variables

    private readonly string _key;
    private readonly string _value;

    #endregion

    #region Properties

    public string Key
    {
      get { return _value; }
    }

    public string Value
    {
      get { return _value; }
    }

    #endregion

    #region Constructors

    public FileTableEntry(string key, string value)
    {
      _key = key;
      _value = value;
    }

    public FileTableEntry(KeyValuePair<string, string> pair)
      : this(pair.Key, pair.Value) { }

    private FileTableEntry(SerializationInfo info, StreamingContext context)
    {
      try
      {
        _key = info.GetString("key");
      }
      catch (SerializationException)
      {
        _key = null;
        /// ToDo: Log the exception.
      }
      try
      {
        _value = info.GetString("value");        
      }
      catch(SerializationException)
      {
        _value = null;
        /// ToDo: Log the exception.
      }
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("key", _key);
      info.AddValue("value", _value);
    }

    #endregion

    #region IEquatable<FileTableEntry> Members

    public bool Equals(FileTableEntry other)
    {
      return _key == other.Key;
    }

    #endregion

  }
}
