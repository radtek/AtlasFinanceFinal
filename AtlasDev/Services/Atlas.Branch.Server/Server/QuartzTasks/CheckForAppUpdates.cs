/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   wyUpdate self updating quartz task
 *   
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-06-13  Created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using Quartz;
using Serilog;
using wyDay.Controls;


namespace ASSSyncClient.QuartzTasks
{
  [global::Quartz.DisallowConcurrentExecution]
  public class CheckForAppUpdates : IJob
  {
    /// <summary>
    /// wyUpdate- check for updates
    /// </summary>
    /// <param name="context"></param>
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "CheckForAppUpdates.Execute";
      _log.Information("{MethodName} Starting", methodName);
      auBackend = new AutomaticUpdaterBackend
        {
          GUID = "79B2EA5B-356F-46A7-BA74-8B537CB6204D",

          // With UpdateType set to Automatic, you're still in
          // charge of checking for updates, but the
          // AutomaticUpdaterBackend continues with the
          // downloading and extracting automatically.
          UpdateType = UpdateType.Automatic,

          // We set the service name that will be used by wyUpdate
          // to restart this service on update success or failure.
          ServiceName = (string)context.JobDetail.JobDataMap["serviceName"]          
        };
      
      auBackend.ReadyToBeInstalled += auBackend_ReadyToBeInstalled;
      auBackend.UpdateSuccessful += auBackend_UpdateSuccessful;
      auBackend.CheckingFailed += auBackend_CheckingFailed;
      auBackend.DownloadingFailed += auBackend_DownloadingFailed;
      auBackend.UpdateFailed += auBackend_UpdateFailed;
     
      // Initialize() and AppLoaded() must be called after events have been set.
      // Note: If there's a pending update to be installed, wyUpdate will be
      //       started, then it will talk back and say "ready to install,
      //       you can close now" at which point your app will be closed.
      auBackend.Initialize();
      auBackend.AppLoaded();
      auBackend.ForceCheckForUpdate();
      if (!auBackend.ClosingForInstall)
      {
        auBackend.ForceCheckForUpdate();        
      }
    }

    
    #region Private events

    private static void auBackend_ReadyToBeInstalled(object sender, EventArgs e)
    {
      // ReadyToBeInstalled event is called when
      // either the UpdateStepOn == UpdateDownloaded or UpdateReadyToInstall

      if (auBackend.UpdateStepOn == UpdateStepOn.UpdateReadyToInstall)
      {
        _log.Information("An update is ready to be installed.... proceeding");      
        // here we'll just close immediately to install the new version
        auBackend.InstallNow();
      }
    }


    private static void auBackend_UpdateSuccessful(object sender, SuccessArgs e)
    {
      _log.Information("Update was successful- New version: {Version}", e.Version);
      ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "CheckForAppUpdates.Execute", string.Format("Update was successful- New version: {0}", e.Version), 1);
    }


    void auBackend_UpdateFailed(object sender, FailArgs e)
    {
      _log.Warning(new Exception(e.ErrorMessage), "auBackend_UpdateFailed");
      ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "CheckForAppUpdates.Execute- auBackend_UpdateFailed", string.Format("{0}-{1}", e.ErrorTitle, e.ErrorMessage), 5);
    }

    void auBackend_DownloadingFailed(object sender, FailArgs e)
    {
      _log.Warning(new Exception(e.ErrorMessage), "auBackend_DownloadingFailed");
      ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "CheckForAppUpdates.Execute- auBackend_DownloadingFailed", string.Format("{0}-{1}", e.ErrorTitle, e.ErrorMessage), 5);
    }

    void auBackend_CheckingFailed(object sender, FailArgs e)
    {
      _log.Warning(new Exception(e.ErrorMessage), "auBackend_CheckingFailed");
      ASSSyncClient.Utils.LogEvents.Log(DateTime.Now, "CheckForAppUpdates.Execute- auBackend_CheckingFailed", string.Format("{0}-{1}", e.ErrorTitle, e.ErrorMessage), 5);
    }
    
    #endregion


    #region Private vars

    private static readonly ILogger _log = Log.Logger.ForContext<CheckForAppUpdates>();

    private static AutomaticUpdaterBackend auBackend;

    #endregion    

  }
}
