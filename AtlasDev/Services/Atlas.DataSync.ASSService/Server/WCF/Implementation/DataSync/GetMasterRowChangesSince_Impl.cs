using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using DevExpress.Xpo;
using Npgsql;

using Atlas.Domain.Model;
using Atlas.DataSync.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;
using ASSServer.DbUtils;
using Atlas.Common.Utils;
using ASSServer.Utils.Serialization;


namespace ASSServer.WCF.Implementation.DataSync
{  
  public static class GetMasterRowChangesSince_Impl
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
    public static MasterTableRowChanges Execute(ILogging log, ICacheServer cache, IConfigSettings config,
      SourceRequest sourceRequest, string clientDbVersion, Int64 lastClientRecIdProcessed)
    {
      var result = new MasterTableRowChanges();
      var methodName = "GetMasterRowChangesSince";
     
      try
      {
        ASS_BranchServer_Cached server;
        string errorMessage;
        if (!Checks.VerifyBranchServerRequest(log, sourceRequest, out server, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return null;
        }

        var tableAndRecIds = new Dictionary<string, Tuple<string, List<string>>>();
        long serverLastRecId = 0;

        using (var unitOfWork = new UnitOfWork())
        {
          var changes = unitOfWork.Query<ASS_MasterTableChangeTracking>().Where(s => s.RecId > lastClientRecIdProcessed).OrderBy(s => s.RecId).Take(20);
          if (changes.Any())
          {
            log.Information("{Branch}- Sending master changes: {RecordCount}", sourceRequest.BranchCode, changes.Count());
          }

          foreach (var change in changes)
          {
            if (!tableAndRecIds.ContainsKey(change.TableName))
            {
              tableAndRecIds[change.TableName] = new Tuple<string, List<string>>(change.KeyFieldName, new List<string>());
            }

            if (!tableAndRecIds[change.TableName].Item2.Contains(change.KeyFieldValue))
            {
              tableAndRecIds[change.TableName].Item2.Add(change.KeyFieldValue);
            }

            if (change.RecId > serverLastRecId)
            {
              serverLastRecId = change.RecId;
            }
          }
        }

        if (tableAndRecIds.Count > 0)
        {
          var dataSet = new DataSet();
          using (var conn = new NpgsqlConnection(config.GetAssConnectionString()))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              cmd.CommandType = CommandType.Text;

              foreach (var tableName in tableAndRecIds.Keys)
              {
                var item = tableAndRecIds[tableName];

                var fieldValuesDelimit = item.Item1 == "sr_recno" ? string.Empty : "'";
                cmd.CommandText = string.Format("SELECT * FROM \"company\".\"{0}\" WHERE \"{1}\" IN ({2})",
                  tableName, item.Item1, string.Join(",", item.Item2.Select(s => string.Format("{1}{0}{1}", s, fieldValuesDelimit))));
                using (var adapter = new NpgsqlDataAdapter(cmd))
                {
                  adapter.Fill(dataSet.Tables.Add(tableName));
                }
              }
            }
          }
          
          result.DataSet = (!string.IsNullOrEmpty(sourceRequest.AppVer) && string.Compare(sourceRequest.AppVer, "1.2.0.0") >= 0) ?
            FastJsonSerializer.SerializeToBytesJson(dataSet, true) :
            Serialization.SerializeToBytes(dataSet, true);

          result.ServerLastRecId = serverLastRecId;

          log.Information("{Branch}- Sending result- {DataSetSize} bytes, ServerRecId: {ServerLastRecId}",
            sourceRequest.BranchCode, result.DataSet.Length, result.ServerLastRecId);

          return result;
        }

        return null;
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}- {@Request}", methodName, sourceRequest);
        DbRepos.LogASSBranchServerEvent(0, DateTime.Now, methodName, err.Message, 5);
        return null;
      }
    }
  }
}
