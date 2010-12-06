using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace AppStract.Utilities.Data.Sql
{
  public abstract class SqLiteDatabase<T> : SqlDatabase<T>
  {

    #region Constants

    /// <summary>
    /// A format string for the default connectionstring.
    /// Use like: <code>string.Format(DefaultConnectionStringFormat, myDataSourceVariable)</code>
    /// </summary>
    protected const string _DefaultConnectionStringFormat = "Data Source={0};Version=3;PRAGMA synchronous=OFF;FailIfMissing=True;Journal Mode=Off;";

    #endregion

    #region Constructors

    protected SqLiteDatabase(string connectionString)
      : base(connectionString)
    {
      
    }

    #endregion

    #region Protected Methods

    protected override void CreateDatabase()
    {
      var index = ConnectionString.IndexOf(';');
      var filename = index > 0
                       ? ConnectionString.Substring("Data Source=".Length, index)
                       : ConnectionString.Substring("Data Source=".Length);
      if (!File.Exists(filename))
        File.Create(filename).Close();
    }

    protected static string GetDefaultConnectionString(string filename)
    {
      return string.Format(_DefaultConnectionStringFormat, filename);
    }

    protected override void AssertConnectionString(string connectionString)
    {
      if (!_connectionString.ToLowerInvariant().Contains("data source="))
        throw new DatabaseException("The connectionstring must at least specify a data source: " + connectionString);
    }

    protected override DbCommand CreateCommand(string connectionString, string command)
    {
      return new SQLiteCommand(command, new SQLiteConnection(_connectionString));
    }

    protected override IDbDataParameter CreateParameter(string name, object value)
    {
      return new SQLiteParameter(name, value);
    }

    #endregion

  }
}
