using System.Data;
using System.Data.Common;
using Deveel.Data.Client;

namespace AppStract.Utilities.Data.Sql
{
  public abstract class DeveelDatabase<T> : SqlDatabase<T>
  {

    #region Constants

    /// <summary>
    /// A format string for the default connectionstring.
    /// Use like: <code>string.Format(DefaultConnectionStringFormat, myDataSourceVariable)</code>
    /// </summary>
    protected const string _DefaultConnectionStringFormat = "Path={0};Version=3;PRAGMA synchronous=OFF;FailIfMissing=True;Journal Mode=Off;";

    #endregion

    #region Constructors

    protected DeveelDatabase(string connectionString)
      : base(connectionString)
    {
      
    }

    #endregion

    #region Protected Methods

    protected static string GetDefaultConnectionString(string filename)
    {
      return string.Format(_DefaultConnectionStringFormat, filename);
    }

    protected override void AssertConnectionString(string connectionString)
    {
      if (!_connectionString.ToLowerInvariant().Contains("path="))
        throw new DatabaseException("The connectionstring must at least specify a data source: " + connectionString);
    }

    protected override DbCommand CreateCommand(string connectionString, string command)
    {
      return new DeveelDbConnection(_connectionString).CreateCommand(command);
    }

    protected override IDbDataParameter CreateParameter(string name, object value)
    {
      return new DeveelDbParameter {ParameterName = name, Value = value};
    }

    #endregion

  }
}
