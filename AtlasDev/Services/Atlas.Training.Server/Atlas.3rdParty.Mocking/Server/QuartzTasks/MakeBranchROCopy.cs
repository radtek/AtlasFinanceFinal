using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using System.Configuration;
using System.Data;

using Quartz;
using Npgsql;
using Serilog;
using Humanizer;


namespace Atlas.Server.Training.QuartzTasks
{
  [DisallowConcurrentExecution]
  /// <summary>
  /// Quartz task to copy ass data and make a read-only version (db: ass_ro), using PostgreSQL triggers on every tables
  /// </summary>
  internal class MakeBranchROCopy : IJob
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "MakeBranchROCopy.Execute";
      _log.Information("{MethodName} starting", methodName);
         
      // Temporary PSQL database name to restore the dump into
      var tempDbName = string.Format("tempdb_{0:yyyy_dd_HH_mm_ss}", DateTime.Now);

      // PostgreSQL 'system' connection- NOTE: do not specify a db name for dropping/creating a DB!!!
      var destSystemConn = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLDestConnectionString()) { Database = null };

      var processTimer = Stopwatch.StartNew();

      try
      {
        try
        {
          _log.Information("{MethodName} Task starting", methodName);

          #region Create temp db from ass
          var taskTime = Stopwatch.StartNew();
          _log.Information("Copying ass to {TempDb}...", tempDbName);
          ServiceUtils.CloseAllConnectionsToDb(_log, "ass");
          using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            destConn.Open();
            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandTimeout = (int)TimeSpan.FromHours(3).TotalSeconds;// 3 * 60 * 60; // 3 hours
              cmd.CommandText = $" CREATE DATABASE {tempDbName} WITH TEMPLATE ass OWNER postgres";
              cmd.ExecuteNonQuery();
            }
          }          
         
          _log.Information("{MethodName} Database copy completed in {Elapsed}", methodName, taskTime.Elapsed.Humanize(2));
          #endregion
                            
          NpgsqlConnection.ClearAllPools();
          Thread.Sleep(30000); // Give PSQL some time...

          #region Add triggers to all tables to stop them from being writable, without raising any errors

          taskTime.Restart();
          _log.Information("{MethodName} RO trigger process starting", methodName);
          var tempConn = new NpgsqlConnectionStringBuilder(destSystemConn.ConnectionString) { Database = tempDbName };
          using (var conn = new NpgsqlConnection(tempConn.ConnectionString))
          {
            conn.Open();

            var tables = new List<Tuple<string, string>>();
            using (var cmd = conn.CreateCommand())
            {
              #region Get ASS Schemas/Tables
              cmd.CommandTimeout = 120;
              cmd.CommandText =
                "SELECT table_schema, table_name " +
                "FROM information_schema.tables " +
                "WHERE (table_schema LIKE 'br%') and (table_name NOT LIKE 'sr_%') and (table_name != 'asstmast') and (table_name != 'asstbran')";
              cmd.CommandType = System.Data.CommandType.Text;
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  tables.Add(new Tuple<string, string>(rdr.GetString(0), rdr.GetString(1)));
                }
              }
              #endregion

              // Add the DoNothing trigger function
              cmd.CommandTimeout = 60;
              cmd.CommandText = "CREATE OR REPLACE FUNCTION DoNothing() RETURNS trigger AS $$   BEGIN    RETURN NULL;  END;  $$ LANGUAGE plpgsql;";
              cmd.ExecuteNonQuery();

              // Add the DoNothing() trigger to every ASS table to ignore any inserts/updates
              foreach (var table in tables)
              {
                cmd.CommandTimeout = 20;
                cmd.CommandText = string.Format("CREATE TRIGGER {0}_{1}_donothing BEFORE INSERT OR UPDATE OR DELETE ON {0}.\"{1}\" FOR EACH ROW EXECUTE PROCEDURE DoNothing();", table.Item1, table.Item2);
                cmd.ExecuteNonQuery();
              }
            }
          }
          _log.Information("{MethodName} RO trigger process completed in {Elapsed}", methodName, taskTime.Elapsed.Humanize(2));
          #endregion

          var attemptCount = 0;
          var successful = false;

          #region Drop ass_ro
          taskTime.Restart();
          _log.Information("{MethodName} Dropping ass_ro...", methodName);
          NpgsqlConnection.ClearAllPools();

          while (!successful && attemptCount++ < 3)
          {
            ServiceUtils.CloseAllConnectionsToDb(_log, "ass_ro");
            
            Thread.Sleep(30000); // Give PSQL some time...
            NpgsqlConnection.ClearAllPools();  // We are about to RENAME a db- no connections
            Thread.Sleep(30000); // Give PSQL some time...

            using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
            {
              destConn.Open();

              using (var cmd = destConn.CreateCommand())
              {
                try
                {
                  cmd.CommandTimeout = 500;
                  cmd.CommandText = "DROP DATABASE IF EXISTS ass_ro";
                  cmd.ExecuteNonQuery();
                  successful = true;
                }
                catch (Exception err)
                {
                  _log.Error(err, "{MethodName} DROP DATABASE ass_ro- attempt: {Attempt}", methodName, attemptCount);
                }
              }
            }
          }

          if (!successful)
          {
            _log.Error("{MethodName} Failed to drop ass_ro", methodName);
          }
          else
          {
            _log.Information("{MethodName} Successfully dropped database: {Elapsed}", methodName, taskTime.Elapsed.Humanize(2));
          }
          #endregion

          #region Rename db -> 'temp' to 'ass_ro'
          attemptCount = 0;
          successful = false;
          taskTime.Restart();

          _log.Information("Rename db {Temp} to 'ass_ro'", tempDbName);
          while (!successful && attemptCount++ < 3)
          {
            ServiceUtils.CloseAllConnectionsToDb(_log, tempDbName);

            Thread.Sleep(30000); // Give PSQL some time...
            NpgsqlConnection.ClearAllPools();  // We are about to RENAME a db- no connections
            Thread.Sleep(30000); // Give PSQL some time...

            using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
            {
              destConn.Open();

              using (var cmd = destConn.CreateCommand())
              {
                try
                {
                  cmd.CommandTimeout = 300;
                  cmd.CommandText = string.Format("ALTER DATABASE {0} RENAME TO ass_ro", tempDbName);
                  cmd.ExecuteNonQuery();
                  successful = true;
                }
                catch (Exception err)
                {
                  _log.Error(err, "{MethodName} ALTER DATABASE ass_ro: {Attempt}", methodName, attemptCount);
                }
              }
            }
          }

          _log.Information("{MethodName} Renaming complete- {Elapsed}", methodName, taskTime.Elapsed.Humanize(2));
          #endregion
        }
        catch (Exception err)
        {
          _log.Error(err, "{MethodName}", methodName);
        }
      }
      finally
      {        
        // Clean temp restored db, if rename process failed
        try
        {
          NpgsqlConnection.ClearAllPools(); // We are about to drop a db- no connections

          using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            destConn.Open();

            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandTimeout = 500;
              cmd.CommandText = string.Format("DROP DATABASE IF EXISTS {0}", tempDbName);
              cmd.ExecuteNonQuery();
            }
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "{MethodName} DROP DATABASE {TempDB}", methodName, tempDbName);
        }

        _log.Information("{MethodName} completed", methodName);
      }

      _log.Information("{MethodName} completed in {Elapsed}", methodName, processTimer.Elapsed.Humanize(2));
    }


    #region Private fields

    private static readonly ILogger _log = Log.ForContext<MakeBranchROCopy>();

    #endregion

  }
}
