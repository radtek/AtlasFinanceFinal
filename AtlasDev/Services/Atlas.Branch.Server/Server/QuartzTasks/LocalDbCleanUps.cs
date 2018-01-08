/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    Performs PSQL VACUUM and deletes old NLR records
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    2014-02-28- Created
 
 * 
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Data;

using Serilog;
using Npgsql;

using System.Diagnostics;
using ASSSyncClient.Utils.Settings;
using Atlas.DataSync.WCF.Client.ClientProxies;
using ASSSyncClient.Utils;


namespace ASSSyncClient.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class LocalDbCleanUps : global::Quartz.IJob
  {
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "LocalDbCleanUps.Execute";
      _log.Information("{MethodName} starting", methodName);
      try
      {
        // Ensure our local time is currently correct, to ensure we don't delete valid rows
        var serverDateTime = DateTime.MinValue;
        var callTime = new Stopwatch();
        using (var client = new DataSyncDataClient())
        {
          serverDateTime = client.GetServerDateTime();
        }
        callTime.Stop();

        if (serverDateTime > DateTime.MinValue && (Math.Abs(DateTime.Now.Subtract(serverDateTime).TotalSeconds) < 60))
        {
          using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
          {
            conn.Open();

            using (var cmd = conn.CreateCommand())
            {
              var olderThan = DateTime.Now.AddDays(DateTime.Now.Day * -1).AddMonths(-3);

              // Clean up NLR
              cmd.CommandType = CommandType.Text;
              cmd.CommandTimeout = (int)TimeSpan.FromMinutes(5).TotalSeconds;
              cmd.CommandText = string.Format(
                "DELETE FROM \"nlrbatb2\" WHERE \"create_dat\" < '{0:yyyyMMdd}';\r\n" +
                "DELETE FROM \"cs_respx\" WHERE \"proc_date\" < '{0:yyyyMMdd}';", olderThan);
              cmd.ExecuteNonQuery();

              // Perform a vacuum and then an analyze on the DB              
              cmd.CommandText = "VACUUM ANALYZE";
              cmd.CommandTimeout = (int)TimeSpan.FromHours(4).TotalSeconds;
              cmd.ExecuteNonQuery();
            }
          }

          _log.Information("{MethodName} Successfully ran clean-up scripts", methodName);
        }
        else
        {
          _log.Warning("{MethodName} Failed to perform clean-up- local time is >60 seconds out!", methodName);
        }

        _log.Information("{MethodName} Completed", methodName);
      }
      catch (Exception err)
      {
        LogEvents.Log(DateTime.Now, methodName, err.Message, 5);
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<LocalDbCleanUps>();

    #endregion

  }
}
