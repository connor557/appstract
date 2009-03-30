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
using System.Data;
using System.Data.SQLite;
using AppStract.Core.Virtualization.Registry;
using ValueType=AppStract.Core.Virtualization.Registry.ValueType;

namespace AppStract.Core.Data.Databases
{
  public class RegistryDatabase : Database<VirtualRegistryKey>
  {

    #region Variables

    private const string _DatabaseKeyTable = "registrykeys";
    private const string _DatabaseValueTable = "registryvalues";
    private const string _DatabaseKeyHandle = "id";
    private const string _DatabaseKeyName = "name";
    private const string _DatabaseValueKey = "key";
    private const string _DatabaseValueName = "name";
    private const string _DatabaseValueValue = "value";
    private const string _DatabaseValueType = "type";

    #endregion

    #region Properties

    #endregion

    #region Constructors

    public RegistryDatabase(string connectionString)
      : base(connectionString)
    {

    }

    #endregion

    #region Public Methods

    public override void Initialize()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Reads the complete database to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<VirtualRegistryKey> ReadAll()
    {
      /// Read the keys.
      var keys = ReadAll<VirtualRegistryKey>(new[] {_DatabaseKeyTable},
                                             new[] {_DatabaseKeyHandle, _DatabaseKeyName},
                                             BuildKeyFromReadAllQuery);
      /// Read the values
      foreach (var key in keys)
      {
        var values = ReadAll<VirtualRegistryValue>(new[] {_DatabaseValueTable},
                                                   new[]
                                                     {
                                                       _DatabaseValueKey, _DatabaseValueName,
                                                       _DatabaseValueValue, _DatabaseValueType
                                                     },
                                                   new[]
                                                     {new KeyValuePair<object, object>(_DatabaseValueKey, key.Handle)},
                                                   BuildValueFromReadAllQuery);
        foreach (var value in values)
          key.Values.Add(value.Name, value);
      }
      return keys;
    }

    #endregion

    #region Protected Methods

    protected override void AppendDeleteQuery(SQLiteCommand command, ParameterGenerator seed, VirtualRegistryKey item)
    {
      string param = seed.Next();
      command.CommandText += string.Format("DELETE FROM {0} WHERE {1} = {2}; DELETE FROM {3} WHERE {4} = {2};",
                                           _DatabaseKeyTable, _DatabaseKeyHandle, param,
                                           _DatabaseValueTable, _DatabaseValueKey);
      command.Parameters.AddWithValue(param, item.Handle);
    }

    protected override void AppendInsertQuery(SQLiteCommand command, ParameterGenerator seed, VirtualRegistryKey item)
    {
      var paramHandle = seed.Next();
      var paramName = seed.Next();
      /// Append query for insertion of the key.
      command.CommandText += string.Format("INSERT INTO [{0}] ({1}, {2}) VALUES ({3}, {4});",
                                           _DatabaseKeyTable,
                                           _DatabaseKeyHandle, _DatabaseKeyName,
                                           paramHandle, paramName);
      command.Parameters.AddWithValue(paramHandle, item.Handle);
      command.Parameters.AddWithValue(paramName, item.Path);
      /// Append queries for insertion of the values.
      AppendInsertQueryForValues(command, seed, item.Handle, item.Values.Values);
    }

    protected override void AppendUpdateQuery(SQLiteCommand command, ParameterGenerator seed, VirtualRegistryKey item)
    {
      var paramHandle = seed.Next();
      var paramName = seed.Next();
      /// Append query for update of the key.
      command.CommandText += string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4};",
                                           _DatabaseKeyTable,
                                           _DatabaseKeyName, paramName,
                                           _DatabaseKeyHandle, paramHandle);
      command.Parameters.AddWithValue(paramHandle, item.Handle);
      command.Parameters.AddWithValue(paramName, item.Path);
      /// Delete all values. It's not possible to use an update query.
      paramHandle = seed.Next();
      command.CommandText += string.Format("DELETE FROM {0} WHERE {1} = {2};",
                                           _DatabaseValueTable, _DatabaseValueKey, paramHandle);
      command.Parameters.AddWithValue(paramHandle, item.Handle);
      /// Now re-add all values.
      AppendInsertQueryForValues(command, seed, item.Handle, item.Values.Values);
    }

    #endregion

    #region Private Methods

    private static VirtualRegistryKey BuildKeyFromReadAllQuery(IDataRecord dataRecord)
    {
      return new VirtualRegistryKey((uint)dataRecord.GetValue(0), dataRecord.GetString(1));
    }

    private static VirtualRegistryValue BuildValueFromReadAllQuery(IDataRecord dataRecord)
    {
      var value = new VirtualRegistryValue(dataRecord.GetString(1), dataRecord.GetValue(2), ValueType.REG_NONE);
      string valueType = dataRecord.GetString(3);
      var enumType = typeof (ValueType);
      if (Enum.IsDefined(enumType, valueType))
        value.Type = (ValueType)Enum.Parse(enumType, valueType, true);
      return value;
    }

    private static void AppendInsertQueryForValues(SQLiteCommand command, ParameterGenerator seed, object keyHandle, IEnumerable<VirtualRegistryValue> values)
    {
      foreach (var value in values)
      {
        var paramHandle = seed.Next();
        var paramName = seed.Next();
        var paramValue = seed.Next();
        var paramType = seed.Next();
        command.CommandText
          += string.Format("INSERT INTO [{0}] ({1}, {2}, {3}, {4}) VALUES ({5}, {6}, {7}, {8});",
                           _DatabaseValueTable,
                           _DatabaseValueKey, _DatabaseValueName, _DatabaseValueValue, _DatabaseValueType,
                           paramHandle, paramName, paramValue, paramType);
        command.Parameters.AddWithValue(paramHandle, keyHandle);
        command.Parameters.AddWithValue(paramName, value.Name);
        command.Parameters.AddWithValue(paramValue, value.Data);
        command.Parameters.AddWithValue(paramType, value.Type);
      }
    }

    #endregion

  }
}
