using Autofac;
using Falcon.Web.Api.Interfaces;
using Falcon.Web.Api.Jobs.Attributes;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Falcon.Web.Api.Jobs
{
  [DisallowConcurrentExecution]
  [JobName("StreamPerformanceReport")]
  [TriggerName("StreamPerformanceReport")]
  [DisableJob]
  //[CronExpression("0 0/5 * 1/1 * ? *")] // run every 15 minutes
  [CronExpression("0 0/1 * 1/1 * ? *")] // run every 1 minute for testing
  public class StreamPerformanceReport : IStreamPerformanceReport
  {
    private IStreamReportRepository _streamReportRepository;
    private ILogger _logger;
    private ILifetimeScope _scope;
    public StreamPerformanceReport(ILifetimeScope scope, IStreamReportRepository streamReportRepository, ILogger logger)
    {
      _scope = scope;
      _streamReportRepository = streamReportRepository;
      _logger = logger;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (var scoped = _scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [StreamPerformanceReport]");

          // process cache for this month
          // startDate = start of this month, endDate = end of this month
          var startDate = DateTime.Today.AddDays(-DateTime.Today.Day + 1);
          for (var i = 0; i <= 6; i++)
          {
            var endDate = startDate.AddMonths(1).AddDays(-1);
            if (i == 0)
              _streamReportRepository.CacheReportData(startDate: DateTime.Today, endDate: DateTime.Today, expiryTime: new TimeSpan(0, 30, 0), ignoreIfKeyExists: false);
            else if (startDate.Date.Month == DateTime.Today.Month)
              _streamReportRepository.CacheReportData(startDate: startDate, endDate: endDate, expiryTime: new TimeSpan(0, 30, 0), ignoreIfKeyExists: false);
            else
              _streamReportRepository.CacheReportData(startDate: startDate, endDate: endDate, expiryTime: new TimeSpan(24, 0, 0), ignoreIfKeyExists: true);
            startDate = startDate.AddMonths(-1); // go back 1 month
          }

          _logger.Information("Finished Job [StreamPerformanceReport]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamPerformanceReport]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}