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
using System.Text;
using System.Threading;

namespace AppStract.Core.Data.FileSystem
{
  public class FileSystemDatabase
  {

    #region Constants

    private const string _DatabaseFileTable = "filetable";
    private const string _DatabaseFileTableKey = "key";
    private const string _DatabaseFileTableValue = "value";
    private const string _DatabaseFileTableKeyParameter = "*key";
    private const string _DatabaseFileTableValueParameter = "*value";

    #endregion

    #region Variables

    private readonly string _connectionString;
    private readonly ReaderWriterLockSlim _sqliteLock;

    #endregion

    #region Constructors

    public FileSystemDatabase(string connectionString)
    {
      _connectionString = connectionString;
      _sqliteLock = new ReaderWriterLockSlim();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Reads the full database.
    /// </summary>
    /// <returns></returns>
    public IDictionary<string, string> ReadAll()
    {
      Dictionary<string, string> result = new Dictionary<string, string>();
      try
      {
        _sqliteLock.EnterReadLock();
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
          SQLiteCommand command = new SQLiteCommand("SELECT " + _DatabaseFileTableKey + ", " + _DatabaseFileTableValue + " FROM " + _DatabaseFileTable);
          command.Connection = connection;
          connection.Open();
          SQLiteDataReader reader = command.ExecuteReader();
          while (reader.Read())
            result.Add(reader.GetString(0), reader.GetString(1));
        }
      }
      finally
      {
        _sqliteLock.ExitReadLock();
      }
      return result;
    }

    /// <summary>
    /// Reads a single entry.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string ReadEntry(string key)
    {
      try
      {
        _sqliteLock.EnterReadLock();
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
          SQLiteCommand command = BuildQueryCommand(key);
          connection.Open();
          SQLiteDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult);
          return reader.Read()
                   ? reader.GetString(0)
                   : null;
        }
      }
      catch
      {
        return null;
      }
      finally
      {
        _sqliteLock.ExitReadLock();
      }
    }

    /// <summary>
    /// Deletes a single entry.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool DeleteEntry(string key)
    {
      _sqliteLock.EnterWriteLock();
      try
      {
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
          SQLiteCommand command = BuildDeleteCommand(key);
          command.Connection = connection;
          connection.Open();
          return command.ExecuteNonQuery() != 0;
        }
      }
      finally
      {
        _sqliteLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Updates or inserts a single entry.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool UpdateEntry(string key, string value)
    {
      string entry = ReadEntry(key);
      try
      {
        _sqliteLock.EnterWriteLock();
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
          SQLiteCommand command = entry != null
                                    ? BuildUpdateCommand(key, value)
                                    : BuildInsertCommand(key, value);
          connection.Open();
          return command.ExecuteNonQuery() != 0;
        }
      }
      catch
      {
        return false;
      }
      finally
      {
        _sqliteLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Reads multiple entries at once.
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
    public string[] BatchRead(string[] keys)
    {
      List<string> result = new List<string>(keys.Length);
      try
      {
        _sqliteLock.EnterReadLock();
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
          connection.Open();
          SQLiteCommand command = BuildQueryCommand(keys);
          command.Connection = connection;
          SQLiteDataReader reader = command.ExecuteReader();
          while (reader.Read())
            result.Add(reader.GetString(0));
        }
      }
      finally
      {
        _sqliteLock.ExitReadLock();
      }
      return result.ToArray();
    }

    /// <summary>
    /// Updates or inserts multiple <see cref="KeyValuePair{TKey,TValue}"/>s.
    /// </summary>
    /// <remarks>
    /// The items are updated or inserted one by one because we have to check the existance of each item
    /// before being able to build a query.
    /// </remarks>
    /// <param name="keysValues"></param>
    /// <returns></returns>
    public bool[] BatchUpdate(KeyValuePair<string, string>[] keysValues)
    {
      SQLiteConnection connection = null;
      List<bool> result = new List<bool>(keysValues.Length);
      try
      {
        _sqliteLock.EnterWriteLock();
        foreach (KeyValuePair<string, string> keyValue in keysValues)
        {
          try
          {
            if (connection == null)
            {
              connection = new SQLiteConnection(_connectionString);
              connection.Open();
            }
            SQLiteCommand command = BuildQueryCommand(keyValue.Key);
            command.Connection = connection;
            command = command.ExecuteReader().Read()
                        ? BuildUpdateCommand(keyValue.Key, keyValue.Value)
                        : BuildInsertCommand(keyValue.Key, keyValue.Value);
            command.Connection = connection;
            result.Add(command.ExecuteScalar() != null);
          }
          catch
          {
            if (connection != null)
              connection.Dispose();
            connection = null;
            result.Add(false);
          }
        }
      }
      finally
      {
        if (connection != null)
          connection.Dispose();
        _sqliteLock.ExitWriteLock();
      }
      return result.ToArray();
    }

    #endregion

    #region Private Methods

    private static SQLiteCommand BuildDeleteCommand(string key)
    {
      SQLiteCommand command = new SQLiteCommand(
        string.Format("DELETE FROM {0} WHERE {1} = {2}",
                      _DatabaseFileTable,
                      _DatabaseFileTableKey, _DatabaseFileTableKeyParameter));
      command.Parameters.AddWithValue(_DatabaseFileTableKeyParameter, key);
      return command;
    }

    private static SQLiteCommand BuildQueryCommand(string key)
    {
      SQLiteCommand command = new SQLiteCommand(
        string.Format("SELECT {0} FROM {1} WHERE {2} = {3}",
                      _DatabaseFileTableValue,
                      _DatabaseFileTable,
                      _DatabaseFileTableKey, _DatabaseFileTableKeyParameter));
      command.Parameters.AddWithValue(_DatabaseFileTableKeyParameter, key);
      return command;
    }

    private static SQLiteCommand BuildQueryCommand(string[] keys)
    {
      if (keys == null)
        throw new ArgumentNullException("keys");
      if (keys.Length == 0)
        throw new ArgumentException("The keys parameter must contain at least 1 item.", "keys");
      SQLiteCommand command = new SQLiteCommand();
      StringBuilder commandText = new StringBuilder();
      commandText.AppendFormat("SELECT {0} FROM {1} WHERE ", _DatabaseFileTableValue, _DatabaseFileTable);
      for (int i = 0; i < keys.Length; i++)
      {
        commandText.AppendFormat("{0} = {1} OR ", _DatabaseFileTableKey, _DatabaseFileTableValueParameter + i);
        command.Parameters.AddWithValue(_DatabaseFileTableValueParameter + i, keys[i]);
      }
      commandText.Remove(commandText.Length - 4, 4);
      command.CommandText = commandText.ToString();
      return command;
    }

    private static SQLiteCommand BuildInsertCommand(string key, string value)
    {
      SQLiteCommand command = new SQLiteCommand(
        string.Format("INSERT INTO [{0}] ({1}, {2}) VALUES ({3}, {4})",
                      _DatabaseFileTable, _DatabaseFileTableKey, _DatabaseFileTableValue,
                      _DatabaseFileTableKeyParameter, _DatabaseFileTableValueParameter));
      command.Parameters.AddWithValue(_DatabaseFileTableKeyParameter, key);
      command.Parameters.AddWithValue(_DatabaseFileTableValueParameter, value);
      return command;
    }

    private static SQLiteCommand BuildUpdateCommand(string key, string value)
    {
      SQLiteCommand command = new SQLiteCommand(
        string.Format("UPDATE {0} SET {1} = {2} WHERE {3} = {4})",
                      _DatabaseFileTable,
                      _DatabaseFileTableValue, _DatabaseFileTableValueParameter,
                      _DatabaseFileTableKey, _DatabaseFileTableKeyParameter));
      command.Parameters.AddWithValue(_DatabaseFileTableKeyParameter, key);
      command.Parameters.AddWithValue(_DatabaseFileTableValueParameter, value);
      return command;
    }

    #endregion

  }
}