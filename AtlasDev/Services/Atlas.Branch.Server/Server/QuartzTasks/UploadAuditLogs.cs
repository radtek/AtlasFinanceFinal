using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Quartz;
using Serilog;

using ASSSyncClient.Utils.Settings;
using Atlas.DataSync.WCF.Client.ClientProxies;
using ASSSyncClient.Utils.WCF;
using Atlas.DataSync.WCF.Interface;


namespace ASSSyncClient.QuartzTasks
{
  internal class UploadAuditLogs : IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "UploadAuditLogs.Execute";
      try
      {
        // Ping server before proceeding
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(10), sendTimeout: TimeSpan.FromSeconds(15)))
        {
          client.GetServerDateTime();
        }

        #region Read audit recid we have
        var localAuditRecId = 0L;
        string localDbVersion;

        using (var conn = new Npgsql.NpgsqlConnection(AppSettings.NPGSQLConnStr))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT COALESCE(MAX(event_id), 0) FROM lrep_audit_changes";
            cmd.CommandType = CommandType.Text;
            localAuditRecId = (Int64)cmd.ExecuteScalar();

            cmd.CommandText = "SELECT \"value\" FROM lrep_db_info WHERE setting = 1 LIMIT 1";
            var value = cmd.ExecuteScalar();
            localDbVersion = (string)value;
          }
        }
        #endregion

        if (localAuditRecId > 0)
        {
          #region Read server's info
          Int64 serverRecId = 0;
          string serverDbVersion;
          using (var client = new DataSyncDataClient())
          {
            serverRecId = client.BranchLastAuditRecId(SyncSourceRequest.CreateSourceRequest());
            serverDbVersion = client.GetServerDatabaseVersion(SyncSourceRequest.CreateSourceRequest());
          }
          #endregion

          _log.Information("Server: {ServerRecId}, {ServerVersion}. Local: {LocalRecId}, {LocalDbVersion}",
            serverRecId, serverDbVersion, localAuditRecId, localDbVersion);

          if (serverRecId > localAuditRecId) // something is wonky- don't proceed
          {
            var err = "UploadAuditLogs cannot proceed- server last audit recid more recent than last client recid";
            _log.Error(new Exception(err), "{MethodName}", methodName);
            Utils.LogEvents.Log(DateTime.Now, methodName, err, 5);
          }

          long lastClientRecId = 0;
          if (serverRecId < localAuditRecId && serverDbVersion == localDbVersion)
          {            
            var items = new List<lrep_audit>();
            #region Get the auditing data
            using (var conn = new Npgsql.NpgsqlConnection(AppSettings.NPGSQLConnStr))
            {
              conn.Open();
              //$"SELECT * FROM lrep_audit_changes WHERE event_id > {serverRecId} ORDER BY event_id LIMIT 10"

              using (var cmd = conn.CreateCommand())
              {
                cmd.CommandText = "SELECT event_id, table_name, oper, sr_recno, relid, session_user_name, " +
                  "action_tstamp_tx, action_tstamp_stm, action_tstamp_clk, transaction_id, " +
                  "application_name, client_addr, client_port, client_query, action, " +
                  "row_data, changed_fields, statement_only FROM lrep_audit_changes " +
                  $"WHERE event_id > {serverRecId} ORDER BY event_id LIMIT 10";
                using (var rdr = cmd.ExecuteReader())
                {
                  while (rdr.Read())
                  {
                    items.Add(new lrep_audit
                    {
                      event_id = rdr.GetInt64(0),
                      table_name = rdr.GetString(1),
                      oper = rdr.GetString(2),
                      sr_recno = rdr.GetDecimal(3),
                      relid = rdr.GetInt32(4),
                      session_user_name = rdr.GetString(5),
                      action_tstamp_tx = rdr.GetDateTime(6),
                      action_tstamp_stm = rdr.GetDateTime(7),
                      action_tstamp_clk = rdr.GetDateTime(8),
                      transaction_id = rdr.GetInt64(9),
                      application_name = rdr.GetString(10),
                      client_addr = ((System.Net.IPAddress)rdr.GetValue(11)).ToString(),
                      client_port = rdr.GetInt32(12),
                      client_query = rdr.GetString(13),
                      action = rdr.GetString(14),
                      row_data = rdr.GetString(15),
                      changed_fields = rdr.IsDBNull(16) ? null : rdr.GetString(16),
                      statement_only = rdr.GetBoolean(17)
                    });
                  }
                }
              }
            }

            lastClientRecId = items.Max(s => s.event_id);
            _log.Information("Found {Rows} audit updates", items.Count);                     
            #endregion

            #region Upload the auditing data
            if (items.Any())
            {
              _log.Information("Uploading {rows} rows of audit data", items.Count);
              using (var client = new DataSyncDataClient())
              {
                var request = SyncSourceRequest.CreateSourceRequest();
                //request.AppVer = "1.2.0.1"; // use new format
                client.UploadBranchAuditChanges(request, localDbVersion, lastClientRecId, items);
              }
            }
            #endregion           
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
        Utils.LogEvents.Log(DateTime.Now, methodName, err.Message, 5);
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<UploadAuditLogs>();

    #endregion

  }
}
