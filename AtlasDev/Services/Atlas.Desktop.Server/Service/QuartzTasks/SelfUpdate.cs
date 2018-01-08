/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Task to perform wyUpdate service self-updating 
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-08-26 Created 
 *     
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using Serilog;
using Quartz;
using wyDay.Controls;


namespace AClientSvc.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class SelfUpdate : IJob
  {
    /// <summary>
    /// Task to check for a new version, via wyUpdate
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {      
      _log.Information("Self update- task starting");

      // Get name of service- wyUpdate AutomaticUpdaterBackend needs to know
      var serviceName = context.JobDetail.JobDataMap.GetString("ServiceName");
      if (string.IsNullOrEmpty(serviceName))
      {
        _log.Error("Execute- Failed to get service name from the job data map!");
        return;
      }

      try
      {
        _auBackend = new AutomaticUpdaterBackend
        
        {
          GUID = "0911CEEB-1B02-4E86-B7C2-ACFA1CDE4CA9",

          // With UpdateType set to Automatic, you're still in
          // charge of checking for updates, but the
          // AutomaticUpdaterBackend continues with the
          // downloading and extracting automatically.
          UpdateType = UpdateType.Automatic,

          // We set the service name that will be used by wyUpdate
          // to restart this service on update success or failure.
          ServiceName = serviceName
        };
        _auBackend.ReadyToBeInstalled += auBackend_ReadyToBeInstalled;
        _auBackend.CheckingFailed += auBackEnd_CheckingFailed;
        _auBackend.DownloadingFailed += auBackEnd_DownloadingFailed;
        _auBackend.ExtractingFailed += auBackEnd_ExtractingFailed;
        _auBackend.UpdateFailed += auBackEnd_UpdateFailed;
        _auBackend.UpdateSuccessful += auBackEnd_UpdateSuccessful;
        _auBackend.UpToDate += auBackEnd_UpToDate;

        _auBackend.Initialize();
        _auBackend.AppLoaded();
        _auBackend.ForceCheckForUpdate();
        _log.Information("Self update- task completed");
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }
    }

    private void auBackEnd_UpToDate(object sender, SuccessArgs e)
    {
      _log.Information("UpToDate- {0}", e.Version);
    }

    private void auBackEnd_UpdateSuccessful(object sender, SuccessArgs e)
    {
      _log.Information("UpdateSuccessful- {0}", e.Version);
    }

    private void auBackEnd_ExtractingFailed(object sender, FailArgs e)
    {
      _log.Error(new Exception(e.ErrorMessage), "ExtractingFailed");
    }

    private void auBackEnd_UpdateFailed(object sender, FailArgs e)
    {
      _log.Error(new Exception(e.ErrorMessage), "UpdateFailed");
    }

    private void auBackEnd_DownloadingFailed(object sender, FailArgs e)
    {
      _log.Error(new Exception(e.ErrorMessage), "DownloadingFailed");
    }

    private void auBackEnd_CheckingFailed(object sender, FailArgs e)
    {
      _log.Error(new Exception(e.ErrorMessage), "CheckingFailed");
    }


    #region Private methods

    private void auBackend_ReadyToBeInstalled(object sender, EventArgs e)
    {
      // ReadyToBeInstalled event is called when
      // either the UpdateStepOn == UpdateDownloaded or UpdateReadyToInstall

      if (_auBackend.UpdateStepOn == UpdateStepOn.UpdateReadyToInstall)
      {
        // Delay the installation of the update until
        // it's appropriate for your app.

        // Do any "spin-down" operations. auBackend.InstallNow() will
        // exit this process using Environment.Exit(0), so run
        // cleanup functions now (close threads, close running programs,
        // release locked files, etc.)

        _log.Information("Self update- update found and being installed...");
        // here we'll just close immediately to install the new version
        _auBackend.InstallNow();
      }
    }

    #endregion


    #region Private vars

    /// <summary>
    /// Auto updater
    /// </summary>
    AutomaticUpdaterBackend _auBackend;

    // Log4net
    private static readonly ILogger _log = Log.ForContext<SelfUpdate>();

    #endregion
        
  }
}
