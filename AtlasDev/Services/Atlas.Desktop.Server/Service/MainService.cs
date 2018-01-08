/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Main TopShelf service events- start/stop- used to start Quartz scheduler tasks
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
using Quartz.Impl;

using AClientSvc.QuartzTasks;


namespace AClientSvc
{
  public class MainService
  {
    /// <summary>
    /// Called on service start
    /// </summary>
    /// <param name="serviceName"></param>
    public void Start(string serviceName)
    {
#if DEBUG
      #region Testing
      (new UploadSystemAudit()).Execute(null);
      #endregion
#endif

      // Mutex for installer/UI comms
      _mutex = new System.Threading.Mutex(true, "Global\\Atlas Core Service V1");
      _log.Information("Service starting");

      #region Start scheduler
      // Construct standard scheduler factory
      _schedFact = new StdSchedulerFactory(/*props*/);

      // Get a scheduler
      var sched = _schedFact.GetScheduler();

      var random = new Random();

      #region Sync local time with server- randomly every hour between 07-19
      var syncLocalTimeJob = JobBuilder.Create<SyncLocalTime>().WithIdentity("SyncLocalTime", "General").Build();
      var syncLocalTimeSched = (ICronTrigger)TriggerBuilder.Create()
          .WithIdentity("SyncLocalTime", "General")
          .WithDescription("Sync local time")
          .WithCronSchedule(string.Format("{0} {1} 7-19/1 ? * MON-SAT *", random.Next(59), random.Next(59)),
          s => s.WithMisfireHandlingInstructionFireAndProceed())
          .Build();

      sched.ScheduleJob(syncLocalTimeJob, syncLocalTimeSched);
      #endregion

      #region Upload a software/hardware system audit every 3- 4 hours
      var uploadSystemAuditJob = JobBuilder.Create<UploadSystemAudit>().WithIdentity("UploadSystemAudit", "General").Build();
      var uploadSystemAuditSched = TriggerBuilder.Create()
          .WithIdentity("UploadSystemAudit", "General")
          .WithDailyTimeIntervalSchedule(x =>
            x.OnDaysOfTheWeek(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday })
            .StartingDailyAt(new TimeOfDay(6, (int)random.Next(30) + 10))
            .WithIntervalInMinutes(random.Next(60) + 180)
            .EndingDailyAt(new TimeOfDay(21, 0))
            .WithMisfireHandlingInstructionDoNothing())
          .Build();

      sched.ScheduleJob(uploadSystemAuditJob, uploadSystemAuditSched);
      #endregion

      #region Self update- check for new version every 3-4 hours, from 7:30ish to 19:00
      var selfUpdateJob = JobBuilder.Create<SelfUpdate>().WithIdentity("SelfUpdate", "General").Build();
      var map = selfUpdateJob.JobDataMap;
      map.Put("ServiceName", serviceName);

      var selfUpdateSched = TriggerBuilder.Create()
        .WithIdentity("SelfUpdateSched", "General")
        .WithDailyTimeIntervalSchedule(x =>
          x.OnDaysOfTheWeek(new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday })
            .StartingDailyAt(new TimeOfDay(7, (int)random.Next(30) + 10))
            .EndingDailyAt(new TimeOfDay(19, 0))
            .WithIntervalInMinutes(random.Next(60) + 180)
            .WithMisfireHandlingInstructionFireAndProceed())
          .Build();
      sched.ScheduleJob(selfUpdateJob, selfUpdateSched);
      #endregion


      #region Lock-down USB memory stick for branch machines
      var usbStickProtectJob = JobBuilder.Create<USBStickProtect>().WithIdentity("USBStickProtect", "General").Build();
      var usbStickProtectSched = (ICronTrigger)TriggerBuilder.Create()
          .WithIdentity("USBStickProtect", "General")
          .WithDescription("Sync local time")
          .WithCronSchedule(string.Format("{0} {1} 7-19/4 ? * MON-SAT *", random.Next(59), random.Next(59)),
          s => s.WithMisfireHandlingInstructionFireAndProceed())
          .Build();

      sched.ScheduleJob(usbStickProtectJob, usbStickProtectSched);
      #endregion

      sched.StartDelayed(TimeSpan.FromMinutes(1));

      #endregion

      _log.Information("Service started");
    }


    /// <summary>
    /// Called on service stop
    /// </summary>
    public void Stop()
    {
      _log.Information("Service stopping");
      #region Close scheduler
      _log.Information("Scheduler stopping");
      if (_schedFact != null)
      {
        var scheduler = _schedFact.GetScheduler();
        if (scheduler != null)
        {
          scheduler.Shutdown(true);
        }
        _schedFact = null;
      }
      _log.Information("Scheduler stopped");
      #endregion

      _log.Information("Service was stopped");

      _mutex.Close();
      _mutex = null;
    }


    #region Private members

    /// <summary>
    /// Quartz scheduler
    /// </summary>
    private static ISchedulerFactory _schedFact;

    /// <summary>
    /// 
    /// </summary>
    private static readonly ILogger _log = Log.ForContext<MainService>();


    /// <summary>
    /// 
    /// </summary>
    private System.Threading.Mutex _mutex;

    #endregion

  }
}
