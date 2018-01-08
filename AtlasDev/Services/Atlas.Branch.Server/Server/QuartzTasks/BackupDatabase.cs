/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Quartz task to dump and ZIP/encrypt(password: backupASS) the local ASS PostgreSQL database and delete backups 
 *   older than MAX_DAYS_TO_KEEP (14) days
 *   
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-06-13  Created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ASSSyncClient.Utils;
using ASSSyncClient.Utils.Settings;
using Atlas.Desktop.Utils.Crypto;
using Ionic.Zip;
using Ionic.Zlib;
using Quartz;
using Serilog;


#endregion


namespace ASSSyncClient.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class BackupDatabase : IJob
  {
    /// <summary>
    /// Main routine- dump the database and compress
    /// </summary>
    /// <param name="context"></param>
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "BackupDatabase.Execute";
      _log.Information("{MethodName} DB backup- Starting...", methodName);

      var tempDumpSQLDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());  // NOTE: PG_DUMP will create the directory- !MUST NOT EXIST!
      var destZIPFile = Path.Combine(DB_DUMP_PATH, string.Format("{0:yyyy-MM-dd-HH-mm-ss}.backup", DateTime.Now));

      try
      {
        try
        {
          #region Delete backups which are older than 14 days
          try
          {
            if (Directory.Exists(DB_DUMP_PATH))
            {
              var files = Directory.GetFiles(DB_DUMP_PATH, "*.backup").ToList();
              foreach (var file in files)
              {
                var fileInfo = new FileInfo(file);
                if ((int)DateTime.Now.Subtract(fileInfo.CreationTime).TotalDays > MAX_DAYS_TO_KEEP)
                {
                  try
                  {
                    File.Delete(file);
                  }
                  catch (Exception err)
                  {
                    _log.Error(err, "{MethodName} Deleting old backup file '{File}'", methodName, file);
                  }
                }
              }
            }
          }
          catch (Exception err)
          {
            LogEvents.Log(DateTime.Now, "BackupDatabase.Execute- Clean", err.Message, 3);
          }
          #endregion

          if (!Directory.Exists(DB_DUMP_PATH))
          {
            Directory.CreateDirectory(DB_DUMP_PATH);
          }

          #region Dump the database
          using (var process = new Process())
          {
            _log.Information("{MethodName} Starting pg_dump to directory: '{0}'", methodName, tempDumpSQLDir);
            var args = string.Format("--file=\"{0}\" --format=d --compress=7 --quote-all-identifiers --host=localhost --port=5432 --username={1} --jobs={2} --no-password ass",
              tempDumpSQLDir, BasicCrypto.Decrypt(AppSettings.A, AppSettings.B, AppSettings.C), Math.Min(2, Environment.ProcessorCount));
            var startInfo = new ProcessStartInfo(PG_DUMP_PATH, args);
            startInfo.EnvironmentVariables.Add("PGPASSWORD", BasicCrypto.Decrypt(AppSettings.A, AppSettings.B, AppSettings.D));
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false; // Required for environment

            process.StartInfo = startInfo;
            if (!process.Start())
            {
              var error = new Exception("Unable to start dump process");
              throw error;
            }

            _log.Information("{MethodName} pg_dump started successfully", methodName);
            var timeout = new Stopwatch();
            timeout.Start();
            var complete = false;
            while (timeout.Elapsed.TotalMinutes < TimeSpan.FromMinutes(20).TotalMinutes && !complete)
            {
              process.Refresh();
              complete = process.WaitForExit(1000) || process.HasExited;
            }

            if (!complete || process.ExitCode != 0) // Errors
            {
              if (!process.HasExited)
              {
                process.Kill();
              }

              var error = new Exception(string.Format("Dump was unsuccessful- pg_dump.exe ExitCode: {0}", process.ExitCode));
              throw error;
            }
          }
          #endregion

          // Zip the directory
          using (var zip = new ZipFile(destZIPFile))
          {
            // Encrypt and store            
            zip.Encryption = EncryptionAlgorithm.WinZipAes256;
            zip.Password = "backupASS";
            zip.CompressionLevel = CompressionLevel.None;
            zip.CompressionMethod = CompressionMethod.None;
            zip.AddDirectory(tempDumpSQLDir, "");
            zip.Save();
          }
        }
        catch (Exception err)
        {
          LogEvents.Log(DateTime.Now, "BackupDatabase.Execute", err.Message, 10);
          _log.Error(err, "{MethodName} Execute", methodName);
        }
      }
      finally
      {
        #region Clean up
        try
        {
          if (Directory.Exists(tempDumpSQLDir))
          {
            Directory.Delete(tempDumpSQLDir, true);
          }
        }
        catch { }
        #endregion

        _log.Information("{MethodName}- Completed...", methodName);
      }
    }


    #region Private vars

    // Logging
    private static readonly ILogger _log = Log.Logger.ForContext<BackupDatabase>();

    private static readonly string PG_DUMP_PATH = "C:\\Atlas\\LMS\\DBServer\\bin\\pg_dump.exe";

    private static readonly string DB_DUMP_PATH = "C:\\Atlas\\LMS\\Backups\\";

    private static readonly int MAX_DAYS_TO_KEEP = 14;

    #endregion

  }
}
