/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2016 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Service events
 * 
 * 
 *  Author:
 *  ------------------
 *     Updates by: Keith Blows
 *     Original author: Fabian
 * 
 *
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Threading.Tasks;

using Atlas.Common.Interface;
using SchedulerServer.AltechNuPay;


namespace SchedulerServer
{
  public class MainService
  {
    public MainService(ILogging log, IConfigSettings config)
    {
      _log = log;
      _config = config;
      _log.Information("Service initialized");
    }


    public bool Start()
    {
      _log.Information("Service starting...");

      #region To Lee: Is this for testing or intentional? If intentional, this will cause service start failure- I have made a task (Keith)
      Task.Run(() =>
        {
          try
          {
            AEDO.ImportReports(_log, _config);
            AEDO.ImportReports(_log, _config, true);
            NAEDO.ImportReports(_log, _config);
            NAEDO.ImportReports(_log, _config, true);
          }
          catch (Exception err)
          {
            _log.Error(err, "MainService.Task.Run().. ImportReports");
          }
        });
      #endregion

      _log.Information("Service started");
      return true;
    }


    public bool Stop()
    {
      _log.Information("Quartz scheduler stopping...");
      _log.Information("Service stopped");

      return true;
    }


    private readonly ILogging _log;
    private readonly IConfigSettings _config;
  }
}