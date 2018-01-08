using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Npgsql;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.DataSync.WCF.Interface;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;
using Atlas.Cache.DataUtils;


namespace ASSServer.WCF.Implementation.DataSync
{
  internal class UploadBranchAuditChanges_Impl
  {
    internal static bool Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      SourceRequest sourceRequest,
      string clientDbVersion, long lastClientAuditRecId, List<lrep_audit> audit)
    {
      var methodName = "UploadBranchAuditChanges";

      try
      {
        #region Check parameters
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }
        var branch = cache.Get<BRN_Branch_Cached>(server.Branch.Value);
        var legacyBranchNum = branch.LegacyBranchNum.PadLeft(3, '0');

        if (string.IsNullOrEmpty(clientDbVersion) || !Regex.IsMatch(clientDbVersion, "^[0-9A-Z]{5}$"))
        {
          log.Error(new ArgumentOutOfRangeException("clientDbVersion", string.Format("Value: '{0}'", clientDbVersion)), methodName);
          return false;
        }

        if (lastClientAuditRecId <= 0)
        {
          log.Error(new ArgumentOutOfRangeException("lastClientAuditRecId"), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }

        var serverDbVersion = CacheUtils.GetCurrentDbVersion(cache);
        if (serverDbVersion.DBVersion != clientDbVersion)
        {
          log.Error(new Exception(string.Format("Cannot sync due to DB mismatch: Client DB version: {0}, Server DB version: {1}", clientDbVersion, serverDbVersion)),
            "{@Request}", sourceRequest);
          return false;
        }

        if (audit == null || audit.Count == 0)
        {
          log.Error(new ArgumentNullException("audit"), "{MethodName}- {@Request}", methodName, sourceRequest);
          return false;
        }      
        #endregion

        log.Information("{MethodName}- Rows: {Rows}", methodName, audit.Count);

        #region Insert the audit records
        var lastClientRecId = 0L;
        using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText =
              "INSERT INTO company.lrep_audit_changes(client_event_id,table_name,oper,sr_recno,relid,session_user_name," +
              "action_tstamp_tx,action_tstamp_stm,action_tstamp_clk,transaction_id,application_name,client_addr,client_port,client_query," +
              "action,row_data,changed_fields,statement_only,lrep_brnum) " +
              "VALUES (@client_event_id,@table_name,@oper,@sr_recno,@relid,@session_user_name," +
              "@action_tstamp_tx,@action_tstamp_stm,@action_tstamp_clk,@transaction_id,@application_name,@client_addr::inet,@client_port,@client_query," +
              "@action,@row_data::hstore,@changed_fields::hstore,@statement_only,@lrep_brnum);";

            // aa unknown types to be text
            cmd.AllResultTypesAreUnknown = true;
            var client_event_id = cmd.Parameters.Add("client_event_id", NpgsqlTypes.NpgsqlDbType.Bigint);
            var table_name = cmd.Parameters.Add("table_name", NpgsqlTypes.NpgsqlDbType.Text);
            var oper = cmd.Parameters.Add("oper", NpgsqlTypes.NpgsqlDbType.Char, 4);
            var sr_recno = cmd.Parameters.Add("sr_recno", NpgsqlTypes.NpgsqlDbType.Numeric, 15);
            var relid = cmd.Parameters.Add("relid", NpgsqlTypes.NpgsqlDbType.Oid);
            var session_user_name = cmd.Parameters.Add("session_user_name", NpgsqlTypes.NpgsqlDbType.Text);
            var action_tstamp_tx = cmd.Parameters.Add("action_tstamp_tx", NpgsqlTypes.NpgsqlDbType.TimestampTZ);
            var action_tstamp_stm = cmd.Parameters.Add("action_tstamp_stm", NpgsqlTypes.NpgsqlDbType.TimestampTZ);
            var action_tstamp_clk = cmd.Parameters.Add("action_tstamp_clk", NpgsqlTypes.NpgsqlDbType.TimestampTZ);
            var transaction_id = cmd.Parameters.Add("transaction_id", NpgsqlTypes.NpgsqlDbType.Bigint);
            var application_name = cmd.Parameters.Add("application_name", NpgsqlTypes.NpgsqlDbType.Text);
            var client_addr = cmd.Parameters.Add("client_addr", NpgsqlTypes.NpgsqlDbType.Text);
            var client_port = cmd.Parameters.Add("client_port", NpgsqlTypes.NpgsqlDbType.Integer);
            var client_query = cmd.Parameters.Add("client_query", NpgsqlTypes.NpgsqlDbType.Text);
            var action = cmd.Parameters.Add("action", NpgsqlTypes.NpgsqlDbType.Text);
            var row_data = cmd.Parameters.Add("row_data", NpgsqlTypes.NpgsqlDbType.Text);
            var changed_fields = cmd.Parameters.Add("changed_fields", NpgsqlTypes.NpgsqlDbType.Text);
            var statement_only = cmd.Parameters.Add("statement_only", NpgsqlTypes.NpgsqlDbType.Boolean);
            cmd.Parameters.AddWithValue("lrep_brnum", sourceRequest.BranchCode.PadLeft(3, '0'));

            foreach (var row in audit)
            {              
              if (row.event_id > server.LastProcessedClientAuditRecId)
              {
                client_event_id.Value = row.event_id;
                table_name.Value = row.table_name;
                oper.Value = row.oper;
                sr_recno.Value = row.sr_recno;
                relid.Value = row.relid;
                session_user_name.Value = row.session_user_name;
                action_tstamp_tx.Value = row.action_tstamp_tx;
                action_tstamp_stm.Value = row.action_tstamp_stm;
                action_tstamp_clk.Value = row.action_tstamp_clk;
                transaction_id.Value = row.transaction_id;
                application_name.Value = row.application_name;
                client_addr.Value = row.client_addr;
                client_port.Value = row.client_port;
                client_query.Value = row.client_query;
                action.Value = row.action;
                row_data.Value = row.row_data;
                changed_fields.Value = string.IsNullOrEmpty(row.changed_fields) ? string.Empty : row.changed_fields;
                statement_only.Value = row.statement_only;

                cmd.ExecuteNonQuery();

                if (row.event_id > lastClientRecId)
                {
                  lastClientRecId = row.event_id;
                }
              }
            }
          }
        }
        #endregion

        // Update ASS_BranchServer.LastProcessedClientAuditRecId
        if (lastClientRecId > server.LastProcessedClientAuditRecId)
        {
          server.LastProcessedClientAuditRecId = lastClientRecId;
          cache.Set(new List<ASS_BranchServer_Cached> { server });
        }
        else if (lastClientRecId < server.LastProcessedClientAuditRecId)
        {
          log.Error("{MethodName}- LastProcessedClientAuditRecId mismatch. Client: {LastProcessedClientAuditRecId}, Server: {ServerLastProcessedClientAuditRecId}",
            methodName, lastClientAuditRecId, server.LastProcessedClientAuditRecId);
        }
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);

        return false;
      }

      return true;
    }

  }
}
