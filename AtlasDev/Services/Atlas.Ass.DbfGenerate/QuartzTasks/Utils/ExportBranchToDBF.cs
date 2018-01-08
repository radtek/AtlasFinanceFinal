/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Routine to convert a branch to its equivalent DBFs- use for QD's, auditing, etc.
 *     Indexes need to be re-created with ASS.
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
 *     2014-09-11 Converted to local PSQL DB conversion (no zips)
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System.Collections.Generic;
using System;
using System.IO;

using System.Threading;

using Serilog;
using Npgsql;
using Humanizer;


namespace Atlas.Services.DbfGenerate.QuartzTasks.Utils
{
  public class ExportBranchToDBF
  {
    /// <summary>
    /// Convert a branch PSQL to DBFs (excludes master tables specified in masterFilePathName)
    /// </summary>
    /// <param name="legacyBranchNum">The branch number</param>    
    /// <param name="dbVersion">The database version</param>
    /// <param name="destPath">The destination path</param>
    /// <param name="assConnectionString">The connection string to the ASS database</param>
    /// <param name="threadCount">Max. threads (concurrent DBF generation) to use</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static void Execute(ILogger log, string legacyBranchNum, string destPath, string dbVersion, string assConnectionString,
      string masterFilePathName, List<string> masterTableNames)
    {
      var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
      var isError = new ManualResetEventSlim(false);
      var timer = System.Diagnostics.Stopwatch.StartNew();
      try
      {
        try
        {
          Directory.CreateDirectory(tempPath);

          legacyBranchNum = legacyBranchNum.ToLower().PadLeft(3, '0');
          var tables = new List<string>();

          #region Get non-master tables
          using (var conn = new NpgsqlConnection(assConnectionString))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandText = string.Format(
                "select \"table_name\" from information_schema.tables " +
                "WHERE \"table_catalog\" = 'ass' and \"table_schema\" = 'br{0}' AND \"table_type\" = 'BASE TABLE'", legacyBranchNum);
              cmd.CommandType = System.Data.CommandType.Text;
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  var tableName = rdr.GetString(0).ToLower();
                  if (!masterTableNames.Contains(tableName) &&
                    !tableName.Contains("lrep_") && !tableName.Contains("sr_mgmnt")) // lrep_ = local replication tables, sr_mgnmt = SQLRDD management tables
                  {
                    tables.Add(tableName);
                  }
                }
              }
            }
          }
          #endregion

          #region Export non-master tables
          foreach (var tableName in tables)
          {
            var destDBF = Path.Combine(tempPath, string.Format("{0}.dbf", tableName));
            TableToDBF.Execute(log, assConnectionString, string.Format("br{0}", legacyBranchNum), tableName, destDBF);
          }
          #endregion

          #region Copy master tables
          foreach (var tableName in masterTableNames)
          {
            var sourceFilename = Path.Combine(masterFilePathName, string.Format("{0}.dbf", tableName));
            var destFilename = Path.Combine(tempPath, string.Format("{0}.dbf", tableName));
            File.Copy(sourceFilename, destFilename);
          }
          #endregion

          #region Create VXXXX file
          var tempDBFWithBackslash = string.Format("{0}{1}", tempPath, tempPath.EndsWith("\\") ? string.Empty : "\\");
          var verFilePath = string.Format("{0}{1}", tempDBFWithBackslash, string.Format("V{0}.",
            dbVersion.Substring(0, dbVersion.Length - 1).ToUpper()));   // Remove last char from file name        
          using (var verFile = File.CreateText(verFilePath))
          {
            verFile.Write(string.Format("Version {0}", dbVersion));
          }
          #endregion
        }
        catch (Exception err)
        {
          log.Error(err, "Execute");
          isError.Set();
        }
      }
      finally
      {
        try
        {
          if (!isError.IsSet)
          {
            DirUtils.MoveDirectory(tempPath, destPath);

            var files = Directory.GetFiles(destPath, "*.*");
            foreach (var file in files)
            {
              // Inherit permissions from parent              
              var fs = File.GetAccessControl(destPath);
              fs.SetAccessRuleProtection(false, false);
              File.SetAccessControl(file, fs);
            }
          }
          else
          {
            Directory.Delete(tempPath, true);
          }
        }
        catch (Exception err)
        {
          log.Error(err, "Failed to move to temporary directory to destination");
        }
      }

      log.Information("Branch DBFs {Branch} completed in {ElapsedTime}", legacyBranchNum, timer.Elapsed.Humanize(3));
    }

  }
}