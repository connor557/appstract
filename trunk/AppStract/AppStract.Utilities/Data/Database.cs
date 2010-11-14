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
using System.Text;
using System.Threading;
using AppStract.Utilities.Logging;
using AppStract.Utilities.Observables;

namespace AppStract.Utilities.Data
{
  /// <summary>
  /// Base class for database interfaces using SQLite.
  /// </summary>
  /// <typeparam name="T">The type of objects stored and accessed by the current <see cref="Database{T}"/>.</typeparam>
  public abstract class Database<T> : IDisposable
  {

    #region Constants

    /// <summary>
    /// A format string for the default connectionstring.
    /// Use like: <code>string.Format(DefaultConnectionStringFormat, myDataSourceVariable)</code>
    /// </summary>
    protected const string _DefaultConnectionStringFormat = "Data Source={0};Version=3;PRAGMA synchronous=OFF;FailIfMissing=True;Journal Mode=Off;";

    #endregion

    #region Variables

    /// <summary>
    /// The connectionstring used to connect to the SQLite database.
    /// Contains only lower cased characters.
    /// </summary>
    protected readonly string _connectionString;
    /// <summary>
    /// Queue for the <see cref="DatabaseAction{T}"/>s waiting to be commited.
    /// </summary>
    private readonly ObservableQueue<DatabaseAction<T>> _actionQueue;
    /// <summary>
    /// Object to acquire a write-lock on when flushing <see cref="_actionQueue"/>.
    /// </summary>
    private readonly ReaderWriterLockSlim _flushLock;
    /// <summary>
    /// Lock to use while a connection to the database is open and used.
    /// </summary>
    private readonly ReaderWriterLockSlim _sqliteLock;

    #endregion

    #region Events

    /// <summary>
    /// Occurs when a new item is added to the queue.
    /// </summary>
    /// <remarks>
    /// This event is synchronously called and is not thread safe.
    /// Inheritors should never unsubscribe, in order to avoid exceptions while raising the event.
    /// Inheritors should also process the event async if processing takes a long time.
    /// If an inheritor exposes this event, the inheritor should implement it's own functionality to make it thread safe.
    /// </remarks>
    protected event EventHandler<DatabaseAction<T>> ItemEnqueued;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the connection string used by the current instance.
    /// </summary>
    public string ConnectionString
    {
      get { return _connectionString; }
    }

    /// <summary>
    /// Gets the log service.
    /// </summary>
    protected Logger Log
    {
      get; private set;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="Database{T}"/>.
    /// </summary>
    /// <exception cref="ArgumentException">An <see cref="ArgumentException"/> is thrown if the <paramref name="connectionString"/> is invalid.</exception>
    /// <param name="connectionString">The connectionstring to use to connect to the SQLite database.</param>
    protected Database(string connectionString)
    {
      _connectionString = connectionString.ToLowerInvariant();
      // Do a basic check on the connectionstring.
      if (!_connectionString.Contains("data source="))
        throw new ArgumentException("The connectionstring must at least specify a data source.");
      _sqliteLock = new ReaderWriterLockSlim();
      _flushLock = new ReaderWriterLockSlim();
      _actionQueue = new ObservableQueue<DatabaseAction<T>>();
      _actionQueue.ItemEnqueued += OnActionEnqueued;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Initializes the database.
    /// Must be called before being able to use any other functionality.
    /// </summary>
    /// <exception cref="DatabaseException">
    /// A <see cref="DatabaseException"/> is thrown if the connectionstring is invalid.
    /// -0R-
    /// A <see cref="DatabaseException"/> is thrown if initialization failed.
    /// </exception>
    public void Initialize()
    {
      Log = DoInitialize() ?? new NullLogger();
    }

    /// <summary>
    /// Reads the complete database to an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<T> ReadAll();

    /// <summary>
    /// Enqueues an action to be committed to the database.
    /// </summary>
    /// <param name="databaseAction"></param>
    public void EnqueueAction(DatabaseAction<T> databaseAction)
    {
      // Don't acquire a write lock, this class can handle the enqueueing of new items while flushing.
      // A lock is only necessary when dequeueing items.
      _actionQueue.Enqueue(databaseAction);
    }

    /// <summary>
    /// Enqueues multiple actions to be committed to the database.
    /// </summary>
    /// <param name="databaseActions"></param>
    public void EnqueueAction(IEnumerable<DatabaseAction<T>> databaseActions)
    {
      // Don't acquire a write lock, this class can handle the enqueueing of new items while flushing.
      // A lock is only necessary when dequeueing items.
      _actionQueue.Enqueue(databaseActions, false);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Initializes the database.
    /// If the SQLite file doesn't exist yet, it is created.
    /// If the tables don't exist yet, they are created.
    /// </summary>
    /// <returns>
    /// The log service to use for writing messages.
    /// </returns>
    protected abstract Logger DoInitialize();

    /// <summary>
    /// Returns whether the table with the specified <paramref name="tableName"/> exists.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// An <see cref="ArgumentNullException"/> is thrown if <paramref name="tableName"/> is null.
    /// </exception>
    /// <param name="tableName">Table to verify.</param>
    protected bool TableExists(string tableName)
    {
      return TableExists(tableName, null);
    }

    /// <summary>
    /// Returns whether the table with the specified <paramref name="tableName"/> exists.
    /// If the table does not exist and <paramref name="creationQuery"/> is not null, the table is created.
    /// </summary>
    /// <exception cref="ArgumentNullException">
    /// An <see cref="ArgumentNullException"/> is thrown if <paramref name="tableName"/> is null.
    /// </exception>
    /// <param name="tableName">Table to verify.</param>
    /// <param name="creationQuery">
    /// Query to use if the table needs to be created.
    /// If this parameter is null, the method behaves like the <see cref="TableExists(string)"/> method.
    /// </param>
    protected bool TableExists(string tableName, string creationQuery)
    {
      //System.Diagnostics.Debugger.Break();
      if (tableName == null)
        throw new ArgumentNullException("tableName");
      // Determine if an UpgradeableReadLock must be entered and exited by the current method call
      var requireLock = !_sqliteLock.IsUpgradeableReadLockHeld;
      try
      {
        if (requireLock)
          _sqliteLock.EnterUpgradeableReadLock();
        using (var connection = new SQLiteConnection(_connectionString))
        {
          var command = new SQLiteCommand("SELECT * FROM \"" + tableName + "\" LIMIT 1", connection);
          command.Connection.Open();
          try
          {
            command.ExecuteReader().Close();
            return true;
          }
          catch (SQLiteException)
          {
            Log.Debug("[Database] Failed to verify existance of table \"{0}\"", tableName);
            // Create the table?
            if (creationQuery != null)
            {
              command = new SQLiteCommand(creationQuery, connection);
              if (ExecuteCommand(command) && TableExists(tableName))
              {
                Log.Debug("[Database] Created table \"{0}\"", tableName);
                return true;
              }
              Log.Error("[Database] Failed to create table \"{0}\" with the following query: {1}",
                                tableName, creationQuery);
            }
            return false;
          }
        }
      }
      finally
      {
        if (requireLock)
          _sqliteLock.ExitUpgradeableReadLock();
      }
    }

    /// <summary>
    /// Reads all values from the specified <paramref name="columns"/> in the specified <paramref name="tables"/>.
    /// The items are build by calling <paramref name="itemBuilder"/>.
    /// </summary>
    /// <exception cref="ArgumentException">An <see cref="ArgumentException"/> is thrown when there are no tables defined.</exception>
    /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if one of the parameters is null.</exception>
    /// <param name="tables">The tables in the database to read from.</param>
    /// <param name="columns">The columns to read all values from.</param>
    /// <param name="itemBuilder">
    /// The method to use when building the item. The <see cref="IDataRecord"/>'s fields
    /// are provided in the same order as the column-definitions in the <paramref name="columns"/> parameter.
    /// </param>
    /// <returns>A list of all items.</returns>
    protected IList<TItemType> Read<TItemType>(IEnumerable<string> tables, IEnumerable<string> columns,
      BuildItemFromQueryData<TItemType> itemBuilder)
    {
      // The list of read values,
      // don't use a 'yield return' statement because the used resources must be freed as soon as possible.
      // These resources include the lock on _sqliteLock and the open SQLiteConnection.
      var entries = new List<TItemType>();
      try
      {
        _sqliteLock.EnterReadLock();
        using (var connection = new SQLiteConnection(_connectionString))
        {
          var query = BuildSelectQuery(tables, columns).ToString();
          using (var command = new SQLiteCommand(query.EndsWith(";") ? query : query + ";", connection))
          {
            command.Connection.Open();
            using (var reader = command.ExecuteReader())
            {
              while (reader.Read())
                entries.Add(itemBuilder(reader));
            }
          }
        }
      }
      finally
      {
        _sqliteLock.ExitReadLock();
      }
      return entries;
    }

    /// <summary>
    /// Reads all values from the specified <paramref name="columns"/> in the specified <paramref name="tables"/>.
    /// The items are build by calling <paramref name="itemBuilder"/>.
    /// </summary>
    /// <exception cref="ArgumentException">An <see cref="ArgumentException"/> is thrown when there are no tables defined.</exception>
    /// <exception cref="ArgumentNullException">An <see cref="ArgumentNullException"/> is thrown if one of the parameters is null.</exception>
    /// <param name="tables">The tables in the database to read from.</param>
    /// <param name="columns">The columns to read all values from.</param>
    /// <param name="conditionals">The elements for the where keyword.</param>
    /// <param name="itemBuilder">
    /// The method to use when building the item. The <see cref="IDataRecord"/>'s fields
    /// are in the same order as the columns in the <paramref name="columns"/> parameter.
    /// </param>
    /// <returns>A list of all items.</returns>
    protected IEnumerable<TItemType> Read<TItemType>(IEnumerable<string> tables, IEnumerable<string> columns,
      IEnumerable<KeyValuePair<object, object>> conditionals, BuildItemFromQueryData<TItemType> itemBuilder)
    {
      // The list of read values,
      // don't use a 'yield return' statement because the used resources must be freed as soon as possible.
      // These resources include the lock on _sqliteLock and the open SQLiteConnection.
      var entries = new List<TItemType>();
      try
      {
        _sqliteLock.EnterReadLock();
        using (var connection = new SQLiteConnection(_connectionString))
        {
          var query = BuildSelectQuery(tables, columns, conditionals).ToString();
          using (var command = new SQLiteCommand(query.EndsWith(";") ? query : query + ";", connection))
          {
            command.Connection.Open();
            using (var reader = command.ExecuteReader())
            {
              while (reader.Read())
                entries.Add(itemBuilder(reader));
            }
          }
        }
      }
      finally
      {
        _sqliteLock.ExitReadLock();
      }
      return entries;
    }

    /// <summary>
    /// Returns whether the specified <paramref name="item"/> exists in the database.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract bool ItemExists(T item);

    /// <summary>
    /// Appends a delete-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command">The command to append the query to.</param>
    /// <param name="seed">The object to use for the generation of unique names.</param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendDeleteQuery(SQLiteCommand command, ParameterGenerator seed, T item);

    /// <summary>
    /// Appends an insert-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="seed"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendInsertQuery(SQLiteCommand command, ParameterGenerator seed, T item);

    /// <summary>
    /// Appends an update-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="seed"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendUpdateQuery(SQLiteCommand command, ParameterGenerator seed, T item);

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

    #endregion

    #region Private Methods

    /// <summary>
    /// Handles newly enqueued <see cref="DatabaseAction{T}"/>s.
    /// </summary>
    private void OnActionEnqueued(object sender, QueueChangedEventArgs<DatabaseAction<T>> e)
    {
      if (ItemEnqueued != null)
        ItemEnqueued(this, e.Data);
      try
      {
        Flush(false);
      }
      catch (DatabaseException ex)
      {
        Log.Error("[Database] Failed to flush to database [" + _connectionString + "]", ex);
      }
    }

    /// <summary>
    /// Flushes all queued <see cref="DatabaseAction{TItem}"/>s to the physical database.
    /// It's not guaranteed that it's the current thread that will perform the flushing.
    /// </summary>
    /// <exception cref="DatabaseException">
    /// An <see cref="DatabaseException"/> is thrown if flushing failes.
    /// </exception>
    /// <exception cref="TimeoutException">
    /// A <see cref="TimeoutException"/> is thrown if <paramref name="awaitRunningSequence"/> is set to true
    /// and if the current flush sequence can't start within 2000ms.
    /// </exception>
    /// <param name="awaitRunningSequence">
    /// Specifies, in case a flush sequence is already running, whether or not this method needs to wait before returning until the other sequence ended.
    /// If waiting takes more then 2000ms, a <see cref="TimeoutException"/> is thrown.
    /// </param>
    private void Flush(bool awaitRunningSequence)
    {
      try
      {
        if (!_flushLock.TryEnterWriteLock(2))
        {
          // Already flushing, wait for flushing to end before return statement?
          if (awaitRunningSequence)
          {
            if (!_flushLock.TryEnterWriteLock(2000))
              throw new TimeoutException("Failed to wait for database-flushing to end.");
            _flushLock.ExitWriteLock();
          }
          return;
        }
        var connection = new SQLiteConnection(_connectionString);
        var command = new SQLiteCommand(connection);
        try
        {
          // The 'items' array is used to cache all dequeued items
          // untill it's made sure that they are written to the database.
          var items = new List<DatabaseAction<T>>(_actionQueue.Count);
          try
          {
            var seed = new ParameterGenerator();
            while (_actionQueue.Count > 0)
            {
              var item = _actionQueue.Dequeue();
              items.Add(item);
              switch (item.ActionType)
              {
                case DatabaseActionType.Set:
                  if (!ItemExists(item.Item))
                    AppendInsertQuery(command, seed, item.Item);
                  else
                    AppendUpdateQuery(command, seed, item.Item);
                  break;
                case DatabaseActionType.Remove:
                  AppendDeleteQuery(command, seed, item.Item);
                  break;
                default:
                  throw new NotImplementedException(
                    "DatabaseActionType is changed without adding an extra handler to the Database.Flush() method.");
              }
            }
          }
          finally
          {
            _flushLock.ExitWriteLock();
          }
          // Execute the command as a single transaction to improve speed.
          command.CommandText = "BEGIN;" + command.CommandText + "COMMIT;";
          if (!ExecuteCommand(command))
          {
            // Failed to write items to database, enqueue them again.
            _actionQueue.Enqueue(items, false);
            throw new DatabaseException("Failed to flush the database.");
          }
        }
        finally
        {
          command.Dispose();
          connection.Dispose();
        }
      }
      finally
      {
        // This finally clause exists to make sure _flushLock is released in case of for example a ThreadAbortException
        if (_flushLock.IsWriteLockHeld)
          _flushLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Executes the given <paramref name="command"/> as a NonQuery.
    /// This method acquires a writelock on <see cref="_sqliteLock"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
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
      catch (Exception e)
      {
        Log.Debug("Failed to execute an SQL statement against the database.", e);
        return false;
      }
      finally
      {
        _sqliteLock.ExitWriteLock();
      }
    }

    /// <summary>
    /// Returns a query which selects the specified <paramref name="fields"/> from the specified <paramref name="tables"/>.
    /// </summary>
    /// <param name="tables">The tables to select fields from.</param>
    /// <param name="fields">The fields to select from the tables. If no fields are specified, a wildcard is used for the select query.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if <paramref name="tables"/> defines no values.
    /// </exception>
    /// <returns></returns>
    private static StringBuilder BuildSelectQuery(IEnumerable<string> tables, IEnumerable<string> fields)
    {
      if (tables == null)
        throw new ArgumentNullException("tables");
      if (fields == null)
        throw new ArgumentNullException("fields");
      var queryString = new StringBuilder("SELECT ");
      // Add columns to the query.
      foreach (var field in fields)
        queryString.Append(field + ", ");
      if (queryString.ToString().EndsWith(", "))
        queryString.Remove(queryString.Length - 2, 2);
      else
        // No columns to select? Select everything.
        queryString.Append("*");
      queryString.Append(" FROM ");
      // Add tables to the query.
      foreach (string table in tables)
        queryString.Append(table + ", ");
      if (queryString.ToString().EndsWith(", "))
        queryString.Remove(queryString.Length - 2, 2);
      else
        // No tables? Can't build a query without tables.
        throw new ArgumentException("Unable to build the query because there are no tables defined.", "tables");
      return queryString;
    }

    /// <summary>
    /// Returns a query which selects the specified <paramref name="fields"/> from the specified <paramref name="tables"/>.
    /// </summary>
    /// <param name="tables">The tables to select fields from.</param>
    /// <param name="fields">The fields to select from the tables. If no fields are specified, a wildcard is used for the select query.</param>
    /// <param name="conditions">The conditions to select on using the WHERE keyword.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException">
    /// An <see cref="ArgumentException"/> is thrown if <paramref name="tables"/> defines no values.
    /// </exception>
    /// <returns></returns>
    private static StringBuilder BuildSelectQuery(IEnumerable<string> tables, IEnumerable<string> fields, IEnumerable<KeyValuePair<object, object>> conditions)
    {
      if (conditions == null)
        throw new ArgumentNullException("conditions");
      var queryString = BuildSelectQuery(tables, fields);
      if (queryString[queryString.Length - 1] == ';')
        queryString.Remove(queryString.Length - 1, 1);
      queryString.Append(" WHERE ");
      // Add the conditions.
      foreach (var condition in conditions)
        queryString.Append(condition.Key + " = \"" + condition.Value + "\" AND ");
      if (queryString.ToString().EndsWith(" AND "))
        queryString.Remove(queryString.Length - 5, 5);
      else
        // No conditions? Remove the "WHERE" keyword.
        queryString.Remove(queryString.Length - 7, 7);
      return queryString;
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      try
      {
        Flush(true);
      }
      catch (Exception e)
      {
        Log.Error("Failed to flush to database during Dispose", e);
      }
    }

    #endregion

  }
}
