using System;
using System.Data;

using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Exceptions;


namespace Atlas.Data.DevExpress
{
  /// <summary>
  /// This class implements a re-connecting PostgreSQL XPO provider. Under normal circumstances, if PostgreSQL is
  /// restarted and the Windows service is not, XPO in the service will barf and not perform a reconnect, causing
  /// the service to fail.
  /// </summary>
  /// <remarks>
  /// Haven't been able to get 
  /// </remarks>
  public class SafePostgreSqlConnectionProvider : IDataStore, IDisposable
  {
    PostgreSqlConnectionProvider InnerDataStore;
    IDbConnection Connection;
    readonly string ConnectionString;
    readonly AutoCreateOption AutoCreateOption;

    public SafePostgreSqlConnectionProvider(string connectionString, AutoCreateOption autoCreateOption)
    {
      this.ConnectionString = connectionString;
      this.AutoCreateOption = autoCreateOption;
      DoReconnect();
    }

        
    public int ExecuteNonQuery(string sql)
    {
      using (var cmd = InnerDataStore.Connection.CreateCommand())
      {
        cmd.CommandText = sql;
        return cmd.ExecuteNonQuery();
      }
    }
    

    ~SafePostgreSqlConnectionProvider()
    {
      Dispose(false);
    }


    void DoReconnect()
    {
      DoDispose(false);
      Connection = PostgreSqlConnectionProvider.CreateConnection(ConnectionString);
      InnerDataStore = new PostgreSqlConnectionProvider(Connection, AutoCreateOption);
    }


    void DoDispose(bool closeConnection)
    {
      if (Connection != null)
      {
        if (closeConnection)
        {
          Connection.Close();
          Connection.Dispose();
        }
        Connection = null;
      }
    }


    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
        DoDispose(true);
    }

    void HandleNullReferenceException(Exception ex)
    {
      if (ex == null) return;
      if (ex is System.IO.IOException && InnerDataStore.Connection.State == ConnectionState.Open)
        DoReconnect();
      else throw ex;
    }


    AutoCreateOption IDataStore.AutoCreateOption
    {
      get { return InnerDataStore.AutoCreateOption; }
    }


    ModificationResult IDataStore.ModifyData(params ModificationStatement[] dmlStatements)
    {
      try
      {
        return InnerDataStore.ModifyData(dmlStatements);
      }
      catch (SqlExecutionErrorException ex)
      {
        HandleNullReferenceException(ex.InnerException);
      }
      return InnerDataStore.ModifyData(dmlStatements);
    }


    SelectedData IDataStore.SelectData(params SelectStatement[] selects)
    {
      try
      {
        return InnerDataStore.SelectData(selects);
      }
      catch (NullReferenceException ex)
      {
        HandleNullReferenceException(ex.InnerException);
      }
      
      return InnerDataStore.SelectData(selects);
    }


    UpdateSchemaResult IDataStore.UpdateSchema(bool dontCreateIfFirstTableNotExist, params DBTable[] tables)
    {
      try
      {
        return InnerDataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
      }
      catch (SqlExecutionErrorException ex)
      {
        HandleNullReferenceException(ex.InnerException);
      }
      return InnerDataStore.UpdateSchema(dontCreateIfFirstTableNotExist, tables);
    }


    void IDisposable.Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

  }
}