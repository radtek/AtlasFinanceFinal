/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *    Logs local, in-memory logged events to the server, via WCF
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    2013-08-15- Created
 
 * 
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;

using Serilog;

using Atlas.DataSync.WCF.Client.ClientProxies;
using Atlas.DataSync.WCF.Interface;
using ASSSyncClient.Utils;


namespace ASSSyncClient.QuartzTasks
{
  /// <summary>
  /// Task to upload events to server
  /// </summary>
  [global::Quartz.DisallowConcurrentExecution]
  public class LogEventsToServer : global::Quartz.IJob
  {
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "LogEventsToServer.Execute";
      try
      {
        // Ping server before proceeding
        using (var client = new DataSyncDataClient(openTimeout: TimeSpan.FromSeconds(10), sendTimeout: TimeSpan.FromSeconds(15)))
        {
          client.GetServerDateTime();
        }

        #region Get events we need to upload and remove from threa-safe memory store
        var events = LogEvents.GetPendingEvents(5);
        if (events == null || events.Count == 0)
        {
          return;
        }

        _log.Information("{MethodName} Logging {EventCount} events", methodName, events.Count);
        var uploadEvents = events.Select(s => new LogEvent()
          {
            RaisedDT = s.RaisedDT,
            Task = s.Task,
            EventMessage = s.EventMessage,
            Severity = s.Severity
          }).ToList();
        #endregion

        #region Try upload
        var attemptCounts = 0;
        var success = false;
        var rnd = new Random();
        while (!success && attemptCounts++ < 5)
        {
          try
          {
            using (var client = new DataSyncDataClient())
            {
              client.LogEvents(ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest(), uploadEvents);
            }

            success = true;
          }
          catch (Exception err)
          {
            _log.Error(err, "{MethodName}", methodName);
            System.Threading.Thread.Sleep(rnd.Next(15000) + 10000); // don't blast the server
          }
        }
        #endregion

        // If failed, add events back to in-memory, thread-safe event store
        if (!success)
        {
          LogEvents.Log(events);
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
      }
    }


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<LogEventsToServer>();

    #endregion

  }
}
