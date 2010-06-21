#region Copyright (C) 2009-2010 Simon Allaeys

/*
    Copyright (C) 2009-2010 Simon Allaeys
 
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
using AppStract.Core.Virtualization.Engine.FileSystem;
using AppStract.Utilities.Helpers;

namespace AppStract.Core.Data.Databases
{
  /// <summary>
  /// Interface class for the file system database.
  /// </summary>
  public class FileSystemDatabase : Database<FileTableEntry>
  {

    #region Constants

    /// <summary>
    /// The name of the table in the database containing all key/value pairs for the Filetable.
    /// </summary>
    protected const string _DatabaseFileTable = "filetable";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseFileTable"/> holding all the keys.
    /// </summary>
    protected const string _DatabaseFileTableKey = "key";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseFileTable"/> holding all the values.
    /// </summary>
    protected const string _DatabaseFileTableValue = "value";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseFileTable"/> holding the types.
    /// </summary>
    protected const string _DatabaseFileTabletype = "type";

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new <see cref="RegistryDatabase"/> using the <paramref name="connectionString"/> provided.
    /// </summary>
    /// <exception cref="ArgumentException">An <see cref="ArgumentException"/> is thrown if the <paramref name="connectionString"/> is invalid.</exception>
    /// <param name="connectionString">The <see cref="string"/> to use for connecting to the underlying database.</param>
    public FileSystemDatabase(string connectionString)
      : base(connectionString)
    {

    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns a <see cref="FileSystemDatabase"/> using the default connectionstring.
    /// </summary>
    /// <remarks>
    /// <see cref="Initialize"/> must be called before being able to use this instance,
    /// just as with any other instance of <see cref="FileSystemDatabase"/>.
    /// </remarks>
    /// <param name="filename">The path of the file to use as data source.</param>
    /// <returns></returns>
    public static FileSystemDatabase CreateDefaultDatabase(string filename)
    {
      return new FileSystemDatabase(string.Format(DefaultConnectionStringFormat, filename));
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
      var filename = _connectionString.Substring(index + 12, _connectionString.IndexOf(';') - 12);
      if (!File.Exists(filename))
        File.Create(filename).Close();
      var creationQuery = string.Format("CREATE TABLE {0} ({1} TEXT, {2} TEXT, {3} VARCHAR(11));",
                                        _DatabaseFileTable, _DatabaseFileTableKey, _DatabaseFileTableValue, _DatabaseFileTabletype);
      if (!TableExists(_DatabaseFileTable, creationQuery))
        throw new DatabaseException("Unable to create table\"" + _DatabaseFileTable
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
    public override IEnumerable<FileTableEntry> ReadAll()
    {
      try
      {
        return Read<FileTableEntry>(new[] {_DatabaseFileTable},
                                    new[] {_DatabaseFileTableKey, _DatabaseFileTableValue, _DatabaseFileTabletype},
                                    BuildItemFromReadAllQuery);
      }
      catch (SQLiteException e)
      {
        throw new DatabaseException("An exception occured while reading all entries from the database.", e);
      }
    }

    /// <summary>
    /// Returns a string representation of the current <see cref="FileSystemDatabase"/>.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return "[FileSystemDatabase] " + _connectionString;
    }

    #endregion

    #region Protected Methods

    protected override bool ItemExists(FileTableEntry item)
    {
      var items
        = new List<FileTableEntry>(
          Read<FileTableEntry>(new[] {_DatabaseFileTable},
                               new[] {_DatabaseFileTableKey, _DatabaseFileTableValue, _DatabaseFileTabletype},
                               new[] {new KeyValuePair<object, object>(_DatabaseFileTableKey, item.Key)},
                               BuildItemFromReadAllQuery));
      return items.Count != 0;
    }

    protected override void AppendDeleteQuery(SQLiteCommand command, ParameterGenerator seed, FileTableEntry item)
    {
      var paramKey = seed.Next();
      command.CommandText += string.Format("DELETE FROM {0} WHERE {1} = {2};",
                                           _DatabaseFileTable, _DatabaseFileTableKey, paramKey);
      command.Parameters.AddWithValue(paramKey, item.Key);
    }

    protected override void AppendInsertQuery(SQLiteCommand command, ParameterGenerator seed, FileTableEntry item)
    {
      var paramKey = seed.Next();
      var paramValue = seed.Next();
      var paramType = seed.Next();
      command.CommandText += string.Format("INSERT INTO [{0}] ({1}, {2}, {3}) VALUES ({4}, {5}, {6});",
                                           _DatabaseFileTable,
                                           _DatabaseFileTableKey, _DatabaseFileTableValue, _DatabaseFileTabletype,
                                           paramKey, paramValue, paramType);
      command.Parameters.AddWithValue(paramKey, item.Key);
      command.Parameters.AddWithValue(paramValue, item.Value);
      command.Parameters.AddWithValue(paramType, Enum.GetName(typeof(FileKind), item.FileKind));
    }

    protected override void AppendUpdateQuery(SQLiteCommand command, ParameterGenerator seed, FileTableEntry item)
    {
      var paramKey = seed.Next();
      var paramValue = seed.Next();
      var paramType = seed.Next();
      command.CommandText += string.Format("UPDATE {0} SET {1} = {2}, {3} = {4} WHERE {5} = {6};",
                                           _DatabaseFileTable,
                                           _DatabaseFileTableValue, paramValue,
                                           _DatabaseFileTabletype, paramType,
                                           _DatabaseFileTableKey, paramKey);
      command.Parameters.AddWithValue(paramKey, item.Key);
      command.Parameters.AddWithValue(paramValue, item.Value);
      command.Parameters.AddWithValue(paramType, Enum.GetName(typeof(FileKind), item.FileKind));
    }

    #endregion

    #region Private Methods

    private static FileTableEntry BuildItemFromReadAllQuery(IDataRecord dataRecord)
    {
      FileKind fileKind;
      ParserHelper.TryParseEnum(dataRecord.GetString(2), out fileKind);
      return new FileTableEntry(dataRecord.GetString(0), dataRecord.GetString(1), fileKind);
    }

    #endregion

  }
}