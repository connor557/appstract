using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.IO;

namespace AppStract.Utilities.Data.Sql
{
  public abstract class SqlCeDatabase<T> : SqlDatabase<T>
  {
  
    #region Constants

    /// <summary>
    /// A format string for the default connectionstring.
    /// Use like: <code>string.Format(DefaultConnectionStringFormat, myDataSourceVariable)</code>
    /// </summary>
    protected const string _DefaultConnectionStringFormat = "Data Source={0}";

    #endregion

    #region Constructors

    protected SqlCeDatabase(string connectionString)
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
      {
        var engine = new SqlCeEngine(ConnectionString);
        engine.CreateDatabase();
      }
    }

    protected static string GetDefaultConnectionString(string filename)
    {
      return string.Format(_DefaultConnectionStringFormat, filename);
    }

    protected override void AssertConnectionString(string connectionString)
    {
      if (!connectionString.ToLowerInvariant().Contains("data source="))
        throw new DatabaseException("The connectionstring must at least specify a data source: " + connectionString);
    }

    protected override DbCommand CreateCommand(string command)
    {
      return new SqlCeCommand(command, new SqlCeConnection(ConnectionString));
    }

    protected override IDbDataParameter CreateParameter(string name, object value)
    {
      return new SqlCeParameter(name, value);
    }

    #endregion

  }
}
