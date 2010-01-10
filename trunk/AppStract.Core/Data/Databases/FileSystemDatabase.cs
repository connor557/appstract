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
using AppStract.Core.Virtualization.FileSystem;

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
    private const string _DatabaseFileTable = "filetable";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseFileTable"/> holding all the keys.
    /// </summary>
    private const string _DatabaseFileTableKey = "key";
    /// <summary>
    /// The name of the column in <see cref="_DatabaseFileTable"/> holding all the values.
    /// </summary>
    private const string _DatabaseFileTableValue = "value";

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
      var connectionstring =
        string.Format("Data Source={0};Version=3;PRAGMA synchronous=OFF;FailIfMissing=True;Journal Mode=Off;",
                      filename);
      return new FileSystemDatabase(connectionstring);
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
      var creationQuery = string.Format("CREATE TABLE {0} ({1} TEXT, {2} TEXT);",
                                        _DatabaseFileTable, _DatabaseFileTableKey, _DatabaseFileTableValue);
      if (!TableExists(_DatabaseFileTable, creationQuery))
        throw new DatabaseException("Unable to create table\"" + _DatabaseFileTable
                                    + "\" with the following query: " + creationQuery);
    }

    /// <summary>
    /// Reads the complete database to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<FileTableEntry> ReadAll()
    {
      return Read<FileTableEntry>(new[] {_DatabaseFileTable},
                                     new[] {_DatabaseFileTableKey, _DatabaseFileTableValue},
                                     BuildItemFromReadAllQuery);
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
      command.CommandText += string.Format("INSERT INTO [{0}] ({1}, {2}) VALUES ({3}, {4});",
                                           _DatabaseFileTable, _DatabaseFileTableKey, _DatabaseFileTableValue,
                                           paramKey, paramValue);
      command.Parameters.AddWithValue(paramKey, item.Key);
      command.Parameters.AddWithValue(paramValue, item.Value);
    }

    protected override void AppendUpdateQuery(SQLiteCommand command, ParameterGenerator seed, FileTableEntry item)
    {
      var paramKey = seed.Next();
      var paramValue = seed.Next();
      command.CommandText += string.Format("UPDATE {0} SET {1} = {2} WHERE \"{3}\" = {4};",
                                           _DatabaseFileTable,
                                           _DatabaseFileTableValue, paramValue,
                                           _DatabaseFileTableKey, paramKey);
      command.Parameters.AddWithValue(paramKey, item.Key);
      command.Parameters.AddWithValue(paramValue, item.Value);
    }

    #endregion

    #region Private Methods

    private static FileTableEntry BuildItemFromReadAllQuery(IDataRecord dataRecord)
    {
      return new FileTableEntry(dataRecord.GetString(0), dataRecord.GetString(1), FileKind.Unspecified);
    }

    #endregion

  }
}