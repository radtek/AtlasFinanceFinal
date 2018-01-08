using System;

using Serilog;
using Quartz;

using Atlas.DataSync.WCF.Client.ClientProxies;
using ASSSyncClient.Utils.WCF;
using ASSSyncClient.Utils.Settings;


namespace ASSSyncClient.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class UploadLocalLrepRecId : IJob
  {    
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "UploadLocalLrepRecId.Execute";
      try
      {
        // Ping server before proceeding
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(10), sendTimeout: TimeSpan.FromSeconds(15)))
        {
          client.GetServerDateTime();
        }

        var maxRecId = -1L;
        using (var conn = new Npgsql.NpgsqlConnection(AppSettings.NPGSQLConnStr))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT COALESCE(MAX(recid), 0) FROM lrep_rec_tracking";
            cmd.CommandType = System.Data.CommandType.Text;
            maxRecId = (Int64)cmd.ExecuteScalar();
          }
        }

        if (maxRecId > 0)
        {
          using (var client = new DataSyncDataClient())
          {            
            client.UploadCurrentBranchRecId(SyncSourceRequest.CreateSourceRequest(), maxRecId);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
      }
    }


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<UploadLocalLrepRecId>();

    #endregion

  }
}
