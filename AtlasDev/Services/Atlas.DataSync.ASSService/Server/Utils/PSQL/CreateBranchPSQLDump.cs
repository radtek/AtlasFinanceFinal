/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Creates a compressed custom backup of a specified schema (branch) 
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-07-01 Created
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using ASSServer.WCF;


namespace ASSServer.Utils.PSQL
{
  public class CreateBranchPSQLDump
  {
    public static void Execute(ILogging log, ICacheServer cache, IConfigSettings config, string legacyBranchNum, string transactionId)
    {
      var methodName = "Execute";
      var tempDir = Path.Combine(Path.GetTempPath(), transactionId);
      try
      {
        log.Information("[{0}] CreateBranchPSQLDump.Execute starting- {1}", legacyBranchNum, transactionId);

        if (Directory.Exists(tempDir))
        {
          Directory.Delete(tempDir, true);
        }

        try
        {
          #region Ensure the branch has the latest master table data
          var schemaName = string.Format("br{0}", legacyBranchNum.ToLower().PadLeft(3, '0'));
          var messages = new ConcurrentQueue<string>();
          if (!ASSServer.Utils.PSQL.CopyMasterTablesToBranch.Execute(cache, config, schemaName, messages))
          {
            var error = new Exception(string.Format("[{0}] Failed copy master data to branch data: '{1}'",
              legacyBranchNum, string.Join(". ", messages)));
            log.Error(error, methodName);
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, error.Message, null);
            return;
          }
          #endregion

          #region Dump branch schema using 'Directory' format- directory must not exist!
          log.Information("[{0}] Dumping PSQL database for branch", legacyBranchNum);

          if (!File.Exists(PG_DUMP_EXE_PATH))
          {
            var error = new FileNotFoundException("pg_dump.exe missing", PG_DUMP_EXE_PATH);
            log.Error(error, "CreateBranchPSQLDump.Execute");
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, error.Message);
            return;
          }

          using (var process = new Process())
          {
            var connStr = new Npgsql.NpgsqlConnectionStringBuilder(config.GetAssConnectionString());
            log.Information("[{0}] Starting pg_dump to directory: '{1}'", legacyBranchNum, tempDir);
            var args = $"--file=\"{tempDir}\" --format=d --jobs=4 --schema=br{legacyBranchNum} --compress=9 " +
              $"--quote-all-identifiers --host={connStr.Host} --port=5432 --username={connStr.Username} --no-password ass";

            var startInfo = new ProcessStartInfo(PG_DUMP_EXE_PATH, args)
            {
              WindowStyle = ProcessWindowStyle.Hidden,
              UseShellExecute = false,
              RedirectStandardError = true,
              RedirectStandardOutput = true
            };
            startInfo.EnvironmentVariables.Add("PGPASSWORD", connStr.Password);
            
            process.StartInfo = startInfo;
            if (!process.Start())
            {
              var error = new Exception(string.Format("[{0}] Unable to start dump process", legacyBranchNum));
              log.Error(error, "CreateBranchPSQLDump.Execute");
              ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, error.Message);
              return;
            }

            log.Information("[{0}] pg_dump started successfully", legacyBranchNum);
            var timeout = new Stopwatch();
            timeout.Start();
            var complete = false;
            while (timeout.Elapsed.TotalMinutes < TimeSpan.FromMinutes(20).TotalMinutes && !complete)
            {
              process.Refresh();
              complete = process.WaitForExit(1000) || process.HasExited;
              log.Information("[{0}] {1}- pg_dump for branch in progress...", legacyBranchNum, transactionId);
            }

            if (!complete || process.ExitCode != 0 || Directory.GetFiles(tempDir).Length < 50) // Errors
            {
              if (!process.HasExited)
              {
                process.Kill();
              }

              var consoleErr = process.StandardError.ReadToEnd();
              var standardOutput = process.StandardOutput.ReadToEnd();
              var error = new Exception(string.Format("Dump was unsuccessful- pg_dump.exe ExitCode: {0}- Err: {1}, Output: {2}", process.ExitCode, consoleErr, standardOutput));
              log.Error(error, "CreateBranchPSQLDump.Execute");
              ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, error.Message);
              return;
            }
          }
          log.Information("[{0}] pg_dump completed successfully", legacyBranchNum);
          #endregion

          #region Zip the 'Directory' backup using 'Store' compression- pg_dump performed necessary compression, no need to compress again...
          log.Information("[{0}] Zip storage starting", legacyBranchNum);
          var tempZip = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), string.Format("{0}.zip", transactionId));
          using (var zip = new Ionic.Zip.ZipFile(tempZip))
          {
            zip.CompressionMethod = Ionic.Zip.CompressionMethod.None;
            zip.CompressionLevel = Ionic.Zlib.CompressionLevel.None;
            zip.AddDirectory(tempDir, "");
            zip.Save();
          }
          log.Information("[{0}] Zip storage completed", legacyBranchNum);
          #endregion

          ASSServer.Shared.TempFiles.AddTempFile(tempZip);
          ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Completed, null, tempZip);
        }
        catch (Exception err)
        {
          log.Error(err, "{MethodName}- {Branch}", legacyBranchNum);
          ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, err.Message);
        }
      }
      finally
      {
        if (Directory.Exists(tempDir))
        {
          Directory.Delete(tempDir, true);
        }
      }
    }


    #region Private vars

    private static readonly string PG_DUMP_EXE_PATH = "C:\\Program Files\\PostgreSQL\\9.5\\bin\\pg_dump.exe";

    #endregion

  }
}
