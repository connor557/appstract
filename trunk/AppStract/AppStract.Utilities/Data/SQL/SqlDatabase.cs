using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Threading;

namespace AppStract.Utilities.Data.Sql
{
  public abstract class SqlDatabase<T> : Database<T>
  {

    #region Variables

    /// <summary>
    /// The connectionstring used to connect to the SQLite database.
    /// Contains only lower cased characters.
    /// </summary>
    private readonly string _connectionString;
    /// <summary>
    /// Lock to use while a connection to the database is open and used.
    /// </summary>
    private readonly ReaderWriterLockSlim _sqliteLock;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the connection string used by the current instance.
    /// </summary>
    public string ConnectionString
    {
      get { return _connectionString; }
    }

    #endregion
    
    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="Database{T}"/>.
    /// </summary>
    /// <exception cref="ArgumentException">An <see cref="ArgumentException"/> is thrown if the <paramref name="connectionString"/> is invalid.</exception>
    /// <param name="connectionString">The connectionstring to use to connect to the SQL database.</param>
    protected SqlDatabase(string connectionString)
    {
      if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentNullException("connectionString");
      AssertConnectionString(connectionString);
      _connectionString = connectionString;
      _sqliteLock = new ReaderWriterLockSlim();
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Creates the actual database.
    /// </summary>
    protected abstract void CreateDatabase();

    /// <summary>
    /// Asserts the correctness of the connection string.
    /// </summary>
    /// <remarks>
    /// The method is called from the <see cref="SqlDatabase{T}"/> class' constructor,
    /// therefore the implementation should not depend on any variables set in the derived class' constructor.
    /// </remarks>
    /// <param name="connectionString"></param>
    protected abstract void AssertConnectionString(string connectionString);

    /// <summary>
    /// Creates a command for the given command string.
    /// </summary>
    /// <param name="command">The initial command string.</param>
    /// <returns></returns>
    protected abstract DbCommand CreateCommand(string command);

    /// <summary>
    /// Creates a parameter that can be added to any command returned by <see cref="CreateCommand"/>.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <returns></returns>
    protected abstract IDbDataParameter CreateParameter(string name, object value);

    /// <summary>
    /// Appends a delete-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command">The command to append the query to.</param>
    /// <param name="seed">The object to use for the generation of unique names.</param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendDeleteQuery(IDbCommand command, ParameterGenerator seed, T item);

    /// <summary>
    /// Appends an insert-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="seed"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendInsertQuery(IDbCommand command, ParameterGenerator seed, T item);

    /// <summary>
    /// Appends an update-query for <paramref name="item"/> to <paramref name="command"/>.
    /// The parameters are added to the Parameters of <paramref name="command"/>,
    /// the (unique) naming of these parameters is done by using <paramref name="seed"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="seed"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    protected abstract void AppendUpdateQuery(IDbCommand command, ParameterGenerator seed, T item);

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
        var query = BuildSelectQuery(tables, columns).ToString();
        using (var command = CreateCommand(query.EndsWith(";") ? query : query + ";"))
        {
          command.Connection.Open();
          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
              entries.Add(itemBuilder(reader));
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
        var query = BuildSelectQuery(tables, columns, conditionals).ToString();
        using (var command = CreateCommand(query.EndsWith(";") ? query : query + ";"))
        {
          command.Connection.Open();
          using (var reader = command.ExecuteReader())
          {
            while (reader.Read())
              entries.Add(itemBuilder(reader));
          }
        }
      }
      finally
      {
        _sqliteLock.ExitReadLock();
      }
      return entries;
    }

    protected override void Write(IEnumerator<DatabaseAction<T>> items)
    {
      using (var command = CreateCommand(""))
      {
        var seed = new ParameterGenerator();
        while (items.MoveNext())
        {
          switch (items.Current.ActionType)
          {
            case DatabaseActionType.Set:
              if (!ItemExists(items.Current.Item))
                AppendInsertQuery(command, seed, items.Current.Item);
              else
                AppendUpdateQuery(command, seed, items.Current.Item);
              break;
            case DatabaseActionType.Remove:
              AppendDeleteQuery(command, seed, items.Current.Item);
              break;
            default:
              throw new NotImplementedException(
                "DatabaseActionType is changed without adding an extra handler to the Database.Flush() method.");
          }
        }
        // Execute the command as a single transaction to improve speed.
        command.CommandText = "BEGIN;" + command.CommandText + "COMMIT;";
        if (!ExecuteCommand(command))
        {
          throw new DatabaseException("Failed to flush the database.");
        }
      }
    }

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
      if (tableName == null)
        throw new ArgumentNullException("tableName");
      // Determine if an UpgradeableReadLock must be entered and exited by the current method call
      var requireLock = !_sqliteLock.IsUpgradeableReadLockHeld;
      try
      {
        if (requireLock)
          _sqliteLock.EnterUpgradeableReadLock();
        using (var command = CreateCommand("SELECT * FROM \"" + tableName + "\" LIMIT 1"))
        {
          try
          {
            command.ExecuteReader().Close();
            return true;
          }
          catch (DbException)
          {
            Log.Debug("[Database] Failed to verify existance of table \"{0}\"", tableName);
            // Create the table?
            if (creationQuery != null)
            {
              using (var creationCommand = CreateCommand(creationQuery))
              {
                if (ExecuteCommand(creationCommand) && TableExists(tableName))
                {
                  Log.Debug("[Database] Created table \"{0}\"", tableName);
                  return true;
                }
                Log.Error("[Database] Failed to create table \"{0}\" with the following query: {1}",
                          tableName, creationQuery);
              }
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
  }
}
