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
  [Serializable]
  public struct FileTableEntry : ISerializable, IEquatable<FileTableEntry>
  {

    #region Variables

    private string _key;
    private string _value;
    private readonly FileKind _fileKind;

    #endregion

    #region Properties

    /// <summary>
    /// The path as used by the real file sytem.
    /// </summary>
    public string Key
    {
      get { return _key; }
      set { _key = value; }
    }

    /// <summary>
    /// The path as used by the virtual file system.
    /// </summary>
    public string Value
    {
      get { return _value; }
      set { _value = value; }
    }

    /// <summary>
    /// Gets whether the current <see cref="FileTableEntry"/> is a file or a directory.
    /// </summary>
    public FileKind FileKind
    {
      get { return _fileKind; }
    }

    #endregion

    #region Constructors

    public FileTableEntry(string key, string value, FileKind fileKind)
    {
      _key = key;
      _value = value;
      _fileKind = fileKind;
    }

    public FileTableEntry(KeyValuePair<string, string> pair, FileKind fileKind)
      : this(pair.Key, pair.Value, fileKind) { }

    private FileTableEntry(SerializationInfo info, StreamingContext context)
    {
      try
      {
        _key = info.GetString("key");
      }
      catch (SerializationException)
      {
        _key = null;
        CoreBus.Log.Warning("Unable to deserialize the key of a FileTableEntry.");
      }
      try
      {
        _value = info.GetString("value");
      }
      catch (SerializationException)
      {
        _value = null;
        CoreBus.Log.Warning("Unable to deserialize the value of a FileTableEntry."
                            + _key != null
                              ? "With key " + _key
                              : "");
      }
      try
      {
        string fileKind = info.GetString("kind");
        Type enumType = typeof (FileKind);
        _fileKind = Enum.IsDefined(enumType, fileKind)
                      ? (FileKind) Enum.Parse(enumType, fileKind)
                      : FileKind.Unspecified;
      }
      catch (SerializationException)
      {
        _fileKind = FileSystem.FileKind.Unspecified;
        CoreBus.Log.Warning("Unable to deserialize the FileKind of a FileTableEntry."
                            + _key != null
                              ? "With key " + _key
                              : "");
      }
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("key", _key);
      info.AddValue("value", _value);
      info.AddValue("kind", _fileKind.ToString());
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
