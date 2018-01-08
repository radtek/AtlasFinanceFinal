using Atlas.Online.Web.Service.Tasks;
using Ninject.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Service
{
  public sealed class TaskSpooler
  {
    private ILogger _logger = null;
    private ISchedulerFactory _schedFact;

    public TaskSpooler(ILogger logger)
    {
      _logger = logger;
    }

    public void Start()
    {
      _logger.Info("Preparing tasks...");

      _schedFact = new StdSchedulerFactory(/*props*/);

      //// Get a scheduler
      var sched = _schedFact.GetScheduler();

      var taskAVSCheckTask = JobBuilder.Create<AVSCheckTask>().WithIdentity("AVSCheckTask ", "Tasks").Build();

      var triggerAVSCheckTask = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerAVSCheckTask ")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskAVSCheckTask, triggerAVSCheckTask);

      var taskNotificationOfInactiveAccountTask = JobBuilder.Create<NotificationOfInactiveAccountTask>().WithIdentity("NotificationOfInactiveAccountTask ", "Tasks").Build();

      var triggerNotificationOfInactiveAccountTask = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerNotificationOfInactiveAccountTask ")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskNotificationOfInactiveAccountTask, triggerNotificationOfInactiveAccountTask);


      var taskSevenDayExpire = JobBuilder.Create<QuoteSevenDayExpireTask>().WithIdentity("QuoteSevenDayExpireTask", "Tasks").Build();

      var triggerSevenDayExpire = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerSevenDayExpire")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskSevenDayExpire, triggerSevenDayExpire);


      var taskExpire = JobBuilder.Create<QuotationExpiredTask>().WithIdentity("QuotationExpiredTask", "Tasks").Build();

      var triggerExpire = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerExpire")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskExpire, triggerExpire);

      var taskRecentlyPaid = JobBuilder.Create<NotificationOfRecentlyPaidTask>().WithIdentity("NotificationOfRecentlyPaidTask", "Tasks").Build();

      var triggerRecentlyPaid = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerRecentlyPaid")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskRecentlyPaid, triggerRecentlyPaid);

      var taskOverdue = JobBuilder.Create<OverduePaymentTask>().WithIdentity("NotificationOverduePaymentTask", "Tasks").Build();

      var triggerOverdue = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerOverdue")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskOverdue, triggerOverdue);


      var taskPaymentAcknowledgementTask = JobBuilder.Create<PaymentAcknowledgementTask>().WithIdentity("PaymentAcknowledgementTask", "Tasks").Build();

      var triggerPaymentAcknowledgementTask = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerPaymentAcknowledgementTask")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskPaymentAcknowledgementTask, triggerPaymentAcknowledgementTask);


      var taskReviewTechnicalToInactiveTask = JobBuilder.Create<ReviewTechnicalToInactiveTask>().WithIdentity("ReviewTechnicalToInactiveTask", "Tasks").Build();

      var triggerReviewTechnicalToInactiveTask = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerReviewTechnicalToInactiveTask")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskReviewTechnicalToInactiveTask, triggerReviewTechnicalToInactiveTask);

      var taskReminderOfPaymentDueTask = JobBuilder.Create<ReminderofPaymentDueTask>().WithIdentity("ReminderOfPaymentDueTask", "Tasks").Build();

      var triggerReminderOfPaymentDueTask = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity("triggerReminderOfPaymentDueTask")
            .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
            .StartNow()
           .Build();

      sched.ScheduleJob(taskReminderOfPaymentDueTask, triggerReminderOfPaymentDueTask);     
      
      sched.Start();

      _logger.Info("Schedulers created.");
    }
  }
}
