using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.RabbitMQ.Messages.Notification;
using Falcon.Common.Interfaces.Jobs;
using Falcon.Common.Interfaces.Services;
using Falcon.DuckHawk.Jobs.Attributes;
using MassTransit;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using StackExchange.Redis;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.UserTracking
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("UserTrackingReporter")]
  [TriggerName("UserTrackingReporter")]
  [CronExpression("0 0/5 * 1/1 * ? *")]
  public class UserTrackingReporter : IUserTrackingReporter
  {

    private readonly TimeSpan _businessStartTime = new TimeSpan(08, 00, 00);
    private readonly TimeSpan _businessEndTime = new TimeSpan(17, 00, 00);

    readonly IDatabase _redis;
    readonly IEmailService _emailService;
    readonly ISmsService _smsService;
    readonly ILogger _logger;
    readonly IUserTrackingService _userTrackingService;
    readonly IServiceBus _serviceBus;

    struct ViolationRec
    {
      public long PinnedUserId;
      public int ViolationCount;

      public ViolationRec(long pinnedUserId, int violationCount)
      {
        PinnedUserId = pinnedUserId;
        ViolationCount = violationCount;
      }
    }


    public UserTrackingReporter(IDatabase redis, IEmailService emailService, ISmsService smsService, ILogger logger, IUserTrackingService userTrackingService, IServiceBus serviceBus)
    {
      _redis = redis;
      _emailService = emailService;
      _smsService = smsService;
      _logger = logger;
      _userTrackingService = userTrackingService;
      _serviceBus = serviceBus;
    }


    public void Execute(IJobExecutionContext context)
    {
      if (DateTime.Now.TimeOfDay >= _businessStartTime && DateTime.Now.TimeOfDay <= _businessEndTime)
      {
        _logger.Information("[UserTrackingReporter] - Start");
        var violations = _userTrackingService.GetViolations(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00), DateTime.Now);       
        if (violations.Count > 0)
        {
          foreach (var violation in violations)
          {
            var redisNotificationKey = string.Format("falcon.user.tracking.notification.key.{0}", violation.PinnedUserId);
            List<ViolationRec> redisStoredViolations = null;
            var redisViolations = _redis.StringGet(redisNotificationKey);

            if (!redisViolations.IsNull)
              redisStoredViolations = JsonConvert.DeserializeObject<List<ViolationRec>>(redisViolations);

            if (redisStoredViolations == null)
            {
              redisStoredViolations = new List<ViolationRec>
              {
                new ViolationRec(violation.PinnedUserId, violation.ViolationCount)
              };
            }
            else
            {
              var violationMatchCount = redisStoredViolations.FirstOrDefault(p => p.ViolationCount == violation.ViolationCount);
              if (violationMatchCount.PinnedUserId == 0 && violationMatchCount.ViolationCount == 0)
              {
                // Fetch matching pinnedUserId regardless of count, and update the count.
                var matchPinn = redisStoredViolations.FirstOrDefault(p => p.PinnedUserId == violation.PinnedUserId);

                if (matchPinn.PinnedUserId != 0)
                  matchPinn.ViolationCount = violation.ViolationCount;

                // Calculate elapsed time.
                var elapsed = (violation.ViolationCount * violation.RuleSet.Value);

                // get recipients.
                var recipients = _redis.StringGet(string.Format("{0}-{1}", violation.PinnedUserId, violation.RuleSet.RuleSetId));

                // update Template
                var template = Properties.Resources.Warden_Notification_Template;

                template = template.Replace("%%USER%%", string.Format("{0} {1}", violation.FirstName, violation.LastName));
                template = template.Replace("%%PERIOD%%", string.Format("{0} {1}", elapsed, violation.RuleSet.Elapse.ToStringEnum()));
                template = template.Replace("%%VIOLATIONCOUNT%%", string.Format("{0} - Where {1} equals {2} {3}",
                  violation.ViolationCount, 1, violation.RuleSet.Value, violation.RuleSet.Elapse.ToStringEnum()));
                template = template.Replace("%%LASTBRANCHACTIVITY%%", violation.LastBranchActivity);
                template = template.Replace("%%SENTAT%%", DateTime.Now.ToString("yyyy/MM/dd HH:dd:MM"));

                // Send alert
                switch (violation.RuleSet.AlertType)
                {
                  case Atlas.Enumerators.Tracking.AlertType.Email:
                    _serviceBus.Publish(new EmailNotifyMessage(Magnum.CombGuid.Generate())
                      {
                        ActionDate = DateTime.Now,
                        From = "usertracker@atlasonline.co.za",
                        IsHTML = true,
                        Body = template,
                        Priority = Atlas.Enumerators.Notification.NotificationPriority.High,
                        Subject = string.Format("User {0} has not been active for {1} {2}", string.Format("{0} {1}", violation.FirstName, violation.LastName),
                        elapsed, violation.RuleSet.Elapse.ToStringEnum()),
                        To = recipients,
                        Attachments = new List<Tuple<string, string, string>>()
                      });
                    break;
                  case Atlas.Enumerators.Tracking.AlertType.SMS:
                    _serviceBus.Publish(new SMSNotifyMessage(Magnum.CombGuid.Generate())
                    {
                      ActionDate = DateTime.Now,
                      Body = string.Format("User {0} has not been active for {1}, Last activity was ", string.Format("{0} {1}", violation.FirstName, violation.LastName),violation.LastBranchActivity),
                      Priority = Atlas.Enumerators.Notification.NotificationPriority.High,
                      To = recipients
                    });
                    break;
                }
                redisStoredViolations.Clear();
                redisStoredViolations.Add(new ViolationRec { PinnedUserId = violation.PinnedUserId, ViolationCount = violation.ViolationCount });
              }
            }
            _redis.StringSet(redisNotificationKey, JsonConvert.SerializeObject(redisStoredViolations));
          }
        }
        _logger.Information("[UserTrackingReporter] - End");
      }
    }

  }
}