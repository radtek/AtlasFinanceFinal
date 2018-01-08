/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Routine to convert a branch to its equivalent ASS ZIPed DBFs- use for QD's, auditing, etc.
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
 *     2015-03-06 Simplified with no threading and exporting
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System.Collections.Generic;
using System;
using System.IO;
using System.Data;
using System.Linq;

using Ionic.Zip;
using Npgsql;

using Atlas.Services.DbfGenerate;
using ASSServer.Shared;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using Atlas.Cache.DataUtils;


namespace ASSServer.Utils.PSQL
{
  public class ExportBranchToZipDBF
  {
    /// <summary>
    /// Convert a branch to zipped DBF
    /// </summary>
    /// <param name="legacyBranchNum">The branch number</param>
    /// <param name="transactionId">The process tracking id</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static void Execute(ILogging log, ICacheServer cache, IConfigSettings config, 
      string legacyBranchNum, string transactionId, long dbVersionId)
    {
      var methodName = "ExportBranchToZipDBF.Execute";
      var schemaName = string.Format("br{0}", legacyBranchNum.ToLower());
      var dbVersion = cache.Get<ASS_DbUpdateScript_Cached>(dbVersionId).DbVersion;

      log.Information("{MethodName} {BranchNum}- Starting", methodName, legacyBranchNum);
      try
      {        
        var tempDBFPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDBFPath);
        legacyBranchNum = legacyBranchNum.ToLower();

        var tempZipName = Path.Combine(config.GetCustomSetting("", "DataSyncPath", false), string.Format("{0}.zip", transactionId));
        
        try
        {
          log.Information("{MethodName} {BranchNum}- Exporting master tables", methodName, legacyBranchNum);
          #region Export master tables
          var masterTables = new List<string>();//
          masterTables.AddRange(CacheUtils.GetServerTableNames(cache));
          foreach (var master in masterTables)
          {
            var dbfName = Path.Combine(tempDBFPath, string.Format("{0}.dbf", master));
            TableToDBF.Execute(log, config.GetAssConnectionString(), "company", master, dbfName);
          }
          #endregion

          log.Information("{MethodName} {BranchNum}- Exporting branch tables", methodName, legacyBranchNum);
          #region Export non-master tables
          var tableNames = new List<string>();
          using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              #region Get all standard table names
              cmd.CommandText = string.Format(
                "select \"table_name\" from information_schema.tables " +
                "WHERE \"table_catalog\" = 'ass' and \"table_schema\" = '{0}' AND \"table_type\" = 'BASE TABLE'", schemaName);
              cmd.CommandType = CommandType.Text;
              using (var rdr = cmd.ExecuteReader())
              {
                while (rdr.Read())
                {
                  var tableName = rdr.GetString(0).ToLower();
                  if (!tableName.Contains("lrep_") && !tableName.Contains("sr_mgmnt") && !masterTables.Contains(tableName)) // lrep_ = local replication tables, sr_mgnmt = SQLRDD management tables
                  {
                    tableNames.Add(tableName);
                  }
                }
              }
              #endregion
            }
          }

          foreach(var tableName in tableNames)
          {
            log.Information("{MethodName} {BranchNum}- Exporting table {TableName}", methodName, legacyBranchNum, tableName);
            var dbfName = Path.Combine(tempDBFPath, string.Format("{0}.dbf", tableName));
            TableToDBF.Execute(log, config.GetAssConnectionString(), schemaName, tableName, dbfName);
          }
          #endregion
          
          #region Create VXXXX file
          var tempDBFWithBackslash = tempDBFPath.EndsWith("\\") ? tempDBFPath : string.Format("{0}\\", tempDBFPath);
          var verFilePath = string.Format("{0}{1}", tempDBFWithBackslash, string.Format("V{0}.", dbVersion.Substring(0, dbVersion.Length - 1).ToUpper()));   // Remove last char from file name        
          using (var verFile = File.CreateText(verFilePath))
          {
            verFile.Write(string.Format("Version {0}", dbVersion));
            //verFile.Write("Version 0070A");
          }
          #endregion

          #region Compress DBF to ZIP
          log.Information("{MethodName} {BranchNum}- Zipping DBF files- started- zip file: {TempZipFileName}, source path: {TempDbfPath}", 
            methodName, legacyBranchNum, tempZipName, tempDBFPath);
          using (var zip = new ZipFile(tempZipName))
          {
            zip.AddDirectory(tempDBFPath, "");
            zip.CompressionMethod = CompressionMethod.BZip2;
            zip.CompressionLevel = Ionic.Zlib.CompressionLevel.Level6;
            zip.Save();
          }
          
          log.Information("{MethodName} {BranchNum}- Zipping DBF files- completed: {TempZipFileName}", methodName, legacyBranchNum, tempZipName);
          TempFiles.AddTempFile(tempZipName);
          #endregion

          ASSServer.WCF.ProcessTracking.SetTransactionState(transactionId, ASSServer.WCF.ProcessTracking.CurrentStatus.Completed, null, tempZipName);
        }
        finally
        {
          try
          {
            Directory.Delete(tempDBFPath, true);
          }
          catch (Exception dirDelErr)
          {
            log.Error(dirDelErr, "{MethodName} {BranchNum}", methodName, legacyBranchNum);
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName} {BranchNum}", methodName, legacyBranchNum);
        ASSServer.WCF.ProcessTracking.SetTransactionState(transactionId, ASSServer.WCF.ProcessTracking.CurrentStatus.Failed, err.Message);
      }
    }
  }
}
