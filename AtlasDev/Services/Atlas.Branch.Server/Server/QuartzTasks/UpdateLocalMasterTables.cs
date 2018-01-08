/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Handles updating local live lookup tables -> 'ASSTMAST'
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
 *     15 July 2013-  Fleshed out 
 * 
 *     17 July 2013-  Initial version complete
 *     
 * 
 *  Comments:    
 *  ------------------
 *     The first field in the DataSet is treated as the primary key/seek field and it must be a string
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Data;
using System.Linq;
using ASSSyncClient.Utils;
using ASSSyncClient.Utils.Settings;
using Atlas.Data.Utils;
using Atlas.DataSync.WCF.Client.ClientProxies;
using Atlas.DataSync.WCF.Interface;
using Atlas.Utils.Serialization;
using Npgsql;
using Quartz;
using Serilog;
using ASSSyncClient.Utils.WCF;


namespace ASSSyncClient.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class UpdateLocalMasterTables : global::Quartz.IJob
  {
    /// <summary>
    /// Handles updating local live lookup tables
    /// </summary>
    /// <param name="context"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "UpdateLocalMasterTables.Execute";
      try
      {
        // Ping server before proceeding
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(10), sendTimeout: TimeSpan.FromSeconds(15)))
        {
          client.GetServerDateTime();
        }

        string lastUpdatecRecId = null;
        string localDbVersion = null;

        #region Get local DB version and last recId we processed locally
        using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
        {
          conn.Open();
          using (var cmd = conn.CreateCommand())
          {
            // Get local database version
            cmd.CommandText = "SELECT \"value\" FROM lrep_db_info WHERE setting = 1 LIMIT 1";
            cmd.CommandType = CommandType.Text;
            var value = cmd.ExecuteScalar();
            if (value == null)
            {
              var err = "lrep_db_info version information missing- no version information entry";
              _log.Error(new Exception(err), "{MethodName}", methodName);
              LogEvents.Log(DateTime.Now, "UpdateLocalMasterTables.Execute", err, 5);
              return;
            }
            localDbVersion = (string)value;

            // Get last master recId we successfully processed locally
            cmd.CommandText = "SELECT \"value\" FROM \"lrep_db_info\" WHERE \"setting\" = 2 LIMIT 1";
            cmd.CommandType = CommandType.Text;
            value = cmd.ExecuteScalar();
            if (value == null)
            {
              var err = "lrep_db_info master tables recid missing";
              _log.Error(new Exception(err), "{MethodName}", methodName);
              LogEvents.Log(DateTime.Now, "UpdateLocalMasterTables.Execute", err, 5);
              return;
            }
            lastUpdatecRecId = (string)value;
          }
        }

        var lastRecId = 0L;
        if (!string.IsNullOrEmpty(lastUpdatecRecId))
        {
          if (!Int64.TryParse(lastUpdatecRecId, out lastRecId))
          {
            var err = string.Format("lrep_db_info master tables recid contains an invalid value: '{0}'", lastUpdatecRecId);
            _log.Error(new Exception(err), "{MethodName}", methodName);
            LogEvents.Log(DateTime.Now, "UpdateLocalMasterTables.Execute", err, 5);
            return;
          }
        }
        #endregion

        #region Ensure we match database versions
        string serverDbVersion = null;
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(20), sendTimeout: TimeSpan.FromSeconds(30)))
        {
          serverDbVersion = client.GetServerDatabaseVersion(SyncSourceRequest.CreateSourceRequest());
        }

        if (serverDbVersion == null)
        {
          var err = "GetServerDatabaseVersion returned an empty result";
          _log.Error(new Exception(err), "{MethodName}", methodName);
          LogEvents.Log(DateTime.Now, "UpdateLocalMasterTables.Execute", err, 5);
          return;
        }

        if (localDbVersion != serverDbVersion)
        {
          var err = string.Format("Database version mismatch: Local DB version: {0}, Server DB version: {1}", localDbVersion, serverDbVersion);
          _log.Error(new Exception(err), "{MethodName}", methodName);
          LogEvents.Log(DateTime.Now, "UpdateLocalMasterTables.Execute", err, 5);
          return;
        }
        #endregion

        #region Get master table updates since our last request
        MasterTableRowChanges changes = null;
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(20), sendTimeout: TimeSpan.FromSeconds(40)))
        {
          var request = SyncSourceRequest.CreateSourceRequest();
          //request.AppVer = "1.2.0.0"; // Inform server to use new fastJson DataSet format (*much* faster/smaller)
          changes = client.GetMasterRowChangesSince(request, localDbVersion, lastRecId);
        }

        if (changes == null || changes.DataSet == null || changes.DataSet.Length < 10 || changes.ServerLastRecId == 0)
        {
          return;
        }
        #endregion

        var dataSet = ObjectToBytes.DeserializeFromBytesJson<DataSet>(changes.DataSet, true);
        if (dataSet != null && dataSet.Tables != null && dataSet.Tables.Count > 0)
        {
          using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
          {
            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
              using (var cmd = conn.CreateCommand())
              {
                cmd.Transaction = transaction;

                foreach (DataTable table in dataSet.Tables)
                {
                  var dataSetRecIdCol = table.Columns.IndexOf("sr_recno");
                  if (!string.IsNullOrEmpty(table.TableName) && dataSetRecIdCol > -1)
                  {
                    if (table.Columns.Count > 4)
                    {
                      var fullTableName = string.Format("\"{0}\"", table.TableName.ToLower());

                      // Build insert field names SQL statement
                      var insertSQLFieldNames = string.Join(",", table.Columns.Cast<DataColumn>().Select(s => string.Format("\"{0}\"", s.ColumnName)));

                      #region Convert DataSet to PostgreSQL insert/update
                      for (var i = 0; i < table.Rows.Count; i++)
                      {
                        var currRow = table.Rows[i];

                        cmd.CommandText = string.Format("SELECT COUNT(*) FROM {0} WHERE \"sr_recno\" = {1:F0}", fullTableName, currRow[dataSetRecIdCol]);
                        cmd.CommandType = CommandType.Text;
                        var recCount = (Int64)cmd.ExecuteScalar();
                        if (recCount == 0) // No existing record- insert
                        {
                          var values = string.Join(",", table.Columns.Cast<DataColumn>().Select(s => PostgresUtils.PSQLStringyfy(currRow[s.ColumnName], s.ColumnName)));
                          cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES({2});", fullTableName, insertSQLFieldNames, values);
                        }
                        else // existing record- update all fields
                        {
                          var setValues = string.Join(",",
                            table.Columns.Cast<DataColumn>().Where(s => s.ColumnName != "sr_recno")
                            .Select(s => string.Format("\"{0}\" = {1}", s.ColumnName, PostgresUtils.PSQLStringyfy(currRow[s.ColumnName], s.ColumnName))));
                          cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE \"sr_recno\" = {2:F0}", fullTableName, setValues, currRow[dataSetRecIdCol]);
                        }

                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteNonQuery();
                      }
                      #endregion
                    }
                  }
                }

                #region Log the server RecId we have successfully processed
                cmd.CommandText = "SELECT COUNT(*) FROM \"lrep_db_info\" WHERE \"setting\" = 2";
                cmd.CommandType = CommandType.Text;
                var settingCount = (Int64)cmd.ExecuteScalar();
                cmd.CommandText = (settingCount == 0) ?
                  string.Format("INSERT INTO \"lrep_db_info\"(\"setting\", \"value\") VALUES (2, '{0}')", changes.ServerLastRecId) :
                  string.Format("UPDATE \"lrep_db_info\" SET \"value\" = '{0}' WHERE \"setting\" = 2", changes.ServerLastRecId);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                #endregion

                transaction.Commit();
              }
            }
            catch
            {
              transaction.Rollback();
              throw;
            }
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
        LogEvents.Log(DateTime.Now, "UpdateLocalMasterTables.Execute", err.Message, 10);
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<UpdateLocalMasterTables>();

    #endregion

  }
}
