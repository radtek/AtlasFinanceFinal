using System;
using System.Configuration;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.RabbitMQ.Messages.Notification;
using Autofac;
using Falcon.Common.Interfaces.Jobs;
using Falcon.DuckHawk.Jobs.Attributes;
using Magnum;
using MassTransit;
using Quartz;
using Serilog;
using Stream.Framework.Repository;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamEmailer")]
  [TriggerName("StreamEmailer")]
  [CronExpression("0 00 7,19,20 1/1 * ? *")] // run every day @ 07:00 && 15:00 && 19:00
  //[CronExpression("0 0/1 * 1/1 * ? *")] // runs every 1 minute for testing
  public class StreamEmailer : IStreamEmailer
  {
    private readonly IServiceBus _bus;
    private readonly IStreamReportRepository _streamReportRepository;
    private readonly ILogger _logger;
    private readonly ILifetimeScope _scope;


    public StreamEmailer(IServiceBus bus, ILifetimeScope scope, IStreamReportRepository streamReportRepository,
      ILogger logger)
    {
      _bus = bus;
      _scope = scope;
      _streamReportRepository = streamReportRepository;
      _logger = logger;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [StreamEmailer]");

          _logger.Information("Working Job [StreamEmailer] - Getting Report");
          var startDate = DateTime.Today;
          var endDate = startDate;
          if (DateTime.Now.Hour == 20)
          {
            // monthly
            startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
            endDate = startDate.AddMonths(1).AddDays(-1);
          }

          var attachments = (from groupType in EnumUtil.GetValues<global::Stream.Framework.Enumerators.Stream.GroupType>()
            let report = _streamReportRepository.GetPerformanceReport(groupType, startDate, endDate)
            let data = Base64.EncodeString(report)
            select
              new Tuple<string, string, string>(
                string.Format("StreamReport_{0}_{1}_{2}", groupType.ToStringEnum(),
                  startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")), ".xlsx", data)).ToList();

          _logger.Information("Working Job [StreamEmailer] - Sending Reports");
          _bus.Publish(new EmailNotifyMessage(CombGuid.Generate())
          {
            CreatedAt = DateTime.Now,
            ActionDate = DateTime.Today,
            Body =
              string.Format("Hi, {0}{0}Please see attached Stream Performance Report.{0}{0}Regards,{0}Falcon",
                Environment.NewLine),
            Cc = "srinivasd@atcorp.co.za",
            From = "falcon@atcorp.co.za",
            IsHTML = false,
            Attachments = attachments,
            Priority = Atlas.Enumerators.Notification.NotificationPriority.High,
            Subject =
              string.Format("Stream Performance Report dated: {0} to {1}", startDate.ToString("dd MMM yyyy"),
                endDate.ToString("dd MMM yyyy")),
            To = ConfigurationManager.AppSettings["streamEmailList"] ?? "lee@atcorp.co.za"
          });

          _logger.Information("Finished Job [StreamEmailer]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamEmailer]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}