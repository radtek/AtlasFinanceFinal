using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Falcon.Common.Interfaces.Jobs;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;
using Stream.Framework.Repository;
using Stream.Framework.Services;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamOnceADayJob")]
  [TriggerName("StreamOnceADayJob")]
  [CronExpression("0 0 22 1/1 * ? *")] // run every night @ 22:00
  //[CronExpression("0 0/1 * 1/1 * ? *")] //run every minute for testing purposes
  public class StreamOnceADayJob : IStreamOnceADayJob
  {
    private readonly IStreamRepository _streamRepository;
    private readonly IStreamService _streamService;
    private readonly ILogger _logger;
    private readonly ILifetimeScope _scope;

    public StreamOnceADayJob(ILifetimeScope scope, IStreamRepository streamRepository, IStreamService streamService, ILogger logger)
    {
      _scope = scope;
      _streamRepository = streamRepository;
      _streamService = streamService;
      _logger = logger;
    }


    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [StreamOnceADayJob]");

          _logger.Information("Working Job [StreamOnceADayJob] - Escalate cases not worked - DISABLED FOR NOW");
          //_streamRepository.EscalateUnworkedCases();

          _logger.Information("Working Job [StreamOnceADayJob] - Break Outstanding PTC");
          _streamRepository.BreakOutstandingPtCs();

          var casesCollectionsToUpdateCategory =
            _streamRepository.GetCasesByStatus(global::Stream.Framework.Enumerators.Stream.GroupType.Collections,
              global::Stream.Framework.Enumerators.CaseStatus.Type.New,
              global::Stream.Framework.Enumerators.CaseStatus.Type.InProgress,
              global::Stream.Framework.Enumerators.CaseStatus.Type.OnHold);
          Parallel.ForEach(casesCollectionsToUpdateCategory, new ParallelOptions
          {
            MaxDegreeOfParallelism = 15
          }, streamCase =>
          {
            _logger.Debug("Job [StreamUpdateCategory] collections - " + streamCase.CaseId);
            try
            {
              _streamService.SetCaseCategory(streamCase.CaseId, global::Stream.Framework.Enumerators.Stream.GroupType.Collections);
            }
            catch (Exception ex)
            {
              _logger.Error("Job [StreamUpdateCategory] collections end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
            }
          });

          var casesSalesToUpdateCategory =
            _streamRepository.GetCasesByStatus(global::Stream.Framework.Enumerators.Stream.GroupType.Sales,
              global::Stream.Framework.Enumerators.CaseStatus.Type.New,
              global::Stream.Framework.Enumerators.CaseStatus.Type.InProgress,
              global::Stream.Framework.Enumerators.CaseStatus.Type.OnHold);
          Parallel.ForEach(casesSalesToUpdateCategory, new ParallelOptions
          {
            MaxDegreeOfParallelism = 15
          }, streamCase =>
          {
            _logger.Debug("Job [StreamUpdateCategory] sales - " + streamCase.CaseId);
            try
            {
              _streamService.SetCaseCategory(streamCase.CaseId, global::Stream.Framework.Enumerators.Stream.GroupType.Sales);
            }
            catch (Exception ex)
            {
              _logger.Error("Job [StreamUpdateCategory] sales end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
            }
          });

          _logger.Information("Finished Job [StreamOnceADayJob]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamOnceADayJob]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}
