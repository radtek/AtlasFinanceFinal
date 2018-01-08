using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Npgsql;


namespace Atlas.Common.Utils
{
  public class RawSql
  {
    public List<T> ExecuteObject<T>(string sql, string connectionString, int commandTimeout = 1800)
      where T : class, new()
    {
      List<T> data;
      using (var connection = new NpgsqlConnection(connectionString)) 
      {
        try
        {
          connection.Open();
          data = connection.Query<T>(sql, null, null, true, commandTimeout).ToList();
        }
        finally
        {
          if (connection.State != ConnectionState.Closed)
            connection.Close();
        }
      }

      return data;
    }

    public async Task<List<T>> ExecuteObjectAsync<T>(string sql, string connectionString, int commandTimeout = 600)
      where T : class, new()
    {
      List<T> data;
      using (var connection = new NpgsqlConnection(connectionString))
      {
        try
        {
          connection.Open();
          data = (await connection.QueryAsync<T>(sql: sql,param: null,transaction: null,commandTimeout: commandTimeout)).ToList();
        }
        finally
        {
          if (connection.State != ConnectionState.Closed)
            connection.Close();
        }
      }

      return data;
    }

    public List<long> ExecuteObject(string sql, string connectionString, int commandTimeout = 600)
    {
      List<long> data;
      using (var connection = new NpgsqlConnection(connectionString))
      {
        try
        {
          connection.Open();
          data = connection.Query<long>(sql, null, null, true, commandTimeout).ToList();
        }
        finally
        {
          if (connection.State != ConnectionState.Closed)
            connection.Close();
        }
      }
      return data;
    }

    public List<string> ExecuteObjectString(string sql, string connectionString, int commandTimeout = 600)
    {
      List<string> data;
      using (var connection = new NpgsqlConnection(connectionString))
      {
        try
        {
          connection.Open();
          data = connection.Query<string>(sql, null, null, true, commandTimeout).ToList();
        }
        finally
        {
          if (connection.State != ConnectionState.Closed)
            connection.Close();
        }
      }
      return data;
    }

    public object ExecuteScalar(string sql, string connectionString, int commandTimeout = 600)
    {
      object data;
      using (var connection = new NpgsqlConnection(connectionString))
      {
        try
        {
          connection.Open();
          data = connection.ExecuteScalar(sql, null, null, commandTimeout);
        }
        finally
        {
          if (connection.State != ConnectionState.Closed)
            connection.Close();
        }
      }
      return data;
    }
  }
}