/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Uploads row changes
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     13 July 2013 - Created
 *     
 *     19 Feb 2014  - Fixed nasty upload bug
 *     
 *      9 Dec 2014  - Switched from compressed binary serialized DataSet to UniversalSerilizer
 * 
 * 
 *  Comments:
 *  ------------------
 *    DataSet serialization uses compressed Binary serialized DataSet.
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Configuration;

using Npgsql;
using Serilog;
using Quartz;

using ASSSyncClient.Utils.Settings;
using Atlas.DataSync.WCF.Client.ClientProxies;
using System.Globalization;
using ASSSyncClient.Utils.WCF;


namespace ASSSyncClient.QuartzTasks
{
  /// <summary>
  /// Task to upload all local, non master, table changes to WCF server
  /// </summary>
  [DisallowConcurrentExecution]
  public class UploadChanges : IJob
  {
    /// <summary>
    /// The main task
    /// </summary>
    /// <param name="context"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
      Justification = "Get read-only record changes")]
    public void Execute(IJobExecutionContext context)
    {
      var currTable = string.Empty;
      var currFunction = "Starting";

      var methodName = "UploadChanges.Execute";
      try
      {
        // Ping server before proceeding
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(10), sendTimeout: TimeSpan.FromSeconds(15)))
        {
          client.GetServerDateTime();
        }

        #region Get the last recid server successfully processed by server for this branch, list of master tables + current version of the DB system
        var masterTables = new List<string>();
        Int64 serverLastRecId = 0;
        Int64 localLastRecId = 0;
        string serverDbVersion = null;

        currFunction = "Getting last server RecId processed";
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(20), sendTimeout: TimeSpan.FromSeconds(60)))
        {
          // The last RecId the server successfully processed
          serverLastRecId = client.BranchLastRecId(SyncSourceRequest.CreateSourceRequest());

          // The list of tables which are read-only and which the server will ignore- do not upload
          masterTables = client.GetMasterTableNames(SyncSourceRequest.CreateSourceRequest());

          // Get server's DB version
          serverDbVersion = client.GetServerDatabaseVersion(SyncSourceRequest.CreateSourceRequest());
        }

        if (masterTables == null)
        {
          var err = "UploadChanges cannot proceed- no Master Table listing was received";
          _log.Error(new Exception(err), "{MethodName}", methodName);
          ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "UploadChanges.Execute", err, 5);
          return;
        }

        if (serverLastRecId < 0)
        {
          var err = "UploadChanges cannot proceed- no server last recid was received";
          _log.Error(new Exception(err), "{MethodName}", methodName);
          ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "UploadChanges.Execute", err, 5);
          return;
        }

        if (string.IsNullOrEmpty(serverDbVersion))
        {
          var err = "GetServerDatabaseVersion returned an empty result";
          _log.Error(new Exception(err), "{MethodName}", methodName);
          ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "UploadChanges.Execute", err, 10);

          return;
        }
        #endregion

        using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
        {
          currFunction = "Connecting to local database";
          conn.Open();
          using (var cmd = conn.CreateCommand())
          {
            #region Ensure branch database version is current, else we will not be able to upload
            currFunction = "Checking local database version";
            cmd.CommandText = "SELECT \"value\" FROM lrep_db_info WHERE setting = 1 LIMIT 1";
            var value = cmd.ExecuteScalar();
            if (value == null)
            {
              var err = "lrep_db_info version information missing- no version information entry";
              _log.Error(new Exception(err), "{MethodName}", methodName);
              ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "UploadChanges.Execute", err, 20);

              return;
            }
            var localDbVersion = (string)value;

            if (serverDbVersion != localDbVersion)
            {
              var err = string.Format("Database version mismatch: Local DB version: {0}, Server DB version: {1}", localDbVersion, serverDbVersion);
              _log.Error(new Exception(err), "{MethodName}", methodName);
              ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "UploadChanges.Execute", err, 10);

              return;
            }
            #endregion

            #region Get unique table row changes, since last successful upload
            currFunction = "Getting unique table changes";
            cmd.CommandText = string.Format(
              "SELECT recid, table_name, sr_recno, change_type " +
              "FROM lrep_rec_tracking " +
              "WHERE (recid > {0}) AND (table_name NOT IN ({1})) " +
              "ORDER BY recid " +
              "LIMIT {2}", serverLastRecId,
              string.Join(",", masterTables.Select(s => string.Format("'{0}'", s))), MaxUploadsPerRequest());
            cmd.CommandType = CommandType.Text;

            //_log.Information("{methodName}- {Command}", methodName, cmd.CommandText);

            // Unique changes for each row- Key is 'table_name', Value tuple list contains unique 'sr_recno' (+ last 'change_type')
            var tableNameRecIdAndChange = new Dictionary<string, List<Tuple<decimal, string>>>();

            using (var rdr = cmd.ExecuteReader())
            {
              while (rdr.Read())
              {
                var recId = rdr.GetInt64(0);
                var tableName = rdr.GetString(1);
                var sr_recno = rdr.GetDecimal(2);
                var updateType = rdr.GetString(3);

                var values = tableNameRecIdAndChange.ContainsKey(tableName) ? tableNameRecIdAndChange[tableName] : new List<Tuple<decimal, string>>();
                // Remove previous update type- the last action takes precedence                
                values.RemoveAll(s => s.Item1 == sr_recno);
                values.Add(new Tuple<decimal, string>(sr_recno, updateType));
                tableNameRecIdAndChange[tableName] = values;

                localLastRecId = Math.Max(localLastRecId, recId);
              }
            }
            #endregion

            #region Create DataSet with all changes + current row values
            if (tableNameRecIdAndChange.Any())
            {
              // TODO: Remove !!!!
              /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
              //_log.Information("{MethodName}- {@Changes}", methodName, tableNameRecIdAndChange);

              //_log.Information("Found {ChangeCount} updates", tableNameRecIdAndChange.Count);
              // The DataSet with changes to be uploaded
              var upload = new DataSet();
              var recs = 0;

              foreach (var tableName in tableNameRecIdAndChange.Keys)
              {
                #region Get all rows for records added/updated (not deleted)
                currFunction = "Get added/updated records";
                var sr_recids = string.Join(",", tableNameRecIdAndChange[tableName]
                  .Where(s => s.Item2 != "D")
                  .Select(s => s.Item1.ToString("F0", CultureInfo.InvariantCulture)));

                cmd.CommandText = !string.IsNullOrEmpty(sr_recids) ?
                  string.Format("SELECT * FROM \"{0}\" WHERE sr_recno IN ({1})", tableName, sr_recids) :   // added/edited some records
                  string.Format("SELECT * FROM \"{0}\" WHERE sr_recno = -1", tableName);                   // only contains deleted records- just get structure
                cmd.CommandType = CommandType.Text;

                // TODO: Remove !!!!
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //_log.Information(cmd.CommandText);

                var table = new DataTable(tableName);
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                  adapter.FillSchema(table, SchemaType.Mapped);
                  adapter.Fill(table);
                }
                //_log.Information("Loaded recs: {Recs}", table.Rows.Count);
                #endregion

                #region Set the 'change_type' column for each row
                currFunction = "Getting column indexes";
                var colChangeType = table.Columns.Add("change_type", typeof(string));
                var colRecNo = table.Columns.IndexOf("sr_recno");

                // Set change type for each row
                currFunction = "Setting change type for each row";
                foreach (DataRow dataRow in table.Rows)
                {
                  dataRow[colChangeType.Ordinal] = tableNameRecIdAndChange[tableName]
                    .First(s => s.Item1 == (decimal)dataRow[colRecNo]).Item2;
                  recs++;
                }
                #endregion

                #region Add deleted records- there is no need to include any row data
                currFunction = "Adding deleted records";
                var colSRDeleted = table.Columns.IndexOf("sr_deleted");
                foreach (var deletedRecNo in tableNameRecIdAndChange[tableName].Where(s => s.Item2 == "D"))
                {
                  var row = table.NewRow();
                  row[colRecNo] = deletedRecNo.Item1;
                  row[colChangeType] = "D";
                  row[colSRDeleted] = "Y";
                  table.Rows.Add(row);

                  recs++;
                }
                #endregion

                upload.Tables.Add(table);
              }

              // TODO: Remove !!!!
              /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
              //var dir = "C:\\Atlas\\Lms\\DataSync\\Xml\\";
              //if (!System.IO.Directory.Exists(dir))
              //{
              //  System.IO.Directory.CreateDirectory(dir);
              //}
              //upload.WriteXml(System.IO.Path.Combine(dir, string.Format("{0:yyyy-MM-dd HHmmss}.xml", DateTime.Now)), XmlWriteMode.WriteSchema);
              /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

              #region Upload the DataSet
              currFunction = "Serializing dataset";
              var binDataSet = Atlas.Utils.Serialization.ObjectToBytes.SerializeToBytesJson(upload);

              currFunction = "Uploading dataset";
              using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(100), sendTimeout: TimeSpan.FromSeconds(120)))
              {
                var request = SyncSourceRequest.CreateSourceRequest();
                //request.AppVer = "1.2.0.0"; // Inform server to use new fastJson DataSet format (*much* faster/smaller)
                //_log.Information("Started uploading {Bytes} DataSet", binDataSet.Length);
                client.UploadBranchRowChanges(request, localDbVersion, localLastRecId, binDataSet);
                //_log.Information("Completed uploading {Bytes} DataSet", binDataSet.Length);
              }
              #endregion
            }
            else
            {
              // Notify server we have no changes to upload, but are connected
              using (var client = new DataSyncDataClient())
              {
                var request = SyncSourceRequest.CreateSourceRequest();
                //request.AppVer = "1.2.0.0";
                client.UploadBranchRowChanges(request, localDbVersion, -1, null);
              }
            }
            #endregion
          }
        }
      }
      catch (Exception err)
      {
        var info = string.Format("{0}- Table: [{1}] @ '{2}'", methodName, currTable, currFunction);
        _log.Error(err, info);
        ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, info, err.Message, 5);
      }
    }


    #region Private methods

    private static int MaxUploadsPerRequest()
    {
      var value = ConfigurationManager.AppSettings["maxUploadRecCount"];
      var maxRecs = 0;
      return (!string.IsNullOrEmpty(value) && int.TryParse(value, out maxRecs) && maxRecs > 0 && maxRecs <= MAX_UPLOAD_RECORDS_PER_REQUEST) ?
        maxRecs : DEFAULT_UPLOAD_RECORDS_PER_REQUEST;
    }

    #endregion


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<UploadChanges>();

    /// <summary>
    /// Maximum allowable records to upload per request
    /// </summary>
    private const int MAX_UPLOAD_RECORDS_PER_REQUEST = 500;

    /// <summary>
    /// Default records to upload per request
    /// </summary>
    private const int DEFAULT_UPLOAD_RECORDS_PER_REQUEST = 200;

    #endregion

  }
}
