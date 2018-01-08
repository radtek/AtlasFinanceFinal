using System;
using Atlas.Common.Extensions;
using Falcon.Common.Interfaces.Jobs;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;
using Stream.Framework.Repository;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  [DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamReportCacher")]
  [TriggerName("StreamReportCacher")]
  [CronExpression("0 0/5 * 1/1 * ? *")] // runs every 5 minutes
  //[CronExpression("0 0/1 * 1/1 * ? *")] //run every minute for testing purposes
  public class StreamReportCacher : IStreamReportCacher
  {
    private readonly ILogger _logger;
    private readonly IStreamReportRepository _streamReportRepository;

    public StreamReportCacher(ILogger logger, IStreamReportRepository streamReportRepository)
    {
      _logger = logger;
      _streamReportRepository = streamReportRepository;
    }


    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _logger.Information("[StreamReportCacher] - Started Job");

        var startDate = DateTime.Today.AddDays(-DateTime.Today.Day).AddDays(1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // go back 36 months, including this month, 37
        for (var i = 0; i >= -36; i--)
        {
          foreach (var groupType in EnumUtil.GetValues<global::Stream.Framework.Enumerators.Stream.GroupType>())
          {
            if (i == -36)
              _streamReportRepository.CacheReportData(groupType, DateTime.Today.Date, DateTime.Today.Date);
            _streamReportRepository.CacheReportData(groupType, startDate, endDate);
          }
          startDate = startDate.AddMonths(i);
          endDate = startDate.AddMonths(1).AddDays(-1);
        }
        _logger.Information("[StreamReportCacher] - Finished Job");
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("[StreamReportCacher] - {0}, {1}", ex.Message, ex.StackTrace));
      }
    }
  }
}
