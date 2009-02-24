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


namespace AppStract.Server.FileSystem
{
  public sealed class FileTableEntry : ISerializable
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

    private FileTableEntry(SerializationInfo info, StreamingContext context)
    {
      _key = info.GetString("key");
      _value = info.GetString("value");
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("key", _key);
      info.AddValue("value", _value);
    }

    #endregion

  }
}
