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

namespace AppStract.Core.Virtualization.Registry
{
  public struct VirtualRegistryValue
  {

    #region Variables

    private string _name;
    private object _data;
    private ValueType _type;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the name of the value.
    /// </summary>
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    /// <summary>
    /// Gets or sets the data associated to the value.
    /// </summary>
    public object Data
    {
      get { return _data; }
      set { _data = value; }
    }

    /// <summary>
    /// Gets or sets the type of data stored by the current value.
    /// </summary>
    public ValueType Type
    {
      get { return _type; }
      set { _type = value; }
    }

    #endregion

    #region Constructors

    public VirtualRegistryValue(string name, object data, ValueType type)
    {
      _name = name;
      _data = data;
      _type = type;
    }

    #endregion

  }
}
