/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Clean up old, unnecessary ass records
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     31 May 2013 - Created
 * 
 * 
 *  Comments:
 *  ------------------
 *   
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using ASSServer.Shared;
using Atlas.Common.Interface;


namespace ASSServer.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class CleanUps : global::Quartz.IJob
  {
    public CleanUps(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
    }


    /// <summary>
    /// Main Quartz method
    /// </summary>
    /// <param name="context"></param>
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "CleanUps.Execute";
      try
      {
        _log.Information("{MethodName} starting", methodName);

        _log.Information("Deleting temp files- starting");
        TempFiles.DeleteFilesOlderThan(_log, 240);
        _log.Information("Deleting temp files- completed");

        _log.Information("{MethodName} completed", methodName);
      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }

      _log.Information("{MethodName} completed", methodName);
    }


    #region Private fields
  
    private readonly ILogging _log;
    private readonly IConfigSettings _config;

    #endregion

  }
}
