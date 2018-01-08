/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*     Quartz task- Generate all branch DBF data  (\\x\Loan\loanYYMM)
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
*     2014-09-11 - Modified to use HO replicated server and create DBFs locally, not rely on WCF service
* 
*    
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using DevExpress.Xpo;
using Humanizer;
using Quartz;
using Serilog;
using Npgsql;

using Atlas.Domain.Model;
using Atlas.Services.DbfGenerate.QuartzTasks.Utils;


namespace Atlas.Services.DbfGenerate.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class GenerateDBFs : IJob
  {
    /// <summary>
    /// Main execution- Create DBFs for all branches
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      var mainTimer = Stopwatch.StartNew();
      _log.Information("Execute starting");
      try
      {
        NpgsqlConnection.ClearAllPools();

        var baseDir = ConfigurationManager.AppSettings["BaseDir"];

        #region Ensure DBF output directory exists
        var dbfDate = DateTime.Now.Subtract(TimeSpan.FromDays(DateTime.Now.Hour >= 20 ? 0 : 1)); // If run after 20:00, we can generate for today, else for yesterday
        var dbfDateString = string.Format("loan{0:yyMM}", dbfDate);
        var dbfDir = Path.Combine(baseDir, dbfDateString);
        if (!Directory.Exists(dbfDir))
        {
          Directory.CreateDirectory(dbfDir);

          // Inherit permissions from parent
          //var da = Directory.GetParent(dbfDir).GetAccessControl();
          //da.SetAccessRuleProtection(false, false);
          //destDir.SetAccessControl(da);
        }

        var qdDate = dbfDate.Subtract(TimeSpan.FromDays(7));
        #endregion

        #region Get brXXX schemas and use as branch list
        var currBranches = new List<string>();
        var assConnectionString = ConnStrings.Ass;
        string dbVersion = null;
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

        #region Get master tables, current ASS db version
        using (var unitOfWork = new UnitOfWork())
        {
          dbVersion = unitOfWork.Query<ASS_DbUpdateScript>().Max(s => s.DbVersion);
          masterTables = unitOfWork.Query<ASS_ServerTable>().Select(s => s.TableName).ToList();
        }
        #endregion

        //#if ertt
        #region Generate master table DBFs
        var masterDbfTimer = Stopwatch.StartNew();
        var masterRootPath = Path.Combine(dbfDir, "master_tables");
        try
        {
          _log.Information("Generating master DBFs {Tables}", masterTables.Count);

          if (Directory.Exists(masterRootPath))
          {
            var files = Directory.EnumerateFiles(masterRootPath, "*.dbf");
            foreach (var file in files)
            {
              File.Delete(file);
            }
          }
          else
          {
            Directory.CreateDirectory(masterRootPath);
          }

          foreach (var masterTable in masterTables)
          {
            var destDBFFilePath = Path.Combine(masterRootPath, string.Format("{0}.dbf", masterTable));
            TableToDBF.Execute(_log, assConnectionString, "company", masterTable, destDBFFilePath);
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "Execute- Master DBF generate");
        }

        _log.Information("Master DBF generation completed in {Elapsed}", masterDbfTimer.Elapsed.Humanize(3));
        #endregion

        #region Generate branch DBFs
        var branchDbfTimer = Stopwatch.StartNew();
        try
        {
          _log.Information("Generate branch DBFs starting");
          var doneCount = 0;
          var branchToDo = new ConcurrentQueue<string>(currBranches);

          #region Create tasks
          var generateTasks = new Task[DBF_BRANCH_CONCURRENT_TASKS];
          for (var i = 0; i < generateTasks.Length; i++)
          {
            generateTasks[i] = Task.Factory.StartNew(() =>
              {
                string branch;
                while (branchToDo.TryDequeue(out branch))
                {
                  try
                  {
                    _log.Information("Starting: [{branch}]...", branch);
                    var destPath = Path.Combine(dbfDir, branch.Substring(1, 2));
                    ExportBranchToDBF.Execute(_log, branch, destPath, dbVersion, assConnectionString, masterRootPath, masterTables);
                    Interlocked.Increment(ref doneCount);
                    _log.Information("Completed: [{branch}] - {BranchCount}", branch, doneCount);
                  }
                  catch (Exception err)
                  {
                    _log.Error(err, "Handle branch: {Branch}", branch);
                  }
                }
              }
            );
          }
          #endregion

          Task.WaitAll(generateTasks);
        }
        catch (Exception err)
        {
          _log.Error(err, "Execute- Branch DBF generate");
          return;
        }
        _log.Information("Branch DBF generation completed in {Elapsed}", branchDbfTimer.Elapsed.Humanize(3));
        #endregion

        // TODO:
        // ---------------
        // Keep track of last successful fixchq run and keep running QDs till we catch up (without reindex)
        // Check errorcode == 0
        //#endif

        // Create blank template branch, using branch "SourceBranch"/"33" as source for non-master tables
        CreateTemplateBranch.Execute(_log, masterTables, dbfDir, ConfigurationManager.AppSettings["SourceBranch"] ?? "33", Path.Combine(dbfDir, "branch_template"));

        #region Run fixchq
        try
        {
          _log.Information("Fixchq starting");

          var fixChqResult = 0;
          var daysInMonth = DateTime.DaysInMonth(dbfDate.Year, dbfDate.Month);
          if (dbfDate.Day == daysInMonth) // If doing for end of the month, run the full gamut- reindex, QD generation and month-end
          {
            var monthEnd = new DateTime(dbfDate.Year, dbfDate.Month, DateTime.DaysInMonth(dbfDate.Year, dbfDate.Month));
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

      _log.Information("Execute complete in {Elapsed}", mainTimer.Elapsed.Humanize(3));
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

    private static readonly ILogger _log = Log.ForContext<GenerateDBFs>();

    /// <summary>
    /// Number of tasks to start for generation of DBFs
    /// </summary>
    private static readonly int DBF_BRANCH_CONCURRENT_TASKS = 3;

    #endregion

  }
}
