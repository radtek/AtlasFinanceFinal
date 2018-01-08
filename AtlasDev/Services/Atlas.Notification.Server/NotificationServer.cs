using Atlas.Online.Node.Core;
using Atlas.Notification.Server.Cache;
using Atlas.Notification.Server.Handlers;
using Atlas.RabbitMQ.Messages.Notification;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using Quartz;
using Atlas.Notification.Server.Tasks;
using Quartz.Impl;
using System.Linq;
using Atlas.Notification.Server.EasyNetQ;
using EasyNetQ;

namespace Atlas.Notification.Server
{
  public sealed class NotificationServer : IService
  {
    private static ILogger _logger = null;
    private static IKernel _kernal = null;
    private ISchedulerFactory _schedFact;

    public NotificationServer(ILogger ilogger, IKernel ikernal)
    {
      _logger = ilogger;
      _kernal = ikernal;
    }

    public void Start()
    {
      try
      {
        _logger.Info("[NotificationServer] - Starting...");

        _schedFact = new StdSchedulerFactory(/*props*/);
        var sched = _schedFact.GetScheduler();

        var atlasOnlineServiceBus = _kernal.Get<AtlasOnlineServiceBus>();

        atlasOnlineServiceBus.GetServiceBus().Subscribe<EmailNotifyMessage>("queue_EmailNotifyMessage", EmailHandle.Send);
        atlasOnlineServiceBus.GetServiceBus().Subscribe<EventLogNotifyMessage>("queue_EventLogNotifyMessage", EventLogHandle.Send);

        atlasOnlineServiceBus.GetServiceBus().Subscribe<SMSNotifyMessage>("queue_SMSNotifyMessage",
          msg =>
          {
            new NotificationServiceConsumers().Consume(msg);
          });

        atlasOnlineServiceBus.GetServiceBus().Subscribe<SMSNotifyUpdateWithStatus>("queue_SMSNotifyUpdateWithStatus",
          msg =>
          {
            new NotificationServiceConsumers().Consume(msg);
          });

        /* Load Cache */
        _logger.Info("[NotificationServer] - Loading Cache...");
        if (PendingCache.Load() == true)
        {
          _logger.Info("[NotificationServer] - Cache Loaded.");
          var task = JobBuilder.Create<NormalPriorityNotification>().WithIdentity("NormalPriorityTask", "Tasks").Build();

          var trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("NormalPriorityTaskFetch")
                .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(2).RepeatForever().WithMisfireHandlingInstructionFireNow())
                .StartNow()
               .Build();

          sched.ScheduleJob(task, trigger);

          var taskPerist = JobBuilder.Create<PersistNotificationCache>().WithIdentity("PersistTask", "Tasks").Build();

          var persistTrigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("persistTrigger")
                .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInSeconds(50).RepeatForever().WithMisfireHandlingInstructionFireNow())
                .StartNow()
               .Build();

          sched.ScheduleJob(taskPerist, persistTrigger);

          var taskRetrieveReplies = JobBuilder.Create<RetrieveSMSReplyTask>().WithIdentity("taskRetrieveReplies", "Tasks").Build();

          var replyTrigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("replyTrigger")
                .WithSchedule(SimpleScheduleBuilder.Create().WithIntervalInMinutes(2).RepeatForever().WithMisfireHandlingInstructionFireNow())
                .StartNow()
               .Build();

          sched.ScheduleJob(taskRetrieveReplies, replyTrigger);


          sched.Start();
        }

        ServiceLocator.SetServiceLocator(_kernal);

        _logger.Info("[NotificationServer] - Started.");
      }
      catch (Exception exception)
      {
        _logger.Error(string.Format("[NotificationServer] - Message: {0} Inner Exception: {1} Stack: {2}",
          exception.Message + Environment.NewLine, exception.InnerException + Environment.NewLine, exception.StackTrace + Environment.NewLine));
      }
    }

    public void Stop()
    {
      if (PendingCache.Count() > 0)
      {
        _logger.Warn("[NotificationServer] - Pending items in cache waiting to be sent...");
        _logger.Warn("[NotificationServer] - Forcing sending of cached items...");
        CachePurgeNotification.Execute(PendingCache.GetAll().Keys.ToList());
        _logger.Warn("[NotificationServer] - Pending cache items sent.");
      }
      _logger.Info("[NotificationServer] - Writing cache file...");
      PendingCache.Write();
      _logger.Info("[NotificationServer] - Shutting Down.");
      _kernal.Dispose();
      _logger.Info("[NotificationServer] - Shut Down Complete.");
    }
  }
}
