/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Reboots the server, if uptime >48 hours. To be run outside of operating hours.
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

using Serilog;
using ASSSyncClient.Utils;


namespace ASSSyncClient.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class RebootServerWeekly : global::Quartz.IJob
  {
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "RebootServerWeekly.Execute";
      try
      {
        _log.Information("{MethodName} starting", methodName);
        // Uptime >48 hours
        var ts = TimeSpan.FromMilliseconds(Environment.TickCount);  //     

        _log.Information("{MethodName} Server uptime: {Uptime}", methodName, ts.TotalHours);
        if (ts >= TimeSpan.FromHours(48))
        {
          _log.Warning("{MethodName} Server has been running for more than 48 hours- forcing a reboot", methodName);          
          ASSSyncClient.API.Windows.NativeMethods.Reboot();
        }
      }
      catch (Exception err)
      {
        LogEvents.Log(DateTime.Now, "RebootServerWeekly.Execute", err.Message, 5);
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<RebootServerWeekly>();

    #endregion

  }
}
