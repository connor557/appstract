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
  public sealed class RegistryKey : ISerializable
  {

    #region Variables

    private readonly int _key;
    private readonly string _name;

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

    #endregion

    #region Constructors

    public RegistryKey(int key, string name)
    {
      _key = key;
      _name = name;
    }

    private RegistryKey(SerializationInfo info, StreamingContext context)
    {
      _key = info.GetInt32("key");
      _name = info.GetString("name");
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("key", _key);
      info.AddValue("name", _name);
    }

    #endregion

  }
}
