/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Main service
 *     
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013-07-05 Created
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Configuration;

using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Quartz;
using Quartz.Impl;
using Serilog;

using Atlas.Services.DbfGenerate.QuartzTasks;
using Atlas.Services.DbfGenerate.QuartzTasks.Utils;
using Atlas.Data.DevExpress;


namespace Atlas.Services.DbfGenerate
{
  public class ServiceEvents
  {
    /// <summary>
    /// Service started
    /// </summary>
    public void Start()
    {
      _log.Information("Service starting");
      // Construct standard scheduler factory
      _schedFact = new StdSchedulerFactory(/*props*/);

      #region Start XPO- Create a thread-safe data layer
      try
      {
        // Create thread-safe- load and build the domain!
        _log.Information("Attempting to validate and build/load domain");

        var dataStore = XpoDefault.GetConnectionProvider(ConnStrings.Atlas_Core, AutoCreateOption.None);
        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {          
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;

        _log.Information("Successfully loaded and validated domain");
      }
      catch (Exception err)
      {
        _log.Error(err, "Error while loading XPO layer");
        throw;
      }
      #endregion

      var runNowSetting = ConfigurationManager.AppSettings["GenerateDBFsNow"] ?? "false";
      bool runTaskNow;
      if (bool.TryParse(runNowSetting, out runTaskNow) && runTaskNow)
      {
        var runTask = new GenerateDBFs(); //DownloadDBFs_Emergency(); /* */
        runTask.Execute(null);
        return;
      }

      #region Scheduled tasks
      try
      {
        // Get a scheduler
        var sched = _schedFact.GetScheduler();

        #region Generate DBFs @ 22:00
        try
        {
          var downDbfsTask = JobBuilder.Create<GenerateDBFs>().WithIdentity("GenerateDBFTask", "General").Build();
          var downDbfSched = TriggerBuilder.Create()
              .WithIdentity("GenerateDbfDaily", "General")
              .WithDailyTimeIntervalSchedule(s =>
                s.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(22, 0))
                .WithInterval(24, IntervalUnit.Hour)
                .OnEveryDay()
                .WithMisfireHandlingInstructionIgnoreMisfires())
              .Build();
          sched.ScheduleJob(downDbfsTask, downDbfSched);
          _log.Information("DBF generation next fire: {NextFire: yyyy-MM-dd HH:mm:ss}", downDbfSched.GetNextFireTimeUtc());
        }
        catch (Exception err)
        {
          _log.Error(err, "Failed to schedule task");
        }

        #endregion

        _log.Information("Service started successfully");

        sched.Start();
      }
      catch (Exception err)
      {
        _log.Error(err, "Error while scheduling tasks");
        throw;
      }
      #endregion
    }


    /// <summary>
    /// Service stopped- clean-up
    /// </summary>
    public void Stop()
    {
      _log.Information("Service stopping");

      try
      {
        #region Close Quartz scheduler
        _log.Information("Shutting down scheduler");
        if (_schedFact != null)
        {
          _log.Information("Quartz scheduler stopping");
          var scheduler = _schedFact.GetScheduler();
          if (scheduler != null)
          {
            scheduler.Shutdown(true);
          }
        }
        _log.Information("Quartz scheduler stopped");
        _schedFact = null;
        #endregion

        #region Close XPO
        XpoDefault.DataLayer = null;
        #endregion
      }
      catch (Exception err)
      {
        _log.Error(err, "Error while shutting down");
      }

      _log.Warning("Service stopped");
    }


    #region Private vars
      
    private static readonly ILogger _log = Log.ForContext<ServiceEvents>();

    private ISchedulerFactory _schedFact;

    #endregion

  }
}
