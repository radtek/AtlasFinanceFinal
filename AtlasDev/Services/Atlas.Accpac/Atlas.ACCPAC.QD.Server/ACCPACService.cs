using System;

using Quartz;
using Quartz.Impl;

using WinServices.BLL;
using Serilog;


namespace Atlas.ACCPAC
{
  public class ACCPACService
  {
    public ACCPACService()
    {
      _log.Information("Service created");
    }


    public void Start()
    {
      _log.Information("Service starting");

      SharedMembers.CreateDirectories();

      // Construct standard scheduler factory
      _schedFact = new StdSchedulerFactory(/*props*/);

      // Get a scheduler
      var sched = _schedFact.GetScheduler();

      try
      {
        // Delete orphaned fingerprint upload sessions- run continuously every 13 min  
        var runGLTask = JobBuilder.Create<Atlas.ACCPAC.QuartzTasks.UploadGLTransactions>().WithIdentity("ProcessQDs", "General").Build();
        var runGLEveryMinute = (ICronTrigger)TriggerBuilder.Create()
            .WithIdentity("ProcessQDs", "General")
            .WithCronSchedule("0/5 * * * * ? *", x => x.WithMisfireHandlingInstructionDoNothing())
            .Build();

        sched.ScheduleJob(runGLTask, runGLEveryMinute);
      }
      catch (Exception err)
      {
        _log.Error(err, "Quartz.net: Error scheduling reset all TCC terminals to unknown status");
      }

      sched.Start();
    }
    

    public void Stop()
    {
      _log.Information("Service stopping");

      if (_schedFact != null)
      {
        var scheduler = _schedFact.GetScheduler();
        if (scheduler != null)
        {
          scheduler.Shutdown(true);
        }
      }

      _log.Information("Service stopped");
    }
       

    #region Private vars

    // Logging
    private static readonly ILogger _log = Log.Logger.ForContext<ACCPACService>();


    // Quartz scheduler
    private ISchedulerFactory _schedFact;

    #endregion
  }
}
