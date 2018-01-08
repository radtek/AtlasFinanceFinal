using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

using Quartz;
using Serilog;
using Humanizer;
using Npgsql;


namespace Atlas.Server.Training.QuartzTasks
{
  [DisallowConcurrentExecution]
  internal class CopyAtlasCore : IJob
  {
    [SuppressMessage("Microsoft.Usage", "CA2241:Provide correct arguments to formatting methods"),
     SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "CopyAtlasCore.Execute";
      _log.Information("{MethodName} starting", methodName);

      // pg_dump temporary filename
      var dumpFile = string.Format("{0}.cpbackup", Path.Combine(ConfigHelper.GetTempPath(), Guid.NewGuid().ToString("N")));

      try
      {
        try
        {
          NpgsqlConnection.ClearAllPools();

          // Temporary PSQL data to restore dump into
          var tempDb = string.Format("tempdb_{0:yyyy_dd_HH_mm_ss}", DateTime.Now);

          // PostgreSQL 'system' connection- NOTE: do not specify a data for dropping/creating a DB!!!
          var destSystemConn = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLDestConnectionString()) { Database = null };

          // PostgreSQL 'data' connection        
          var sourceDataConn = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLSourceConnectionString()) { Database = "atlas_core" };
          var tempDataConn = new NpgsqlConnectionStringBuilder(ConfigHelper.PSQLDestConnectionString()) { Database = tempDb };

          // Wildcards and mixed case do not work with pg_dump.exe- we need to specify every table with DOS escaping.
          // We also need to be aware of FKs... so only really large tables with no FK impact are included here...
          var excludeTables = new List<string> {
          "AVS_Batch", "AVS_Transaction",

          "AEDOReportBatch", "AEDOReportSuccess", "AEDOReportSettled", "AEDOReportFailed", "AEDOReportRetry", "AEDOReportCancelled",
          "AEDOReportFuture", "AEDOReportNewTransaction",

          "NAEDOReportBatch", "NAEDOReportBatch", "NAEDOReportFailed", "NAEDOReportCancelled", "NAEDOReportSuccess",
          "NAEDOReportInProcess", "NAEDOReportTransactionUploaded",  "NAEDOReportFuture", "NAEDOReportDisputed",

          "NUC_LogStockEvent", "NUC_LogAdminEvent","NUC_Transaction",

          "LogTCCTerminal",  "COR_LogMachineInfo", "LogHWStatus", "ASS_LogSyncServerEvent",
          "COR_LogAppUsage","BIO_LogRequest",

          "LogMachineAudit", "COR_AppUsage",
          "NTF_Notification"};

          #region Get table listing of BUR_* and FRM_* and exclude most of these tables, due to painful FKs...
          // BUR_* over-ride to include the requisite look-ups
          var include_BUR = new List<string> { "BUR_AccountPolicy", "BUR_AccountStatusCode", "BUR_AccountTypeCode", "BUR_Policy", "BUR_Band", "BUR_BandRange", "BUR_Service" };

          using (var conn = new NpgsqlConnection(sourceDataConn))
          {
            conn.Open();
            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandText = "SELECT table_name FROM information_schema.tables " +
                "WHERE table_type = 'BASE TABLE' AND table_schema NOT IN ('pg_catalog', 'information_schema') " +
                "AND (table_name ~ 'BUR_*'  OR table_name ~ 'FPM_*')";
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  if (!include_BUR.Contains(rdr.GetString(0)))
                  {
                    excludeTables.Add(rdr.GetString(0));
                  }
                }
              }
            }
          }
          NpgsqlConnection.ClearAllPools();
          #endregion

          #region Run pg_dump to dump atlas_core, excluding data for large, unnecessary tables
          _log.Information("Starting database backup to {DumpFile}", dumpFile);
          var wait = Stopwatch.StartNew();
          using (var dump = new Process())
          {
            var exe = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["PSQLBase"] ?? "C:\\Program Files\\PostgreSQL\\9.5", "bin", "pg_dump.exe");
            dump.StartInfo = new ProcessStartInfo(exe);
            // NOTE: Cannot use 'directory' format + jobs=x, due to postgresql hot-standby limitations -> "cannot assign TransactionIds during recovery"
            // Postgres requires escaping \"TableName\", so we are double escaping here... pg_dump -t --table="\"ACC_Account\"" mydb > mytab.sql
            dump.StartInfo.Arguments = string.Format("--file={0} --format=c --compress=9 --host={1} " +
              "--username={2} --no-password --no-owner {3} atlas_core",
              dumpFile, ConfigHelper.PSQLSourceHost(), ConfigHelper.PSQLSourceUser(),
              string.Join(" ", excludeTables.Select(s => string.Format("--exclude-table-data=\"\\\"{0}\\\"\" ", s))));
            dump.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            dump.StartInfo.UseShellExecute = false;

            dump.StartInfo.EnvironmentVariables.Add("PGUSER", ConfigHelper.PSQLSourceUser());
            dump.StartInfo.EnvironmentVariables.Add("PGPASSWORD", ConfigHelper.PSQLSourcePass());

            _log.Information("Running {PGDump} with arguments: {Arguments}", exe, dump.StartInfo.Arguments);
            if (dump.Start())
            {
              while (!dump.WaitForExit(60000))
              {
                _log.Information("Waiting for pg_dump... {Elapsed}", wait.Elapsed.Humanize(2));
                if (wait.Elapsed > TimeSpan.FromHours(4))
                {
                  _log.Error("pg_dump process exceeded 4 hours");
                  dump.Kill();
                  return;
                }
              }
            }
            else
            {
              _log.Error("Failed to start the pg_dump process");
              return;
            }
          }
          _log.Information("Database backup completed in {Elapsed}", wait.Elapsed.Humanize(2));
          #endregion

          #region Create the temp database to restore into
          using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            _log.Information("Connecting to database");
            destConn.Open();

            _log.Information("Create temporary database {TempDB}", tempDb);
            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandText = string.Format("CREATE DATABASE {0}", tempDb);
              cmd.ExecuteNonQuery();
            }
          }
          #endregion

          NpgsqlConnection.ClearAllPools();
          System.Threading.Thread.Sleep(10000);
          ServiceUtils.CloseAllConnectionsToDb(_log, tempDb);

          #region Run pg_restore to restore the dump into the temp database
          wait.Restart();
          using (var restore = new Process())
          {
            var pg_restore = Path.Combine(System.Configuration.ConfigurationManager.AppSettings["PSQLBase"] ?? "C:\\Program Files\\PostgreSQL\\9.5", "bin", "pg_restore.exe");
            restore.StartInfo = new ProcessStartInfo(pg_restore);
            restore.StartInfo.Arguments = string.Format("--dbname=\"{0}\" --format=c --no-owner --no-privileges --no-acl --host={1} " +
              "--username={2} --no-password --verbose \"{3}\"", tempDb, ConfigHelper.PSQLDestHost(), ConfigHelper.PSQLDestUser(), dumpFile);
            restore.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            restore.StartInfo.UseShellExecute = false;
            restore.StartInfo.EnvironmentVariables.Add("PGPASSWORD", ConfigHelper.PSQLDestPass());
            restore.StartInfo.EnvironmentVariables.Add("PGUSER", ConfigHelper.PSQLDestUser());

            restore.StartInfo.RedirectStandardOutput = true;
            restore.OutputDataReceived += (sender, args) => _log.Information("pg_restore: {line}", args.Data);

            restore.StartInfo.RedirectStandardError = true;
            restore.ErrorDataReceived += (sender, args) => _log.Error("pg_restore: {line}", args.Data);

            _log.Information("Running {PGRestore} with arguments: {Arguments}", pg_restore, restore.StartInfo.Arguments);
            if (restore.Start())
            {
              restore.BeginOutputReadLine(); // !!!!! Critical !!!!!!
              restore.BeginErrorReadLine();

              while (!restore.WaitForExit(60000))
              {
                _log.Information("Waiting for pg_restore... {Elapsed}", wait.Elapsed.Humanize(2));

                if (wait.Elapsed > TimeSpan.FromHours(4))
                {
                  _log.Error("pg_restore process exceeded 4 hours");
                  restore.Kill();
                  return;
                }
              }
            }
            else
            {
              _log.Error("Failed to start the pg_restore process");
              return;
            }
          }
          _log.Information("Database restore completed in {Elapsed}", wait.Elapsed.Humanize(2));
          #endregion

          #region Run update scripts for test evironment
          _log.Information("Running update scripts");

          using (var conn = new NpgsqlConnection(tempDataConn.ConnectionString))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              // Point to compuscan dev environment- 
              // 2015-05-20: No need, we have re-implemented with new scorecard server, which returns a random scorecard
              // 2016-05-20: Need- use for HO testing!
              cmd.CommandText = "UPDATE \"BUR_Service\" " +
                "SET \"Username\" = '30092-1', \"Password\" = 'devtest', \"BranchCode\" = '30053', \"Destination\" = 'T' " +
                "WHERE \"Username\" <> '30092-1'";
              cmd.CommandType = CommandType.Text;
              cmd.ExecuteNonQuery();

              // TCC to deactivate-
              // 2015-05-20: No need- we point to our own SOAP implementation of TCC, which is always successful
              // 2016-05-20: Need- use for HO testing!
              cmd.CommandText = "UPDATE \"TCCTerminal\" SET \"Status\" = 99 WHERE \"TerminalId\" != 1";
              cmd.CommandType = CommandType.Text;
              cmd.ExecuteNonQuery();
            }
          }
          #endregion

          #region Copy 20 recent scorecards for testing scorecard server
          try
          {
            // Copy enquiries
            Atlas.Server.Utils.CopyData.CopyAtlasCoreData(
              "SELECT * FROM \"BUR_Enquiry\" WHERE \"IsSucess\" ORDER BY \"EnquiryDate\" DESC LIMIT 20", "BUR_Enquiry",
              sourceDataConn.ConnectionString, tempDataConn.ConnectionString);

            // Get the enquiry ids we just copied across
            var sb = new System.Text.StringBuilder();
            using (var conn = new NpgsqlConnection(tempDataConn))
            {
              conn.Open();
              using (var cmd = conn.CreateCommand())
              {
                cmd.CommandText = "SELECT \"EnquiryId\" FROM \"BUR_Enquiry\"";
                using (var rdr = cmd.ExecuteReader())
                {
                  while (rdr.Read())
                  {
                    sb.AppendFormat("{0}{1}", sb.Length == 0 ? "" : ",", rdr.GetInt64(0));
                  }
                }
              }
            }

            // Copy the associated scorecards for the enquiries above
            Atlas.Server.Utils.CopyData.CopyAtlasCoreData(
              string.Format("SELECT * FROM \"BUR_Storage\" WHERE \"EnquiryId\" IN ({0})", sb.ToString()),
              "BUR_Storage", sourceDataConn.ConnectionString, tempDataConn.ConnectionString);
          }
          catch (Exception err) // Not fatal? 
          {
            _log.Error(err, "{MethodName}- Error while copying scorecard data");
          }
          #endregion

          #region Switch over (delete old DB and rename)

          NpgsqlConnection.ClearAllPools();
          ServiceUtils.CloseAllConnectionsToDb(_log, "atlas_core");

          using (var destConn = new NpgsqlConnection(destSystemConn.ConnectionString))
          {
            destConn.Open();

            _log.Information("Renaming database");
            using (var cmd = destConn.CreateCommand())
            {
              cmd.CommandTimeout = 120;
              cmd.CommandText = string.Format("DROP DATABASE IF EXISTS atlas_core", tempDb);
              cmd.ExecuteNonQuery();

              cmd.CommandText = string.Format("ALTER DATABASE {0} RENAME TO atlas_core", tempDb);
              cmd.ExecuteNonQuery();
            }
            _log.Information("Renaming complete");
          }

          _log.Information("Task completed");
          #endregion
        }
        catch (Exception err)
        {
          _log.Error(err, methodName);
        }
      }
      finally
      {
        if (File.Exists(dumpFile))
        {
          //File.Delete(dumpFile);
        }
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private fields

    private static readonly ILogger _log = Log.ForContext<CopyAtlasCore>();

    #endregion

  }
}
