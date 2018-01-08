/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 *
 *  Description:
 *  ------------------
 *     Quartz.net task to check the PING status of all ready terminals.
 *
 *  Author:
 *  ------------------
 *     Keith Blows
 *
 *
 *  Revision history:
 *  ------------------
 *     2012-05-02- Skeleton created
 *     2012-07-25- Upgraded to DataObjects.net and improved mechanism (old EF version kept EF transaction open)
 *                 New mechanism uses something like: getNextTerminal(handledTerminalList)
 *
 * ----------------------------------------------------------------------------------------------------------------- */
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Quartz;

using Atlas.Cache.Interfaces.Classes;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Server.Utils;


namespace Atlas.WCF.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class TCCCheckSafe : IJob
  {
    public TCCCheckSafe(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    /// <summary>
    /// Checks the status of all active terminals
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Altech TCC safe online check task starting");

        // Build list of terminals, in use or not       
        var pendingTerminalIds = new ConcurrentBag<long>(TccCacheUtils.GetAllActivatedTerminals(_cache));
       
        #region Check terminals
        if (pendingTerminalIds.Any())
        {
          var tasks = new Task[10];
          for (var i = 0; i < tasks.Length; i++)
          {
            tasks[i] = Task.Run(async () =>
            {
              long terminalId;
              while (pendingTerminalIds.TryTake(out terminalId))
              {
                var polledStart = DateTime.Now;

                string error = null;
                string responseCode = null;

                var terminal = _cache.Get<TCCTerminal_Cached>(terminalId);
                // Network bound- async candidate           
                var terminalStatus = await TccSoapUtils.TerminalReadySafeAsync(_log, _config, terminal);

                #region Update query result counts

                if (terminalStatus.Status == TccSoapUtils.TCCStatus.Offline)
                {
                  TccCacheUtils.SetTerminalFailed(_cache, terminalId, responseCode, error, polledStart);
                  _log.Warning("The active NuPay TCC terminal with TerminalID of {TerminalId}, is not currently in a ready state", terminalId);
                }
                else if (terminalStatus.Status == TccSoapUtils.TCCStatus.Online)
                {
                  TccCacheUtils.SetTerminalOnline(_cache, terminalId, polledStart);
                }
                else if (terminalStatus.Status == TccSoapUtils.TCCStatus.Unknown)
                {
                  //DbSetTerminalUnknown(terminalId, polledStart);
                }

                #endregion Update query result counts
              }
            });
          }

          Task.WaitAll(tasks, 60000);
        }
        #endregion Check terminals
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }

      _log.Information("Altech TCC safe online check task completed");
    }


    private readonly ILogging _log;
    private readonly ICacheServer _cache;
    private readonly IConfigSettings _config;

  }
}