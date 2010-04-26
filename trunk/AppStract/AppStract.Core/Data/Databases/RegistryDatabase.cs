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
using System.IO;
using AppStract.Core.Virtualization.Engine.Registry;
using AppStract.Utilities.Helpers;
using ValueType = AppStract.Core.Virtualization.Engine.Registry.ValueType;

namespace AppStract.Core.Data.Databases
{
  /// <summary>
  /// Interface class for the registry database.
  /// </summary>
  public class RegistryDatabase : Database<VirtualRegistryKey>
  {

    #region Constants

    /// <summary>
    /// The name, as used in the database, of the table containing all known registry keys.
    /// </summary>
    private const string _DatabaseKeyTable = "registrykeys";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseKeyTable"/> which holds the identifiers of the known keys.
    /// </summary>
    private const string _DatabaseKeyHandle = "id";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseKeyTable"/> which holds the names of the known keys.
    /// </summary>
    private const string _DatabaseKeyName = "name";
    /// <summary>
    /// The name, as used in the database, of the table containing all known registry values.
    /// </summary>
    private const string _DatabaseValueTable = "registryvalues";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseValueTable"/> which holds for each known value
    /// the identifier of the key under which the value is stored.
    /// </summary>
    private const string _DatabaseValueKey = "key";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseValueTable"/> which holds the names of the registry values.
    /// </summary>
    private const string _DatabaseValueName = "name";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseValueTable"/> which holds the value data of the registry values.
    /// </summary>
    private const string _DatabaseValueValue = "value";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseValueTable"/> which holds the types of the registry values.
    /// </summary>
    private const string _DatabaseValueType = "type";

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new <see cref="RegistryDatabase"/> using the <paramref name="connectionString"/> provided.
    /// </summary>
    /// <exception cref="ArgumentException">An <see cref="ArgumentException"/> is thrown if the <paramref name="connectionString"/> is invalid.</exception>
    /// <param name="connectionString">The <see cref="string"/> to use for connecting to the underlying database.</param>
    public RegistryDatabase(string connectionString)
      : base(connectionString)
    {

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a <see cref="FileSystemDatabase"/> using the default connectionstring.
    /// </summary>
    /// <remarks>
    /// <see cref="Initialize"/> must be called on the returned instance before being able to use it,
    /// just as with any other instance of <see cref="RegistryDatabase"/>.
    /// </remarks>
    /// <param name="filename">The path of the file to use as data source.</param>
    /// <returns></returns>
    public static RegistryDatabase CreateDefaultDatabase(string filename)
    {
      return new RegistryDatabase(string.Format(DefaultConnectionStringFormat, filename));
    }

    /// <summary>
    /// Initializes the database.
    /// Must be called before being able to use any other functionality.
    /// </summary>
    /// <exception cref="DatabaseException">
    /// A <see cref="DatabaseException"/> is thrown if the connectionstring is invalid.
    /// -0R-
    /// A <see cref="DatabaseException"/> is thrown if initialization failed.
    /// </exception>
    public override void Initialize()
    {
      var index = _connectionString.IndexOf("data source=");
      if (index == -1)
        throw new DatabaseException("The database's connection string is invalid.");
      // The string "data source=" contains 12 characters
      var filename = _connectionString.Substring(index + 12, _connectionString.IndexOf(';') - 12);
      if (!File.Exists(filename))
        File.Create(filename).Close();
      var creationQuery = string.Format("CREATE TABLE {0} ({1} INTEGER PRIMARY KEY, {2} TEXT);",
                                        _DatabaseKeyTable, _DatabaseKeyHandle, _DatabaseKeyName);
      if (!TableExists(_DatabaseKeyTable, creationQuery))
        throw new DatabaseException("Unable to create table\"" + _DatabaseKeyTable
                                    + "\" with the following query: " + creationQuery);
      creationQuery = string.Format("CREATE TABLE {0} ({1} TEXT, {2} INTEGER, {3} BLOB, {4} TEXT);",
                                    _DatabaseValueTable, _DatabaseValueName, _DatabaseValueKey, _DatabaseValueValue,
                                    _DatabaseValueType);
      if (!TableExists(_DatabaseValueTable, creationQuery))
        throw new DatabaseException("Unable to create table\"" + _DatabaseValueTable
                                    + "\" with the following query: " + creationQuery);
    }

    /// <summary>
    /// Reads the complete database to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <exception cref="DatabaseException">
    /// A <see cref="DatabaseException"/> is thrown if the databasefile can't be read.
    /// This might be caused because <see cref="Initialize"/> is not called before this call.
    /// </exception>
    /// <returns></returns>
    public override IEnumerable<VirtualRegistryKey> ReadAll()
    {
      // Declare the constant parameters for the Read() method.
      var keyTables = new[] {_DatabaseKeyTable};
      var keyColumns = new[] {_DatabaseKeyHandle, _DatabaseKeyName};
      var valueTables = new[] {_DatabaseValueTable};
      var valueColumns = new[] {_DatabaseValueKey, _DatabaseValueName, _DatabaseValueValue, _DatabaseValueType};
      // Read all known keys.
      IEnumerable<VirtualRegistryKey> keys;
      try
      {
        keys = Read<VirtualRegistryKey>(keyTables, keyColumns, BuildKeyFromReadAllQuery);
      }
      catch (SQLiteException e)
      {
        throw new DatabaseException("An exception occured while reading all entries from the database.", e);
      }
      var resultKeys = new List<VirtualRegistryKey>();
      foreach (var key in keys)
      {
        // Declare the condinationals for the values associated to the current key, and read those values.
        var valueConditionals = new[] {new KeyValuePair<object, object>(_DatabaseValueKey, key.Handle)};
        IEnumerable<VirtualRegistryValue> values;
        try
        {
          values = Read<VirtualRegistryValue>(valueTables, valueColumns, valueConditionals,
                                              BuildValueFromReadAllQuery);
        }
        catch (SQLiteException e)
        {
          throw new DatabaseException("An exception occured while reading all entries from the database.", e);
        }
        foreach (var value in values)
          key.Values.Add(value.Name, value);
        resultKeys.Add(key);
      }
      return resultKeys;
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="RegistryDatabase"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "[RegistryDatabase] " + _connectionString;
    }

    #endregion

    #region Protected Methods

    protected override bool ItemExists(VirtualRegistryKey item)
    {
      var items
        = new List<VirtualRegistryKey>(
          Read<VirtualRegistryKey>(new[] {_DatabaseKeyTable},
                                   new[] {_DatabaseKeyHandle, _DatabaseKeyName},
                                   new[] {new KeyValuePair<object, object>(_DatabaseKeyHandle, item.Handle)},
                                   BuildKeyFromReadAllQuery));
      return items.Count != 0;
    }

    protected override void AppendDeleteQuery(SQLiteCommand command, ParameterGenerator seed, VirtualRegistryKey item)
    {
      var param = seed.Next();
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
      /// Delete all values. It's too complicated to use an update query.
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
      return new VirtualRegistryKey((uint) dataRecord.GetInt64(0), dataRecord.GetString(1));
    }

    private static VirtualRegistryValue BuildValueFromReadAllQuery(IDataRecord dataRecord)
    {
      ValueType valueType;
      ParserHelper.TryParseEnum(dataRecord.GetString(3), out valueType);
      var data = dataRecord.GetValue(2);
      return new VirtualRegistryValue(dataRecord.GetString(1), data != null ? (byte[])data : null, valueType);
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
        command.Parameters.AddWithValue(paramType, Enum.GetName(typeof(ValueType), value.Type));
      }
    }

    #endregion

  }
}
