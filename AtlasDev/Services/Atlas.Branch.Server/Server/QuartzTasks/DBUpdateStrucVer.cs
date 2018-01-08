/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Quartz task to handle updating local DB server structure (via SQL DDL), to expected version
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     13 June 2013 - Created
 *  
 *     17 July 2013- First version completed
 * 
 * 
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Data;

using Quartz;
using Npgsql;
using Serilog;

using ASSSyncClient.Utils.Settings;
using Atlas.DataSync.WCF.Client.ClientProxies;
using Atlas.DataSync.WCF.Interface;

using ASSSyncClient.Utils;


namespace ASSSyncClient.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class DBUpdateStrucVer : global::Quartz.IJob
  {
    #region Public methods

    /// <summary>
    /// Run SQL DDL updates to match server db
    /// </summary>
    /// <param name="context"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "DBUpdateStrucVer.Execute";
      try
      {
        _log.Information("{MethodName} starting", methodName);

        // Ping server before proceeding
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(10), sendTimeout: TimeSpan.FromSeconds(15)))
        {
          client.GetServerDateTime();
        }

        // Current version of local SQL
        string dbCurrVersion = null;

        #region Get our current DB version
        _log.Information("{MethodName} Getting current local database version", methodName);
        using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
        {
          conn.Open();

          using (var rdr = conn.CreateCommand())
          {
            rdr.CommandText = "SELECT \"value\" from \"lrep_db_info\" WHERE \"setting\" = 1 LIMIT 1";
            rdr.CommandType = CommandType.Text;
            dbCurrVersion = (string)rdr.ExecuteScalar();
          }
        }
        if (string.IsNullOrEmpty(dbCurrVersion))
        {
          var errorString = "Local database version could not be determined- record missing";
          _log.Error(errorString, "{MethodName}", methodName);
          LogEvents.Log(DateTime.Now, "DBUpdateStrucVer.Execute", errorString, 5);

          return;
        }

        _log.Information("{MethodName} Local database version is {CurrVersion}", methodName, dbCurrVersion);
        #endregion

        #region Retrieve update DDL scripts from the server
        _log.Information("{MethodName} Getting DDL/SQL scripts from server...", methodName);
        List<VerUpdateScripts> scripts = null;
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(20), sendTimeout: TimeSpan.FromSeconds(40)))
        {
          scripts = client.GetDbUpdateScript(ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest(), dbCurrVersion);
        }

        #endregion

        #region Execute DDL scripts to update local structure
        if (scripts != null && scripts.Count > 0)
        {
          // DB version we have updated to
          string dbNewVersion = null;

          _log.Information("{MethodName} Server returned updated script count of: {ScriptCount}, with latest version: {LatestVersion}- executing now", 
            methodName, scripts.Count, scripts[scripts.Count - 1].Version);

          using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
          {
            conn.Open();
            var transaction = conn.BeginTransaction();

            string currScriptVer = null;
            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandType = CommandType.Text;
              cmd.Transaction = transaction;

              try
              {
                #region Execute the update scripts
                foreach (var script in scripts.OrderBy(s => s.Version))
                {
                  // Store version for error logging
                  currScriptVer = script.Version;
                  cmd.CommandTimeout = (int)TimeSpan.FromMinutes(60).TotalSeconds; // Update scripts can take some time...!
                  cmd.CommandText = script.SQLScript;
                  cmd.ExecuteNonQuery();
                  dbNewVersion = script.Version;
                }
                #endregion

                #region Mark version we are now on in lrep_db_info
                cmd.CommandText = string.Format("UPDATE \"lrep_db_info\" SET \"value\" = '{0}' WHERE \"setting\" = 1", dbNewVersion);
                cmd.ExecuteNonQuery();
                #endregion

                transaction.Commit();
                dbCurrVersion = dbNewVersion;
                _log.Information("Local system successfully updated to version: {NewVersion}", methodName, dbNewVersion);
              }
              catch (Exception err)
              {
                _log.Error(err, string.Format("Execute script- Ver: '{0}'", methodName, currScriptVer));
                transaction.Rollback();
              }
            }
          }
        }
        else
        {
          _log.Information("{MethodName} Server returned no update scripts- local system is up-to-date", methodName);
        }
        #endregion

        #region Notify server of version we are running (we do this irrespective of changes or not- ensures data match)
        var success = false;
        var attemptCounts = 0;
        while (!success && attemptCounts++ < 5)
        {
          try
          {
            using (var client = new DataSyncDataClient())
            {
              success = client.SetBranchDatabaseVersion(ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest(), dbCurrVersion);
            }
          }
          catch (Exception err)
          {
            _log.Error(err, "{MethodName} Notify server error", methodName);
            Thread.Sleep(5000);
          }
        }
        #endregion
      }
      catch (Exception err)
      {
        LogEvents.Log(DateTime.Now, methodName, err.Message, 5);
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName} completed", methodName);
    }

    #endregion


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<DBUpdateStrucVer>();

    #endregion

  }
}
