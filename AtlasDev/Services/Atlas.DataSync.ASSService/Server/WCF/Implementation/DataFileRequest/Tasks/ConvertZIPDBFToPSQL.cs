/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Orchestrates the conversion of zipped DBFs to its SQLRDD PostgreSQL schemas:
 *        1. Own schema (brXXX), without sequences and triggers & PK, with SR_ tables
 *           
 *            This is used for recovery (pg_dump) / branch extract purposes (DBF ZIP)
 *            It should be perfectly in sync with the branch data, if comms up with branch and sync server
 *            running            
 * 
 *        2. To company, without sequences, triggers and SR_ tables, but with indexes & PK
 *     
 *            This is for company reporting
 *            It should be perfectly in sync with the branch data, if comms up for all branches and sync server 
 *            running
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
 *     2013-09-13 Parallelize all long-running tasks
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using DevExpress.Xpo;
using Ionic.Zip;

using ASSServer.Utils.DBF;
using ASSServer.Utils.PSQL;
using ASSServer.Extensions;
using Atlas.Domain.Model;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using ASSServer.Utils.PSQL.DbfImport;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.DomainMapper;


namespace ASSServer.WCF.Implementation.DataFileRequest.Tasks
{
  public class ConvertZIPDBFToPSQL
  {
    #region Public methods

    /// <summary>
    /// Orchestrates the conversion from ZIPPed DBFs to server PostgreSQL
    /// </summary>
    /// <param name="legacyBranchNum">Branch number</param>
    /// <param name="transactionId">Client transaction ID- used by client to track progress</param>
    public static void Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      ASS_BranchServer_Cached server, string legacyBranchNum, string transactionId)
    {
      var methodName = "ConvertZIPDBFToPSQL.Execute";
      var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
      var schemaName = string.Format("br{0}", legacyBranchNum.Trim().PadLeft(3, '0').ToLower());
      var taskMessages = new List<string>();
      var canDeleteSchema = false;
      var processTimer = Stopwatch.StartNew();
      try
      {
        log.Information("{MethodName} starting, {@BranchServer}, {Branch}, {TransactionId}", methodName, server, legacyBranchNum, transactionId);

        try
        {
          string errorMessage;

          #region Handle the ZIP file
          try
          {
            string fileName;
            ProcessTracking.CurrentStatus status;
            if (!ProcessTracking.GetTransactionState(transactionId, out status, out errorMessage, out fileName))
            {
              errorMessage = string.Format("Failed to retrieve session: '{0}'", transactionId);
              log.Error(methodName, new Exception(errorMessage));
              ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
              return;
            }

            var fullFileName = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), fileName);
            if (!File.Exists(fullFileName))
            {
              errorMessage = string.Format("File missing: '{0}'", fileName);
              log.Error(methodName, new Exception(errorMessage));

              ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
              return;
            }

            #region Unpack the ZIP
            log.Information("{0}- Unzipping file {1}", legacyBranchNum, fileName);
            using (var zip = new ZipFile(fullFileName))
            {
              //zip.ExtractProgress += OnZipProgress;
              zip.ExtractAll(tempDir);
            }
            log.Information("{0}- Successfully unzipped file {1}", legacyBranchNum, fileName);

            var fileCount = Directory.GetFiles(tempDir).Length;
            if (fileCount < 100)
            {
              errorMessage = string.Format("Zip file '{0}' only contained {1} files", fileName, fileCount);
              log.Error(methodName, new Exception(errorMessage));

              ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
              return;
            }
            #endregion
          }
          catch (Exception unzipErr)
          {
            errorMessage = string.Format("Unable to unzip files: '{0}'", unzipErr.Message);
            log.Error(methodName, new Exception(errorMessage));

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }
          #endregion

          #region Get DB version from DBF files
          log.Information("{0}- Determining DB version", legacyBranchNum);
          string dbVersion;
          if (!GetAssVer.Execute(tempDir, out dbVersion, out errorMessage))
          {
            errorMessage = string.Format("Unable to determine DB version: '{0}'", errorMessage);
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);

            log.Error(new Exception(errorMessage), methodName);
            return;
          }
          log.Information("{0}- DB version: {1}", legacyBranchNum, dbVersion);
          #endregion

          #region Ensure no data currently in existing PSQL schema
          log.Information("{0}- checking no data in schema: {1}", legacyBranchNum, schemaName);
          if (!CheckExisting.Execute(config, schemaName, taskMessages))
          {
            errorMessage = "The branch Schema already exists and contains some data- cannot overwrite without manual intervention";
            log.Error(methodName, new Exception(errorMessage));

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }
          DisplayAndClear(log, taskMessages);
          canDeleteSchema = true;
          #endregion

          #region Create the branch schema
          log.Information("{0}- Creating branch schema", legacyBranchNum);
          if (!CreateSchema.Execute(config, schemaName, taskMessages))
          {
            errorMessage = string.Format("Failed to create schema: '{0}'", errorMessage);
            log.Error(methodName, new Exception(errorMessage));

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }
          DisplayAndClear(log, taskMessages);
          #endregion

          #region Build prioritized listing of DBF files to process
          log.Information("{0}- Building DBF listing", legacyBranchNum);
          var allDbfFilesToProcess = Directory.GetFiles(tempDir, "*.DBF").ToList();
          var dbfFilesToProcess = new List<string>();
          foreach (var dbfFile in allDbfFilesToProcess)
          {
            var fileNameOnly = Path.GetFileNameWithoutExtension(dbfFile).ToLower();
            if (fileNameOnly != "naedo" && fileNameOnly != "help" &&
              !fileNameOnly.StartsWith("wk") && !fileNameOnly.StartsWith("a1") && !fileNameOnly.StartsWith("wr") &&
              !fileNameOnly.StartsWith("xx") && !fileNameOnly.StartsWith("bu0"))
            {
              dbfFilesToProcess.Add(dbfFile.ToLower());
            }
          }

          // Prioritize the largest files... these should be done first as they take the longest
          dbfFilesToProcess.MoveItem(Path.Combine(tempDir, "trans.dbf"), 0);
          dbfFilesToProcess.MoveItem(Path.Combine(tempDir, "loans.dbf"), 1);
          dbfFilesToProcess.MoveItem(Path.Combine(tempDir, "cbtrans.dbf"), 2);
          dbfFilesToProcess.MoveItem(Path.Combine(tempDir, "claud"), 3);
          #endregion

          var errorSet = new ManualResetEventSlim(false);

          #region Try fix corrupt DBF bytes
          // Task processing queue
          log.Information("{0}- Checking for invalid bytes", legacyBranchNum);
          var pendingFiles = new ConcurrentQueue<string>(dbfFilesToProcess);
          var taskProgressMessages = new ConcurrentQueue<string>();
          var tryFixDbfTasks = new Task[4];
          for (var i = 0; i < tryFixDbfTasks.Length; i++)
          {
            tryFixDbfTasks[i] = Task.Factory.StartNew(() =>
            {
              string dbfFileName;
              while (!errorSet.IsSet && pendingFiles.TryDequeue(out dbfFileName))
              {
                if (!RepairDBFBytes.Execute(dbfFileName, taskProgressMessages))
                {
                  errorSet.Set();
                }
              }
            });
          }
          if (!WaitForTaskAndDisplay(log, tryFixDbfTasks, taskProgressMessages, TimeSpan.FromMinutes(5), out errorMessage))
          {
            return;
          }

          if (errorSet.IsSet)
          {
            errorMessage = string.Format("Error trying to fix DBFs: '{0}'", errorMessage);
            log.Error(methodName, new Exception(errorMessage));

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }
          #endregion

          #region Check we can read all rows in the DBFs
          log.Information("{0}- Checking DBFs readable", legacyBranchNum);
          pendingFiles = new ConcurrentQueue<string>(dbfFilesToProcess);
          var checkDBFTasks = new Task[4];
          for (var i = 0; i < checkDBFTasks.Length; i++)
          {
            checkDBFTasks[i] = Task.Factory.StartNew(() =>
            {
              string dbfFileName;
              while (!errorSet.IsSet && pendingFiles.TryDequeue(out dbfFileName))
              {
                if (!CheckCanReadData.Execute(dbfFileName, taskProgressMessages))
                {
                  errorSet.Set();
                }
              }
            });
          }
          if (!WaitForTaskAndDisplay(log, checkDBFTasks, taskProgressMessages, TimeSpan.FromMinutes(5), out errorMessage))
          {
            errorMessage = string.Format("Error trying to check DBF readable: '{0}'", errorMessage);
            log.Error(methodName, new Exception(errorMessage));

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }

          if (errorSet.IsSet)
          {
            errorMessage = string.Format("Error trying to check DBF readable: '{0}'", errorMessage);
            log.Error(methodName, new Exception(errorMessage));

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }
          #endregion

          #region DBF structure conversion via xHarbour console application
          log.Information("{0}- Converting structure via xHarbour", legacyBranchNum);
          if (!ConvertStruc.Execute(tempDir, config.GetCustomSetting("", "DBServer", false),
            config.GetCustomSetting("", "DBUserName", false), config.GetCustomSetting("", "DBPassword", false), schemaName, taskMessages))
          {
            DisplayAndClear(log, taskMessages);
            errorMessage = string.Format("Failed to convert structure: '{0}'", string.Join(",", taskMessages));
            log.Error(new Exception(errorMessage), methodName);

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }
          DisplayAndClear(log, taskMessages);
          #endregion

          #region Create PK's for each table on sr_recno
          log.Information("{0}- Creating PKs on sr_recno", legacyBranchNum);
          if (!AddPrimaryKey.Execute(config, schemaName, taskMessages))
          {
            DisplayAndClear(log, taskMessages);
            errorMessage = string.Format("Failed to add primary keys: '{0}'", string.Join(",", taskMessages));
            log.Error(new Exception(errorMessage), methodName);

            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            return;
          }
          DisplayAndClear(log, taskMessages);
          #endregion

          #region Import the data
          log.Information("{0}- Copy data starting", legacyBranchNum);
          pendingFiles = new ConcurrentQueue<string>(dbfFilesToProcess);
          var copyDBFData = new Task[4];
          for (var i = 0; i < copyDBFData.Length; i++)
          {
            copyDBFData[i] = Task.Factory.StartNew(() =>
            {
              string dbfFileName;
              while (!errorSet.IsSet && pendingFiles.TryDequeue(out dbfFileName))
              {
                if (!CopyDataToPSQL.Execute(config, dbfFileName, schemaName, taskProgressMessages))
                {
                  errorSet.Set();
                }
              }
            });
          }
          if (!WaitForTaskAndDisplay(log, copyDBFData, taskProgressMessages, TimeSpan.FromMinutes(60), out errorMessage))
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, string.Format("Error while copying DBF data: '{0}'", errorMessage));
            log.Error(new Exception(errorMessage), methodName);

            return;
          }

          if (errorSet.IsSet)
          {
            errorMessage = "Error copying data";
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, string.Format("Error while copying DBF data: '{0}'", errorMessage));
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          #endregion

          #region Synthetic index column data population
          log.Information("{0}- Synthetic index creation starting", legacyBranchNum);

          var orderedTables = new List<string>(dbfFilesToProcess.Select(s => Path.GetFileNameWithoutExtension(s).ToLower()));

          pendingFiles = new ConcurrentQueue<string>(orderedTables);
          var syntheticIndexTasks = new Task[8];
          for (var i = 0; i < syntheticIndexTasks.Length; i++)
          {
            syntheticIndexTasks[i] = Task.Factory.StartNew(() =>
            {
              string tableName;
              while (!errorSet.IsSet && pendingFiles.TryDequeue(out tableName))
              {
                if (!PopulateSyntheticCols.Execute(config, tableName, schemaName, taskProgressMessages))
                {
                  errorSet.Set();
                }
              }
            });
          }
          if (!WaitForTaskAndDisplay(log, syntheticIndexTasks, taskProgressMessages, TimeSpan.FromMinutes(45), out errorMessage))
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, string.Format("Error while creating synthetic data: '{0}'", errorMessage));
            log.Error(new Exception(errorMessage), methodName);
            return;
          }

          if (errorSet.IsSet)
          {

            errorMessage = "Error creating synthetic data";
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          #endregion

          #region Remove sequences from the tables
          log.Information("{0}- Remove sequences starting", legacyBranchNum);
          if (!RemoveBranchTableSequences.Execute(config, schemaName, taskMessages))
          {
            DisplayAndClear(log, taskMessages);
            errorMessage = string.Format("Failed to remove sequences: '{0}'", string.Join(",", taskMessages));
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          DisplayAndClear(log, taskMessages);
          log.Information("{0}- Remove sequences completed", legacyBranchNum);
          #endregion

          #region Set branch version lrep_db.. and Atlas core domain version...
          log.Information("{0}- Setting branch version and master replication values", legacyBranchNum);
          if (!SetBranchDBReplicationStatus.Execute(config, cache, server.BranchServerId, dbVersion.Trim(), out errorMessage))
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);
          }

          #endregion

          #region Update branch DB structure to latest version
          log.Information("{0}- Update branch schema to latest version", legacyBranchNum);
          if (!UpdateBranchSchemaToLatest.Execute(config, cache, schemaName, out dbVersion, out errorMessage))
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          DisplayAndClear(log, taskMessages);
          log.Information("{0}- Successfully updated branch schema to latest version", legacyBranchNum);
          #endregion

          #region Copy across 'branch' table data into equivalent 'company' tables
          log.Information("{0}- Merge data into 'company' schema", legacyBranchNum);
          var copyBranchDataToCompanyResult = false;
          var copyBranchDataToCompanyTask = Task.Factory.StartNew(() =>
            {
              copyBranchDataToCompanyResult = CopyBranchDataToCompany.Execute(cache, config, legacyBranchNum, taskProgressMessages);
            });
          if (!WaitForTaskAndDisplay(log, new Task[] { copyBranchDataToCompanyTask }, taskProgressMessages, TimeSpan.FromMinutes(90), out errorMessage))
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          if (!copyBranchDataToCompanyResult)
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          #endregion

          #region Copy across master tables and indexes from 'company' into the 'brXXX' schema
          var copyMasterToBranchResult = false;
          var copyMasterToBranchTask = Task.Factory.StartNew(() =>
            {
              copyMasterToBranchResult = CopyMasterTablesToBranch.Execute(cache, config, schemaName, taskProgressMessages);
            }
          );
          if (!WaitForTaskAndDisplay(log, new Task[] { copyMasterToBranchTask }, taskProgressMessages, TimeSpan.FromMinutes(10), out errorMessage))
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          if (!copyMasterToBranchResult)
          {
            ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, errorMessage);
            log.Error(new Exception(errorMessage), methodName);

            return;
          }
          #endregion
          canDeleteSchema = false;

          /*
          #region Vacuum Analyze 'company' schema
          var analyzeCompany = Task.Factory.StartNew(() => { CleanNewTables.Execute("company", taskProgressMessages); });
          WaitForTaskAndDisplay(new Task[] { analyzeCompany }, taskProgressMessages, TimeSpan.FromMinutes(30), out errorMessage);
          #endregion
          */

          // Update ASS_BranchServer.UploadedDBDT with current time          
          using (var unitOfWork = new UnitOfWork())
          {
            var serverDb = unitOfWork.Query<ASS_BranchServer>().First(s => s.BranchServerId == server.BranchServerId);
            serverDb.UploadedDBDT = DateTime.Now;
            unitOfWork.CommitChanges();

            var cachedServer = CacheDomainMapper.ASS_BranchServer_Mapper(serverDb);
            cache.Set(new List<ASS_BranchServer_Cached> { cachedServer });
          }          

          log.Information("{Branch}- dBASE to SQL conversion process completed successfully in {ConversionElapsedMS}",
            legacyBranchNum, processTimer.Elapsed.ToString(@"hh\:mm\:ss"));

          ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Completed, null, null);
        }
        catch (Exception err)
        {
          log.Error(err, methodName);
          ProcessTracking.SetTransactionState(transactionId, ProcessTracking.CurrentStatus.Failed, string.Format("General error: '{0}'", err.Message));
          DropSchemaCascade.Execute(config, schemaName);
        }
      }
      finally
      {
        #region Clean up
        if (Directory.Exists(tempDir))
        {
          try
          {
            Directory.Delete(tempDir, true);
          }
          catch { }
        }

        // If process failed- delete the schema
        ProcessTracking.CurrentStatus status;
        string errorMessage;
        string fileName;
        if (ProcessTracking.GetTransactionState(transactionId, out status, out errorMessage, out fileName) &&
          status == ProcessTracking.CurrentStatus.Failed && canDeleteSchema)
        {
          DropSchemaCascade.Execute(config, schemaName);
        }
        #endregion
      }
    }
    
    #endregion

    #region Private methods

    /// <summary>
    /// Waits for task
    /// </summary>
    /// <param name="tasks"></param>
    /// <param name="progress"></param>
    /// <param name="timeout"></param>
    /// <param name="lastMessage"></param>
    /// <returns></returns>
    private static bool WaitForTaskAndDisplay(ILogging log, Task[] tasks, ConcurrentQueue<string> progress, TimeSpan timeout, out string lastMessage)
    {
      var result = false;
      lastMessage = null;

      var timer = new Stopwatch();
      timer.Start();
      while (timer.Elapsed < timeout && !result)
      {
        result = Task.WaitAll(tasks, 1000);
        while (progress.TryDequeue(out lastMessage))
        {
          log.Information(lastMessage);
        }
      }

      while (progress.TryDequeue(out lastMessage))
      {
        log.Information(lastMessage);
      }

      return result;
    }

    
    private static void DisplayAndClear(ILogging log, List<string> progress)
    {      
      foreach (var item in progress)
      {
        log.Information(item);
      }
      progress.Clear();
    }

    #endregion

  }
}
