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
 *     2012-10-252- Created
 *
 * ----------------------------------------------------------------------------------------------------------------- */
using System;
using System.Linq;

using Quartz;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Cache.Interfaces.Classes;


namespace Atlas.WCF.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class TCCResetAllToUnknown : IJob
  {
    public TCCResetAllToUnknown(ILogging log, ICacheServer cache)
    {
      _log = log;
      _cache = cache;
    }


    /// <summary>
    /// Sets status of all active terminals to 'unknown'
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Altech TCC reset to 'Unknown' starting");

        var terminals = _cache.GetAll<TCCTerminal_Cached>().Where(s => s.Status != 99).ToList();
        foreach (var terminal in terminals)
        {
          terminal.Status = 2;
          terminal.LastRequestResult = null;
          terminal.LastRequestType = null;
          terminal.LastRequestDT = null;
        }
        _cache.Set<TCCTerminal_Cached>(terminals);
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }

      _log.Information("Altech TCC reset to 'Unknown' completed");
    }


    #region Private vars

    private readonly ILogging _log;
    private readonly ICacheServer _cache;

    #endregion Private vars

  }
}