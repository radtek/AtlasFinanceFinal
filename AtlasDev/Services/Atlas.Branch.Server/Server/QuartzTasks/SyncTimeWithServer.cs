/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Synchronize local time with server
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

using Quartz;

using Atlas.DataSync.WCF.Client.ClientProxies;
using Serilog;


namespace ASSSyncClient.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class SyncTimeWithServer: IJob
  {
    /// <summary>
    /// Sync local time with server using WCF call
    /// </summary>
    /// <param name="context"></param>
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "SyncTimeWithServer.Execute";
      try
      {
        _log.Information("{MethodName} starting", methodName);
        var serverDateTime = DateTime.MinValue;
        var callTime = new System.Diagnostics.Stopwatch();
        using (var client = new DataSyncDataClient())
        {
          serverDateTime = client.GetServerDateTime();
        }
        callTime.Stop();

        _log.Information("{MethodName} Server time: {Server:yyyy-MM-dd HH:mm:ss}, local time: {Local::yyyy-MM-dd HH:mm:ss}", 
          methodName, serverDateTime, DateTime.Now);

        if (serverDateTime > DateTime.MinValue && (Math.Abs(DateTime.Now.Subtract(serverDateTime).TotalSeconds) > 10))
        {
          var setTime = serverDateTime.Subtract(callTime.Elapsed);
          ASSSyncClient.API.Windows.NativeMethods.SetLocalTime(setTime);
        }
      }
      catch (Exception err)
      {
        ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, methodName, err.Message, 5);
        _log.Error(err, "{MethodName}", methodName);
      }

      _log.Information("{MethodName} complete", methodName);
    }
    

    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<SyncTimeWithServer>();

    #endregion    

  }
}