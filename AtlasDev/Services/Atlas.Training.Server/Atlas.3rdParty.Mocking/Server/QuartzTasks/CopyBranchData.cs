using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Humanizer;
using Npgsql;
using Quartz;
using Serilog;


namespace Atlas.Server.Training.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class CopyBranchData : IJob
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public void Execute(IJobExecutionContext context)
    {
      var totalTask = Stopwatch.StartNew();
      var methodName = "CopyBranchData.Execute";
      // pg_dump temporary filename
      var dumpDir = string.Format("{0}", Path.Combine(ConfigHelper.GetTempPath(), Guid.NewGuid().ToString("N"))).Trim('\\');

      // Temporary PSQL db name to restore dump into
      var tempDbName = string.Format("tempdb_{0:yyyy_dd_HH_mm_ss}", DateTime.Now);

      // PostgreSQL 'system' connection- NOTE: do not specify a DB for dropping/creating a DB!!!
      var destSystemConn = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLDestConnectionString()) { Database = null };

      // PostgreSQL 'data' connection
      var destDataConn = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLDestConnectionString()) { Database = "ass" };

      try
      {
        try
        {
          _log.Information("{MethodName} Task starting", methodName);

          NpgsqlConnection.ClearAllPools();

          #region Run pg_dump to dump all schemas except company
          _log.Information("{MethodName} Starting database backup to {dumpDir}", methodName, dumpDir);
          var wait = Stopwatch.StartNew();
          using (var dump = new Process())
          {
            var pg_dump = Path.Combine(ConfigurationManager.AppSettings["PSQLBase"] ?? "C:\\Program Files\\PostgreSQL\\9.5", "bin", "pg_dump.exe");
            dump.StartInfo = new ProcessStartInfo(pg_dump);
            var testing = false;
            // NOTE: Cannot use 'directory' format + jobs=x, due to postgresql hot-standby limitations -> "cannot assign TransactionIds during recovery"
            //       Using pre 9.2 over-ride: --no-synchronized-snapshots - seems to work?!?
            dump.StartInfo.Arguments = string.Format("--file={0} --format=d --compress=9 --host={1} --no-synchronized-snapshots --jobs=24 " +
              "--username={2} --no-password --no-owner {3} ass",
              dumpDir, ConfigHelper.PSQLSourceHost(), ConfigHelper.PSQLSourceUser(),
              bool.TryParse(ConfigurationManager.AppSettings["CopyAssDbNowOnlySingle"] ?? "false", out testing) && testing ? "--schema=br001" : "--exclude-schema=company");

            dump.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            dump.StartInfo.UseShellExecute = false;

            // CREATE: %APPDATA%\postgresql\pgpass.conf :
            // 10.0.0.245:5432:*:postgres:Fruity123!
            // 10.0.0.244:5432:*:postgres: s1DT81ChqlVkPZMlRO8b

            // Ensure service has access to file...
            dump.StartInfo.EnvironmentVariables.Add("PGUSER", ConfigHelper.PSQLSourceUser());
            dump.StartInfo.EnvironmentVariables.Add("PGPASSWORD", ConfigHelper.PSQLSourcePass());

            dump.StartInfo.RedirectStandardOutput = true;

            _log.Information("{MethodName} Running {PGDump} with arguments: {Arguments}", methodName, pg_dump, dump.StartInfo.Arguments);
            if (dump.Start())
            {
              while (!dump.WaitForExit(60000))
              {
                _log.Information("{MethodName} Waiting for pg_dump {Elapsed}", methodName, wait.Elapsed.Humanize(2));

                if (wait.Elapsed > TimeSpan.FromHours(4))
                {
                  _log.Error("{MethodName} pg_dump process exceeded 4 hours", methodName);
                  dump.Kill();
                  return;
                }
              }
            }
            else
            {
              _log.Error("Failed to start the pg_dump process", methodName);
              return;
            }

            if (dump.ExitCode != 0)
            {
              _log.Error("{MethodName} pg_dump Exit code {ExitCode}- '{Output}'", methodName, dump.ExitCode, dump.StandardOutput.ReadToEnd());
              return;
            }
          }
          _log.Information("{MethodName} Database backup completed in {Elapsed}", methodName, wait.Elapsed.Humanize(2));
          #endregion

          #region Create the temp database to restore into
          using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            _log.Information("{MethodName} Connecting to database", methodName);
            destConn.Open();

            _log.Information("{MethodName} Create temporary database {TempDB}", methodName, tempDbName);
            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandText = string.Format("CREATE DATABASE {0}", tempDbName);
              cmd.ExecuteNonQuery();
            }
          }
          #endregion

          #region Add HSTORE to temporary
          _log.Information("{MethodName} Create hstore extension in database {TempDB}", methodName, tempDbName);
          destDataConn.Database = tempDbName;
          using (var conn = new NpgsqlConnection(destDataConn.ConnectionString))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandText = "CREATE EXTENSION hstore;";
              cmd.CommandType = CommandType.Text;
              cmd.ExecuteNonQuery();
            }
          }

          NpgsqlConnection.ClearAllPools();
          ServiceUtils.CloseAllConnectionsToDb(_log, tempDbName);
          Thread.Sleep(10000);
          #endregion

          #region Run pg_restore to restore the dump into the temp database
          wait.Restart();
          using (var restore = new Process())
          {
            var pg_restore = Path.Combine(ConfigurationManager.AppSettings["PSQLBase"] ?? "C:\\Program Files\\PostgreSQL\\9.5", "bin", "pg_restore.exe");
            restore.StartInfo = new ProcessStartInfo(pg_restore);
            restore.StartInfo.Arguments = string.Format("--dbname={0} --format=d --no-owner --no-privileges --no-acl --host={1} --jobs=16 " +
              "--username={2} --exit-on-error --verbose  {3}", tempDbName, ConfigHelper.PSQLDestHost(), ConfigHelper.PSQLDestUser(), dumpDir);

            restore.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            restore.StartInfo.UseShellExecute = false;
            restore.StartInfo.CreateNoWindow = true;

            restore.StartInfo.EnvironmentVariables.Add("PGUSER", ConfigHelper.PSQLDestUser());
            restore.StartInfo.EnvironmentVariables.Add("PGPASSWORD", ConfigHelper.PSQLDestPass());
           
            restore.StartInfo.RedirectStandardOutput = true;
            restore.OutputDataReceived += (sender, args) => _log.Information("pg_restore: {line}", args.Data);
            
            restore.StartInfo.RedirectStandardError = true;
            restore.ErrorDataReceived += (sender, args) => _log.Error("pg_restore: {line}", args.Data);

            _log.Information("{MethodName} Running {PGRestore} with arguments: {Arguments}", methodName, pg_restore, restore.StartInfo.Arguments);
            if (restore.Start())
            {
              restore.BeginOutputReadLine();
              restore.BeginErrorReadLine();

              while (!restore.WaitForExit(60000))
              {
                _log.Information("{MethodName} Waiting for pg_restore {Elapsed}", methodName, wait.Elapsed.Humanize(2));

                if (wait.Elapsed > TimeSpan.FromHours(4))
                {
                  _log.Fatal("{MethodName} pg_restore process exceeded 4 hours", methodName);
                  restore.Kill();
                  return;
                }
              }
            }
            else
            {
              _log.Error("{MethodName} Failed to start the pg_restore process", methodName);
              return;
            }

            if (restore.ExitCode != 0)
            {
              _log.Warning("{MethodName} pg_restore Exit code {ExitCode}", methodName, restore.ExitCode);
              return;
            }
          }
          _log.Information("{MethodName} Database restore completed in {Elapsed}", methodName, wait.Elapsed.Humanize(2));
          #endregion

          _log.Information("{MethodName} Processing sequences/PKs...", methodName);

          destDataConn.Database = tempDbName;

          #region Get listing of tables/schemas
          var tablesToDo = new ConcurrentBag<Tuple<string, string>>();
          var schemasToDo = new List<string>();
          using (var destConn = new NpgsqlConnection(destDataConn.ConnectionString))
          {
            destConn.Open();

            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandTimeout = 120;

              #region Tables
              cmd.CommandText = "SELECT DISTINCT table_schema, table_name " +
                "FROM information_schema.columns " +
                "WHERE column_name = 'sr_recno' AND table_schema ~ '^br0[0-9A-Za-z]{1,1}[0-9]{1,1}$'";
              cmd.CommandType = CommandType.Text;
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  var schemaName = rdr.GetString(0);
                  var tableName = rdr.GetString(1);

                  if (!tableName.StartsWith("lrep_") && !tableName.StartsWith("sr_"))
                  {
                    tablesToDo.Add(new Tuple<string, string>(schemaName, tableName));
                  }
                }
              }
              _log.Information("{MethodName} Found {TableCount:N0} tables to update", methodName, tablesToDo.Count);
              #endregion

              #region Schemas
              cmd.CommandText = "select table_schema from information_schema.tables " +
                "WHERE table_schema ~ '^br0[0-9A-Za-z]{1,1}[0-9]{1,1}$' and table_name = 'group'";
              cmd.CommandType = CommandType.Text;

              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  schemasToDo.Add(rdr.GetString(0));
                }
              }
              #endregion
            }
          }
          #endregion

          #region Process PKs/sequences
          var tablesDone = 0;
          _log.Information("{MethodName} Restoring sequences and PKs on tables", methodName);
          var tasks = new Task[PK_TASK_COUNT];
          wait.Restart();
          for (var i = 0; i < PK_TASK_COUNT; i++)
          {
            tasks[i] = Task.Factory.StartNew(() =>
              {
                Tuple<string, string> table;
                while (tablesToDo.TryTake(out table))
                {
                  try
                  {
                    using (var destConn = new NpgsqlConnection(destDataConn.ConnectionString))
                    {
                      destConn.Open();
                      using (var cmd = destConn.CreateCommand())
                      {
                        cmd.CommandType = CommandType.Text;

                        #region Get current sr_recno value to use as sequence seed
                        cmd.CommandText = string.Format("SELECT COALESCE(MAX(sr_recno), 0) FROM {0}.\"{1}\"", table.Item1, table.Item2);
                        var currVal = (Decimal)cmd.ExecuteScalar();
                        #endregion

                        #region Add sequence and set it to be used by the table- if sequence already exists, ignore error
                        cmd.CommandTimeout = 240;
                        cmd.CommandText = string.Format("CREATE SEQUENCE {0}.{1}_sq", table.Item1, table.Item2);
                        try
                        {
                          cmd.ExecuteNonQuery();
                        }
                        catch (Exception err)
                        {
                          _log.Error(err, "{MethodName} CREATE SEQUENCE {schema}.{table}_sq", methodName, table.Item1, table.Item2);
                        }

                        cmd.CommandText = string.Format("ALTER SEQUENCE {0}.{1}_sq RESTART WITH {2}", table.Item1, table.Item2, currVal + 1);
                        cmd.ExecuteNonQuery();

                        // Set sr_recno to use sequence
                        cmd.CommandText = string.Format("ALTER TABLE {0}.\"{1}\" ALTER COLUMN sr_recno SET DEFAULT NEXTVAL('{0}.{1}_sq');", table.Item1, table.Item2);
                        cmd.ExecuteNonQuery();
                        #endregion

                        #region Make sr_recno field, the primary key
                        cmd.CommandTimeout = 400;
                        cmd.CommandText = string.Format(
                          "ALTER TABLE {0}.\"{1}\" DROP CONSTRAINT IF EXISTS pk_{1}; " +
                          "ALTER TABLE {0}.\"{1}\" DROP CONSTRAINT IF EXISTS {1}_sr_recno_key; " +
                          "ALTER TABLE {0}.\"{1}\" DROP CONSTRAINT IF EXISTS pk_{0}_{1}; " +
                          "ALTER TABLE {0}.\"{1}\" ADD CONSTRAINT {1}_sr_recno_key PRIMARY KEY (sr_recno);", table.Item1, table.Item2);
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                        #endregion

                        Interlocked.Increment(ref tablesDone);
                        if (tablesDone % 50 == 0)
                        {
                          _log.Information("{MethodName} Updated {TableCount} sequence/PKs", methodName, tablesDone);
                        }
                      }
                    }
                  }
                  catch (Exception err)
                  {
                    _log.Error(err, "{MethodName} Error while processing {Table}", methodName, table);
                  }
                }
              });
          }

          Task.WaitAll(tasks, TimeSpan.FromHours(2));
          _log.Information("{MethodName} Sequences and PKs restored in {Time}", methodName, wait.Elapsed.Humanize(2));
          #endregion

          #region Set brXXX.GROUP to point to the test server environment/test e-mail
          wait.Restart();
          _log.Information("{MethodName} Updating all 'group' tables to point to test environment", methodName);

          using (var destConn = new NpgsqlConnection(destDataConn.ConnectionString))
          {
            destConn.Open();

            using (var cmd = destConn.CreateCommand())
            {
              foreach (var schema in schemasToDo)
              {
                cmd.CommandText = string.Format("UPDATE {0}.\"group\" SET " +
                  "serverip = '10.0.0.245', " +
                  //"email_svr1= 'mail.atcorp.co.za'', " +
                  "dcemail = 'training_mocking@atcorp.co.za', " +
                  "email_accs = 'training_mocking@atcorp.co.za', " +
                  "email_hub = 'training_mocking@atcorp.co.za', " +
                  "email_sdc = 'training_mocking@atcorp.co.za'", schema);
                cmd.ExecuteNonQuery();
              }
            }
          }
          _log.Information("{MethodName} Updated 'group' tables in {Elapsed}", methodName, wait.Elapsed.Humanize(2));
          #endregion

          NpgsqlConnection.ClearAllPools();
          ServiceUtils.CloseAllConnectionsToDb(_log, "ass");
          ServiceUtils.CloseAllConnectionsToDb(_log, tempDbName);

          #region Switch over (delete old DB and rename)
          using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            destConn.Open();

            _log.Information("{MethodName} Dropping ass", methodName, tempDbName);
            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandTimeout = 500;
              cmd.CommandText = "DROP DATABASE IF EXISTS ass";
              cmd.ExecuteNonQuery();

              _log.Information("Renaming {tempDb} to ass...", tempDbName);
              cmd.CommandText = string.Format("ALTER DATABASE {0} RENAME TO ass", tempDbName);
              cmd.ExecuteNonQuery();
            }
            _log.Information("{MethodName} Renaming completed", methodName);
          }

          _log.Information("{MethodName} Task completed", methodName);
          #endregion                             
        }
        catch (Exception err)
        {
          _log.Error(err, "{MethodName} Execute", methodName);
        }
      }
      finally
      {
        #region Delete dest dump dir
        if (Directory.Exists(dumpDir))
        {
          try
          {
            //Directory.Delete(dumpDir, true);
          }
          catch (Exception err)
          {
            _log.Error(err, "{MethodName} {DumpDir}", methodName, dumpDir);
          }
        }
        #endregion

        #region Dump temp database if exists
        try
        {
          NpgsqlConnection.ClearAllPools();

          Thread.Sleep(60000); // give psql some recovery time...
          _log.Information("Deleting temporary database, if exists: {TempDbName}", tempDbName);
          using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            destConn.Open();

            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandTimeout = 600;
              cmd.CommandText = string.Format("DROP DATABASE IF EXISTS {0}", tempDbName);
              cmd.ExecuteNonQuery();
            }
          }
          _log.Information("Successfully deleted temporary database, if exists: {TempDbName}", tempDbName);
        }
        catch (Exception err)
        {
          _log.Error(err, "{MethodName} DROP DATABASE {TenpDB}", methodName, tempDbName);
        }
        #endregion
      }

      _log.Information("{MethodName} completed in {Elapsed}", methodName, totalTask.Elapsed.Humanize(2));
    }


    #region Private fields

    private static readonly ILogger _log = Log.ForContext<CopyBranchData>();

    /// <summary>
    /// Primary key/sequence task count
    /// </summary>
    private const int PK_TASK_COUNT = 4;

    #endregion

  }
}
