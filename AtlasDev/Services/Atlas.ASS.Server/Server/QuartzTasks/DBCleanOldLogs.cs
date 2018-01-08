/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 *
 *  Description:
 *  ------------------
 *     Clean-up old database logging
 *
 *
 *  Author:
 *  ------------------
 *     Keith Blows
 *
 *
 *  Revision history:
 *  ------------------
 *     2012-04-24- Routine created
 *
 *     2012-07-25- Updated to DataObjects.net
 *                 DO.NET highlighted need for an improved mechanism to scan terminal states and handle db
 *
 *     2012-08-07- Converted to use XPO
 *
 *     2015-05-25- Reverted to npgsql because of issues with SafePostgreSqlConnectionProvider
 *                 Added COR deletion
 *                 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Quartz;
using Npgsql;

using Atlas.Common.Interface;


namespace Atlas.WCF.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class DBCleanOldLogs : IJob
  {
    public DBCleanOldLogs(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    public void Execute(IJobExecutionContext context)
    {
      var methodName = "DBCleanOldLogs.Execute";
      _log.Information("{MethodName}- Starting", methodName);

      try
      {
        using (var conn = new NpgsqlConnection(_config.GetAtlasCoreConnectionString()))
        {
          conn.Open();

          using (var cmd = conn.CreateCommand())
          {
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandTimeout = 600; // seconds
         
            var deleteOlderThan = DateTime.Now.Subtract(TimeSpan.FromDays(60)); // older than 60 days
            cmd.CommandText = string.Format("DELETE FROM \"LogHWStatus\" WHERE (\"EventDT\" < '{0:yyyy-MM-dd HH:mm:ss}')", deleteOlderThan);
            _log.Information("{MethodName}- Deleted {Records} from COR_LogMachineInfo", methodName, cmd.ExecuteNonQuery());

            deleteOlderThan = DateTime.Now.Subtract(TimeSpan.FromDays(30)); // delete non-essential logs older than 30 days (5- AEDONAEDOAuthorise, 11- CheckCard)
            cmd.CommandText =string.Format("DELETE FROM \"LogTCCTerminal\" WHERE (\"StartDT\" < '{0:yyyy-MM-dd HH:mm:ss}') AND NOT (\"RequestTypeId\" IN (5,11))", deleteOlderThan);
            _log.Information("{MethodName}- Deleted {Records} from LogHWStatus", methodName, cmd.ExecuteNonQuery());

            deleteOlderThan = DateTime.Now.Subtract(TimeSpan.FromDays(18));
            cmd.CommandText = string.Format("DELETE FROM \"COR_LogMachineInfo\" WHERE (\"CreatedDT\" < '{0:yyyy-MM-dd HH:mm:ss}')", deleteOlderThan);
            _log.Information("{MethodName}- Deleted {Records} from COR_LogMachineInfo", methodName, cmd.ExecuteNonQuery());
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }

      _log.Information("{MethodName}- Completed", methodName);
    }
    

    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }
}