/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Check we can connect to the local PostgreSQL database
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 

 * 
 *  Comments:    
 *  ------------------
 *    
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Data;

using Serilog;
using Npgsql;
using Quartz;

using ASSSyncClient.Utils;
using ASSSyncClient.Utils.Settings;


namespace ASSSyncClient.QuartzTasks
{
  /// <summary>
  /// Checks we can connect to PostgreSQL via npgsql and execute a simple SQL statement
  /// </summary>
  [DisallowConcurrentExecution]
  public class PostgresCheckConn : IJob
  {
    public void Execute(IJobExecutionContext context)
    {
      var methodName = "PostgresCheckConn.Execute";
      try
      {
        using (var conn = new NpgsqlConnection(AppSettings.NPGSQLConnStr))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandText = "SELECT current_timestamp";
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 10; // seconds
            cmd.ExecuteNonQuery();
          }
        }

        _failCount = 0;
      }
      catch (Exception err)
      {
        _failCount++;
        LogEvents.Log(DateTime.Now, methodName, err.Message, 10 + _failCount);
        _log.Error(err, "{MethodName} - Fail count {0}", _failCount);
      }

      if (_failCount > 5)
      {
        _failCount = 0;
        _log.Fatal("{MethodName} Restarting {FailCount}", _failCount);
        LogEvents.Log(DateTime.Now, "PostgresCheckConn.Execute", "PostgreSQL not responsive- forcing a reboot", 15);
        
        var info = ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest();
        ASSSyncClient.Utils.Alerting.SendEMail.SendError(_log,
          string.Format(
            "WARNING: PostgreSQL server for branch '{0}' (machine {1})- not responding to SQL queries- server is being forcefully rebooted", info.BranchCode, info.MachineName),
          string.Format(
            "The branch PostgreSQL database service is timing out/failing with simple SQL requests.\r\n" +
            "To ensure reliable operation, the machine will now be forcefully re-booted. If this error\r\n" +
            "persists, please manually check the machine:\r\n" +
            "Machine Name: {0}\r\n" +
            "Machine IPs: {1}\r\n" +
            "Machine date/time: {2:yyyy-MM-dd HH:mm:ss}", info.MachineName, info.MachineIPAddresses, info.MachineDateTime), false);
        ASSSyncClient.API.Windows.NativeMethods.Reboot();
      }
    }


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<PostgresCheckConn>();

    /// <summary>
    /// Consecutive fail counts
    /// </summary>
    private static int _failCount = 0;

    #endregion
  }
}
