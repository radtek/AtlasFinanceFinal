/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Deletes old session items for Fingerprint GUI/LMS communications
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-01-14- Basic functionality started
 *
 * -----------------------------------------------------------------------------------------------------------------  */

using System;

using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class ExpiredLmsGuiSessions : global::Quartz.IJob
  {
    public ExpiredLmsGuiSessions(ILogging log)
    {
      _log = log;
    }


    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      try
      {
        _log.Information("Expired LMS/GUI session Quartz task starting");
        Atlas.WCF.FPServer.ClientState.LMSGuiState.LmsGuiSessionDeleteOld();
        _log.Information("Expired LMS/GUI session Quartz task completed");

      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
    }


    #region Private vars

    private readonly ILogging _log;

    #endregion

  }
}
