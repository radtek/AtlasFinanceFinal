using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.RabbitMQ.Messages.Notification;
using Autofac;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Jobs;
using Falcon.Common.Interfaces.Services;
using Falcon.DuckHawk.Jobs.Attributes;
using Magnum;
using MassTransit;
using Quartz;
using Serilog;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.ASS
{
  [DisableJob]
  [DisallowConcurrentExecution]
  [JobName("AssCiReportEmailerTest")]
  [TriggerName("AssCiReportEmailerTest")]
  //[CronExpression("0 0 7,11,15,19,20 1/1 * ? *")] // runs every day @ 07:00, 11:00, 15:00, 19:00 and 20:00
  [CronExpression("0 0/1 * 1/1 * ? *")] //run every minute for testing purposes
  public class AssCiReportEmailerTest : IAssCiReportEmailerJob
  {
    private readonly IServiceBus _bus;
    private readonly ILifetimeScope _scope;
    private readonly ILogger _logger;
    private readonly ICiReportService _ciReportService;

    public AssCiReportEmailerTest(IServiceBus bus, ILifetimeScope scope, ILogger logger, ICiReportService ciReportService)
    {
      _bus = bus;
      _scope = scope;
      _logger = logger;
      _ciReportService = ciReportService;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [AssCiReportEmailerTest]");

          _logger.Information("[AssCiImportTest]: Get All Active branches");
          var notAllowedBranches = new long[] { 1, 18, 21, 84, 111, 113, 149, 152, 167, 207 };

          var startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
          var endDate = startDate.AddMonths(1).AddDays(-1);
          if (DateTime.Now.Hour == 19 || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 15)
          {
            startDate = endDate = DateTime.Today;
          }

          var attachment = new List<Tuple<string, string, string>>();
          using (var uow = new UnitOfWork())
          {
            var allowedRegions = new XPQuery<Region>(uow).Select(r=>r.RegionId).ToArray();
            for (var i = 0; i <= allowedRegions.Length; i++)
            {
              var tempRegions = new List<long>();
              if (i == allowedRegions.Length)
                tempRegions = allowedRegions.ToList();
              else
                tempRegions.Add(allowedRegions[i]);
              var branches =
                new XPQuery<BRN_Branch>(uow).Where(
                  b =>
                    b.BranchId > 1 && !b.IsClosed && !notAllowedBranches.Contains(b.BranchId) &&
                    tempRegions.Contains(b.Region.RegionId))
                  .Select(b => new {b.BranchId, b.LegacyBranchNum})
                  .ToArray();

              if (branches.Any())
              {
                var extract = _ciReportService.GetCiReport(startDate, endDate, branches.Select(b => b.BranchId).ToList());
                var data = Base64.EncodeString(extract);
                var branch = branches.FirstOrDefault();
                if (branch != null)
                  attachment.Add(
                    new Tuple<string, string, string>(
                      string.Format("CIReport_{0}_{1}_{2}",
                        i == allowedRegions.Length ? "AllRegions" : branch.LegacyBranchNum,
                        startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")), ".xlsx", data));
              }
            }
          }

          _bus.Publish(new EmailNotifyMessage(CombGuid.Generate())
          {
            CreatedAt = DateTime.Now,
            ActionDate = DateTime.Today,
            Body = string.Format("Hi, {0}{0}Please see attached CI Report.{0}{0}Regards,{0}Falcon", Environment.NewLine),
            From = "falcon@atcorp.co.za",
            IsHTML = false,
            Attachments = attachment,
            Priority = Notification.NotificationPriority.High,
            Subject =
              string.Format("CI Report dated: {0} to {1}", startDate.ToString("dd MMM yyyy"),
                endDate.ToString("dd MMM yyyy")),
            //To = toEmailGroup
            To = "lee@atcorp.co.za"
          });

          _logger.Information("Finished Job [AssCiReportEmailerTest]");

        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [AssCiReportEmailerTest]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}