/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 *
 *  Description:
 *  ------------------
 *     A Quartz.net task to check on the status of a 'stuck busy' terminal (ie status '1') NuPay magIC TCC enabled terminals
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
 *     2012-07-25- Updated to DataObjects.net
 *                 DO.NET highlighted need for an improved mechanism to scan terminal states and handle db
 *
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;

using Quartz;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Server.Utils;
using Atlas.Cache.Interfaces.Classes;


namespace Atlas.WCF.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class TCCCheckStatusStuck : IJob
  {
    public TCCCheckStatusStuck(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    /// <summary>
    /// Checks for stuck terminals and executes a 'handshake' request to forcibly check the status
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Check NuPay status stuck Quartz task starting");

        var processedTermIDs = new List<long>();
        TCCTerminal_Cached terminal = null;
        while ((terminal = TccCacheUtils.GetNextStuckTerminal(_cache, processedTermIDs)) != null)
        {
          processedTermIDs.Add(terminal.TerminalId);

          string error = null;
          var polledStart = DateTime.Now;
          if (TccSoapUtils.TerminalReady(_cache, _config, _log, terminal, true, out error))
          {
            TccCacheUtils.SetTerminalSuccess(_cache, terminal.TerminalId, polledStart);

            _log.Warning("Automatically reset stuck state for {TerminalID}", terminal.TerminalId);
          }
          else
          {
            TccCacheUtils.SetTerminalFailure(_cache, terminal.TerminalId, error, polledStart);

            _log.Warning("TerminalID {TerminalID} still not currently in a ready state", terminal.TerminalId);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }

      _log.Information("Check NuPay status stuck Quartz task completed");
    }
    

    private readonly ILogging _log;
    private readonly ICacheServer _cache;
    private readonly IConfigSettings _config;
    
  }
}