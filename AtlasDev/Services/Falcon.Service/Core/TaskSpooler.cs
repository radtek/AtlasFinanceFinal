using System;
using System.Configuration;
using Falcon.Service.Business.Reporting;
using Falcon.Service.Tasks;
using Quartz;
using Quartz.Impl;
using Serilog;

namespace Falcon.Service.Core
{
  public sealed class TaskSpooler
  {
    private readonly ILogger _logger = Log.Logger.ForContext<TaskSpooler>();
    private ISchedulerFactory _schedFact;


    public void Start()
    {
      var clearcache = bool.Parse(ConfigurationManager.AppSettings["redis.clearcache"] ?? "false");
      if (clearcache)
      {     
        RedisConnection.SetStringFromObject<DateTime?>(AssReporting.REDIS_KEY_LAST_POSSIBLE_HANDOVERS_RUN, null, new TimeSpan(0, 0, 1));
        RedisConnection.SetStringFromObject<DateTime?>(AssReporting.REDIS_KEY_LAST_ONE_TIME_RUN, null, new TimeSpan(0, 0, 1));
      }
      _logger.Information("[FalconService] - Preparing tasks...");

      _schedFact = new StdSchedulerFactory(/*props*/);

      //// Get a scheduler
      var sched = _schedFact.GetScheduler();

      //var taskNotificationTask = JobBuilder.Create<NotificationTask>().WithIdentity("NotificationTask ", "Tasks").Build();

      //var triggerNotificationTasksk = (ISimpleTrigger)TriggerBuilder.Create()
      //      .WithIdentity("triggerNotificationTasksk")
      //      .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(30).RepeatForever().WithMisfireHandlingInstructionFireNow())
      //      .StartNow().Build();

      //sched.ScheduleJob(taskNotificationTask, triggerNotificationTasksk);

      //var taskRedisAccountCache = JobBuilder.Create<AccountTask>().WithIdentity("RedisAccountCache  ", "Tasks").Build();

      //var triggerRedisAccountcache = (ISimpleTrigger)TriggerBuilder.Create()
      //      .WithIdentity("triggerRedisAccountcache")
      //      .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(4).RepeatForever().WithMisfireHandlingInstructionFireNow())
      //      .StartNow().Build();

      //sched.ScheduleJob(taskRedisAccountCache, triggerRedisAccountcache);

      //var taskAssReportCache = JobBuilder.Create<AssCiReportTask>().WithIdentity("AssReportTask  ", "Tasks").Build();

      //var triggerAssReportcache = (ISimpleTrigger)TriggerBuilder.Create()
      //      .WithIdentity("triggerAssReportcache")
      //      .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(5).RepeatForever().WithMisfireHandlingInstructionFireNow())
      //      .StartNow().Build();

      //sched.ScheduleJob(taskAssReportCache, triggerAssReportcache);

      //var taskAssCiReportPastMonthsCache = JobBuilder.Create<AssCIReportPastMonthsTask>().WithIdentity("AssCIReportPastMonthsTask  ", "Tasks").Build();

      //var triggerAssCiReportPastMonthsCache = (ISimpleTrigger)TriggerBuilder.Create()
      //      .WithIdentity("triggerAssCIReportPastMonthsCache")
      //      .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInHours(2).RepeatForever().WithMisfireHandlingInstructionFireNow())
      //      .StartNow().Build();

      //sched.ScheduleJob(taskAssCiReportPastMonthsCache, triggerAssCiReportPastMonthsCache);

      //var taskAssCiReportToDateCache = JobBuilder.Create<AssCIReportToDateTask>().WithIdentity("AssCIReportToDateTask  ", "Tasks").Build();

      //var triggerAssCiReportToDateCache = (ISimpleTrigger)TriggerBuilder.Create()
      //      .WithIdentity("triggerAssCIReportToDateCache")
      //      .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(5).RepeatForever().WithMisfireHandlingInstructionFireNow())
      //      .StartNow().Build();

      //sched.ScheduleJob(taskAssCiReportToDateCache, triggerAssCiReportToDateCache);

      //var taskAssReportPossibleHandoverCache = JobBuilder.Create<AssCiReportPossibleHandoverTask>().WithIdentity("AssCiReportPossibleHandoverTask", "Tasks").Build();

      //var triggerAssReportPossibleHandoverCache = (ISimpleTrigger)TriggerBuilder.Create()
      //      .WithIdentity("triggerAssReportPossibleHandoverCache")
      //      .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(5).RepeatForever().WithMisfireHandlingInstructionFireNow())
      //      .StartNow().Build();

      //sched.ScheduleJob(taskAssReportPossibleHandoverCache, triggerAssReportPossibleHandoverCache);

      //sched.Start();

      _logger.Information("[FalconService] - Schedulers created.");
    }
  }
}