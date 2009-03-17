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
using AppStract.Core;
using AppStract.Core.Logging;
using AppStract.Utilities.Observables;

namespace AppStract.Core.Data
{
  public abstract class Database<TItem>
  {

    #region Variables

    protected readonly string _connectionString;
    private readonly ObservableQueue<DatabaseAction<TItem>> _actionQueue;
    private readonly ReaderWriterLockSlim _actionQueueLock;
    private readonly ReaderWriterLockSlim _sqliteLock;

    #endregion

    #region Constructors

    protected Database(string connectionString)
    {
      _connectionString = connectionString;
      _sqliteLock = new ReaderWriterLockSlim();
      _actionQueueLock = new ReaderWriterLockSlim();
      _actionQueue = new ObservableQueue<DatabaseAction<TItem>>();
      _actionQueue.ItemEnqueued += ItemEnqueued;
    }

    #endregion

    #region Public Methods

    public abstract IEnumerable<TItem> ReadAll();

    public void EnqueueAction(DatabaseAction<TItem> databaseAction)
    {
      /// Don't acquire a write lock, this class can handle the enqueueing of new items while flushing.
      /// A lock is only necessary when dequeueing items.
      _actionQueue.Enqueue(databaseAction);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Reads all values from the specified <paramref name="columns"/> in the specified <paramref name="tables"/>.
    /// The items are build by calling <paramref name="itemBuilder"/>.
    /// </summary>
    /// <param name="tables">The tables in the database to read from.</param>
    /// <param name="columns">The columns to read all values from.</param>
    /// <param name="itemBuilder">
    /// The method to use when building the item. The <see cref="IDataRecord"/>'s fields
    /// are in the same order as the columns in the <paramref name="columns"/> parameter.
    /// </param>
    /// <returns>A list of all items.</returns>
    protected IList<TItem> ReadAll(IEnumerable<string> tables, IEnumerable<string> columns, BuildItemFromQuery<TItem> itemBuilder)
    {
      List<TItem> entries = new List<TItem>();
      try
      {
        _sqliteLock.EnterReadLock();
        using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
        {
          StringBuilder cmdBuilder = new StringBuilder("SELECT ");
          foreach (string col in columns)
            cmdBuilder.Append(col + ", ");
          cmdBuilder.Remove(cmdBuilder.Length - 2, 2);
          cmdBuilder.Append(" FROM ");
          foreach (string table in tables)
            cmdBuilder.Append(table + ", ");
          cmdBuilder.Remove(cmdBuilder.Length - 2, 2);
          SQLiteCommand command = new SQLiteCommand(cmdBuilder + ";", connection);
          command.Connection.Open();
          SQLiteDataReader reader = command.ExecuteReader();
          while (reader.Read())
            entries.Add(itemBuilder(reader));
        }
      }
      finally
      {
        _sqliteLock.ExitReadLock();
      }
      return entries;
    }

    /// <summary>
    /// Escapes a minimal set of characters by replacing them with their escape codes. 
    /// This instructs the database engine to interpret these characters literally rather than as metacharacters.
    /// </summary>
    /// <seealso cref="Unescape"/>
    /// <param name="str">The input string containing the text to convert.</param>
    /// <returns></returns>
    protected static string Escape(string str)
    {
      str = str.Replace("\"", "#~&DoUBleQuOte$~#");
      str = str.Replace("'", "#~&SiNgLeQuOte$~#");
      str = str.Replace("`", "#~&SiNgLeQuOteOpEn$~#");
      str = str.Replace("´", "#~&SiNgLeQuOte3ClOSe$~#");
      str = str.Replace("(", "#~&BrACKetOpEn$~#");
      str = str.Replace(")", "#~&BrACKetClOSe$~#");
      str = str.Replace("%", "#~&ProCent$~#");
      return str;
    }

    /// <summary>
    /// Unescapes any escaped characters in the input string.
    /// </summary>
    /// <param name="str">The input string containing the text to unescape.</param>
    /// <returns></returns>
    protected static string Unescape(string str)
    {
      str = str.Replace("#~&DoUBleQuOte$~#", "\"");
      str = str.Replace("#~&SiNgLeQuOte$~#", "'");
      str = str.Replace("#~&SiNgLeQuOteOpEn$~#", "`");
      str = str.Replace("#~&SiNgLeQuOteClOSe$~#", "´");
      str = str.Replace("#~&BrACKetOpEn$~#", "(");
      str = str.Replace("#~&BrACKetClOSe$~#", ")");
      str = str.Replace("#~&ProCent$~#", "%");
      return str;

    }

    /// <summary>
    /// Appends a delete-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command">The command to append the query to.</param>
    /// <param name="seed">The object to use for the generation of unique names.</param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendDeleteCommand(SQLiteCommand command, ParameterGenerator seed, TItem item);

    /// <summary>
    /// Appends an insert-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="seed"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendInsertCommand(SQLiteCommand command, ParameterGenerator seed, TItem item);

    /// <summary>
    /// Appends an update-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="seed"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendUpdateCommand(SQLiteCommand command, ParameterGenerator seed, TItem item);

    #endregion

    #region Private Methods

    private void ItemEnqueued(DatabaseAction<TItem> item)
    {
      Flush();
    }

    private void Flush()
    {
      if (!_actionQueueLock.TryEnterWriteLock(400))
        return;
      SQLiteConnection connection = new SQLiteConnection(_connectionString);
      SQLiteCommand command = new SQLiteCommand(connection);
      try
      {
        try
        { 
          ParameterGenerator seed = new ParameterGenerator();
          while (_actionQueue.Count > 0)
          {
            var item = _actionQueue.Dequeue();
            if (item.ActionType == DatabaseActionType.Update)
              AppendUpdateCommand(command, seed, item.Item);
            else if (item.ActionType == DatabaseActionType.Add)
              AppendInsertCommand(command, seed, item.Item);
            else if (item.ActionType == DatabaseActionType.Remove)
              AppendDeleteCommand(command, seed, item.Item);
            else
              throw new NotImplementedException(
                "DatabaseActionType is changed without adding an extra handler to the Database.FlushQueue() method.");
          }
        }
        finally
        {
          _actionQueueLock.ExitWriteLock();
        }
        if (!ExecuteCommand(command))
          throw new Exception("Failed to flush the database.");
      }
      finally
      {
        command.Dispose();
        connection.Dispose();
      }
    }

    private bool ExecuteCommand(IDbCommand command)
    {
      _sqliteLock.EnterWriteLock();
      try
      {
        if (command.Connection.State != ConnectionState.Open)
          command.Connection.Open();
        command.ExecuteNonQuery();
        return true;
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

    #endregion

  }
}
