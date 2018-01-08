/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*     Quartz task- Month-end task- Download all branch DBF data to local server and copy to HO server:
*     (\\10.0.0.22\d\loanYYMM)
*     
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     2013-11-05 - Created
*     
* 
*  To do:
*  -------------------   
*    
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Collections.Concurrent;

using Quartz;
using Serilog;
using Humanizer;
using Npgsql;

using Atlas.DataSync.WCF.Client.ClientProxies;
using Atlas.DataSync.WCF.Interface;
using Atlas.DataSync.WCF.Client.Utils;
using Atlas.Services.DbfGenerate.QuartzTasks.Utils;


namespace Atlas.Services.DbfGenerate.QuartzTasks
{
  /// <summary>
  /// This is only to be used in case Head office WAL replication fails- 
  /// reverts to downloading from .38 ASS datasync server
  /// </summary>
  [DisallowConcurrentExecution]
  public class DownloadDBFs_Emergency : IJob
  {
    /// <summary>
    /// Main execution- request DBFs for all branches and unzip to HO server
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      _log.Information("Execute starting");
      try
      {
        var baseDir = System.Configuration.ConfigurationManager.AppSettings["BaseDir"];
        var folderDate = string.Format("loan{0:yyMM}", DateTime.Now);
        var dbfDir = Path.Combine(baseDir, folderDate);
        var qdDate = DateTime.Now.Subtract(TimeSpan.FromDays(DateTime.Now.Hour >= 20 ? 0 : 1)); // If run after 20:00, we can generate for today, else for yesterday
        if (!Directory.Exists(dbfDir))
        {
          Directory.CreateDirectory(dbfDir);
        }

        #region Get brXXX schemas and use as branch list
        var currBranches = new List<string>();
        var assConnectionString = ConnStrings.Ass;      
        var masterTables = new List<string>();

        using (var conn = new NpgsqlConnection(assConnectionString))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT schema_name FROM information_schema.schemata WHERE schema_name ~ '^br0' ORDER BY schema_name";
            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                currBranches.Add(rdr.GetString(0).Substring(2).PadLeft(3, '0'));
              }
            }
          }
        }        
        #endregion

        try
        {
          var branchToDo = new ConcurrentQueue<string>(currBranches.OrderByDescending(s => s));

          #region Create tasks
          var downloadTasks = new Task[DBF_DOWNLOAD_TASKS];
          for (var i = 0; i < downloadTasks.Length; i++)
          {
            downloadTasks[i] = Task.Factory.StartNew(() =>
              {                        
                string branch;
                while (branchToDo.TryDequeue(out branch))
                {
                  string tempZip = Path.Combine(Path.GetTempPath(), string.Format("{0}-{1}.zip", Guid.NewGuid().ToString("N"), branch));

                  var errCount = 0;
                  var success = false;
                  var processTime = new Stopwatch();
                  processTime.Start();
                  var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                
                  try
                  {
                    // Try 5 times before giving up
                    while (!success && errCount < 5)
                    {
                      var codePos = "Starting";
                      try
                      {
                        #region Request a DBF
                        _log.Information("[Branch {0}]- Requesting ZIP DBF", branch);
                        codePos = "Requesting DBFs ZIP creation";
                        ProcessStatus status = null;
                        using (var client = new DataSyncDataFileRequestClient())
                        {
                          status = client.StartGetBranchDBFs(SourceRequestUtils.CreateRequest(branch, 0));
                        }

                        if (status == null)
                        {
                          throw new ArgumentNullException("StartGetBranchDBFs returned null");
                        }
                        if (status.Status != ProcessStatus.CurrentStatus.Started && status.Status != ProcessStatus.CurrentStatus.Completed)
                        {
                          throw new Exception("StartGetBranchDBFs could not start");
                        }
                        if (string.IsNullOrEmpty(status.TransactionId))
                        {
                          throw new ArgumentNullException("StartGetBranchDBFs returned empty value for status.TransactionId");
                        }
                        var transactionId = status.TransactionId;
                        #endregion

                        #region Wait for server to complete the ZIPped DBF file creation
                        _log.Information("[Branch {0}]- Waiting for ZIP DBF to be created", branch);
                        codePos = "Waiting for DBF to be created by server";
                        var wait = new Stopwatch();
                        wait.Start();
                        do
                        {
                          System.Threading.Thread.Sleep(5000);
                          using (var client = new DataSyncDataFileRequestClient())
                          {
                            status = client.GetProcessStatus(SourceRequestUtils.CreateRequest(branch, 0), transactionId);
                          }
                        }
                        while (wait.Elapsed < TimeSpan.FromMinutes(MAX_DBF_WAIT_TIME_MINUTES) &&
                          status != null && status.Status == ProcessStatus.CurrentStatus.Started);

                        if (status == null)
                        {
                          throw new ArgumentNullException("GetProcessStatus returned null");
                        }
                        if (status.Status != ProcessStatus.CurrentStatus.Completed)
                        {
                          throw new ArgumentNullException(string.Format("GetProcessStatus returned with {0}", status.Status));
                        }
                        if (string.IsNullOrEmpty(status.Filename))
                        {
                          throw new ArgumentNullException("GetProcessStatus returned empty status.Filename");
                        }
                        var remoteFileName = status.Filename;
                        #endregion

                        #region Download the ZIP file
                        _log.Information("[Branch {0}]- Downloading ZIP DBF", branch);
                        codePos = "Downloading file";
                        long fileSize = 0;
                        using (var client = new DataSyncDataFileClient())
                        {
                          fileSize = client.GetFileSize(SourceRequestUtils.CreateRequest(branch, 0), remoteFileName);

                          if (fileSize <= 0)
                          {
                            throw new ArgumentNullException(string.Format("GetFileSize returned {0}", fileSize));
                          }

                          long currPos = 0;
                          if (File.Exists(tempZip))
                          {
                            var fi = new FileInfo(tempZip);
                            currPos = fi.Length;
                          }
                          using (var destFile = File.Create(tempZip))
                          {
                            while (currPos < fileSize)
                            {
                              var data = client.DownloadFileChunk(SourceRequestUtils.CreateRequest(branch, 0), remoteFileName, currPos, 65000);
                              if (data == null)
                              {
                                throw new Exception("Request returned an empty result");
                              }

                              destFile.Write(data, 0, data.Length);
                              currPos += data.Length;
                            }
                          }
                        }
                        #endregion

                        #region UNZIP the DBF files to HO server
                        _log.Information("[Branch {0}]- Unzipping ZIP DBF", branch);
                        codePos = "Unzipping files";
                        
                        using (var zip = new Ionic.Zip.ZipFile(tempZip))
                        {
                          zip.ExtractAll(tempPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                        }
                        #endregion

                        #region Move files to final path and set permissions
                        var destPath = Path.Combine(dbfDir, branch.Substring(1, 2));
                        Atlas.Services.DbfGenerate.QuartzTasks.Utils.DirUtils.MoveDirectory(tempPath, destPath);

                        var files = Directory.GetFiles(destPath, "*.*");
                        foreach (var file in files)
                        {
                          // Inherit permissions from parent              
                          var fs = File.GetAccessControl(destPath);
                          fs.SetAccessRuleProtection(false, false);
                          File.SetAccessControl(file, fs);
                        }
                        #endregion

                        _log.Information("[Branch {0}]- Successfully completed", branch);
                        success = true;
                      }
                      catch (Exception err)
                      {
                        errCount++;
                        _log.Error(err, "Error- Branch: {0}, '{1}'", branch, codePos);
                      }
                    }
                  }
                  finally
                  {
                    #region Clean-up
                    if (File.Exists(tempZip))
                    {
                      File.Delete(tempZip);
                    }
                    #endregion

                    if (Directory.Exists(tempPath))
                    {
                      Directory.Delete(tempPath, true);
                    }

                    _log.Information("Branch '{0}' took {1}:{2} to process", branch, processTime.Elapsed.Minutes, processTime.Elapsed.Seconds.ToString("D2"));
                  }
                }
              }
            );
          }
          #endregion

          Task.WaitAll(downloadTasks);
        }
        catch (Exception err)
        {
          _log.Error(err, "Execute");
        }

        #region Run fixchq
        try
        {
          _log.Information("Fixchq starting");

          var fixChqResult = 0;
          var daysInMonth = DateTime.DaysInMonth(qdDate.Year, qdDate.Month);
          if (qdDate.Day == daysInMonth) // If doing for end of the month, run the full gamut- reindex, QD generation and month-end
          {
            var monthEnd = new DateTime(qdDate.Year, qdDate.Month, DateTime.DaysInMonth(qdDate.Year, qdDate.Month));
            fixChqResult = RunFixCheque(baseDir, dbfDir, string.Format("QD,{0:yyyyMMdd},MONTHEND", monthEnd), TimeSpan.FromHours(8));
          }
          else if (qdDate.DayOfWeek != DayOfWeek.Sunday) // Mon-Sat: Reindex and Run the QDs
          {
            fixChqResult = RunFixCheque(baseDir, dbfDir, string.Format("QD,{0:yyyyMMdd},REINDEX", qdDate), TimeSpan.FromHours(4));
          }
          else // Sunday and not month-end: Just reindex the DBFs
          {
            fixChqResult = RunFixCheque(baseDir, dbfDir, "R", TimeSpan.FromHours(2));
          }

          if (fixChqResult != 0)
          {
            _log.Error(new Exception(string.Format("The FixChq process returned unexpected exit code: {0}", fixChqResult)), "{ExitCode}", fixChqResult);
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "Fixchq error");
        }
        #endregion
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }

      _log.Information("Execute complete");
    }


    #region Private methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="baseDir"></param>
    /// <param name="srcDir"></param>
    /// <param name="arguments"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    private static int RunFixCheque(string baseDir, string srcDir, string arguments, TimeSpan timeout)
    {
      // Ensure fixchq is in the loanyyMM directory
      var fixChqExeDest = Path.Combine(srcDir, "fixchq.exe");
      if (!File.Exists(fixChqExeDest))
      {
        var dllFiles = Directory.GetFiles(baseDir, "*.dll");
        foreach (var dllFile in dllFiles)
        {
          var destDll = Path.Combine(srcDir, Path.GetFileName(dllFile));
          File.Copy(dllFile, destDll);
        }

        var fixChqExeSrc = Path.Combine(baseDir, "fixchq.exe");
        File.Copy(fixChqExeSrc, fixChqExeDest);
      }

      var fixchqProcess = new Process();
      var startUp = new ProcessStartInfo(fixChqExeDest)
      {
        //CreateNoWindow = true,
        UseShellExecute = false,
        WindowStyle = ProcessWindowStyle.Hidden,
        WorkingDirectory = srcDir,
        Arguments = arguments
      };
      fixchqProcess.StartInfo = startUp;

      _log.Information("Starting fixchq process with {Arguments}", startUp.Arguments);
      if (!fixchqProcess.Start())
      {
        _log.Error(new Exception("Process.Start() failed"), "Failed to start process");
        return -1;
      }

      var timer = Stopwatch.StartNew();
      while (!fixchqProcess.WaitForExit(60000) && timer.Elapsed < timeout)
      {
        _log.Information("Waiting for fixchq: {Elapsed}", timer.Elapsed.Humanize(3));
      }

      if (!fixchqProcess.HasExited)
      {
        _log.Error("Process timed out {Timeout}", timer.Elapsed.Humanize(3));
        fixchqProcess.Kill();
        return -2;
      }

      _log.Information("FixChq completed in: {Elapsed}", timer.Elapsed.Humanize(3));

      return fixchqProcess.ExitCode;
    }

    #endregion


    #region Private vars

    private static readonly ILogger _log = Log.ForContext<DownloadDBFs_Emergency>();

    /// <summary>
    /// Number of tasks to start for downloading of DBFs
    /// </summary>
    private static readonly int DBF_DOWNLOAD_TASKS = 5;
       
    /// <summary>
    /// Maximum number of minutes to wait for DBF zip file generation, before timing out
    /// </summary>
    private static readonly int MAX_DBF_WAIT_TIME_MINUTES = 60;

    #endregion

  }
}
