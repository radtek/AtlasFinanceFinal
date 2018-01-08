using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Autofac;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.DuckHawk.Jobs.Attributes;
using MassTransit;
using Quartz;
using Serilog;
using Stream.Framework.Repository;
using Stream.Framework.Services;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  [DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamUpdateCategory")]
  [TriggerName("StreamUpdateCategory")]
  //[CronExpression("0 00 7,19,20 1/1 * ? *")] // run every day @ 07:00 && 15:00 && 19:00
  //[CronExpression("0 0/1 * 1/1 * ? *")] // runs every 1 minute for testing
  [ScheduleBuilder]
  public class StreamUpdateCategory : IJob
  {
    public IScheduleBuilder Schedule
    {
      get
      {
        return SimpleScheduleBuilder.Create().WithIntervalInSeconds(15).WithRepeatCount(0).WithMisfireHandlingInstructionFireNow();
      }
    }
    
    private readonly ILifetimeScope _scope;
    private readonly IStreamRepository _streamRepository;
    private readonly IStreamService _streamService;
    private readonly ILogger _logger;

    public StreamUpdateCategory(ILifetimeScope scope, IStreamRepository streamRepository, IStreamService streamService, ILogger logger)
    {
      _scope = scope;
      _streamRepository = streamRepository;
      _streamService = streamService;
      _logger = logger;
    }

    public StreamUpdateCategory()
    {
      
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [StreamUpdateCategory]");

          // get all cases
          //var cases =
          //  _streamRepository.GetCasesByStatus(global::Stream.Framework.Enumerators.Stream.GroupType.Collections,
          //    global::Stream.Framework.Enumerators.CaseStatus.Type.New,
          //    global::Stream.Framework.Enumerators.CaseStatus.Type.InProgress,
          //    global::Stream.Framework.Enumerators.CaseStatus.Type.OnHold).Where(c => c.CaseId == 4115984);

          //Parallel.ForEach(cases, new ParallelOptions
          //{
          //  MaxDegreeOfParallelism = 15
          //}, streamCase =>
          //{
          //  _logger.Debug("Job [StreamUpdateCategory] sales - " + streamCase.CaseId);
          //  try
          //  {
          //    _streamService.SetCaseCategory(streamCase.CaseId, global::Stream.Framework.Enumerators.Stream.GroupType.Sales);
          //  }
          //  catch (Exception ex)
          //  {
          //    _logger.Error("Job [StreamUpdateCategory] sales end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
          //  }
          //});

          //Parallel.ForEach(cases, new ParallelOptions
          //{
          //  MaxDegreeOfParallelism = 15
          //}, streamCase =>
          //{
          //  _logger.Debug("Job [StreamUpdatePriorityId] collections - " + streamCase.CaseId);
          //  try
          //  {
          //    _streamService.SetCasePriority(streamCase.CaseId, global::Stream.Framework.Enumerators.Stream.GroupType.Collections);
          //  }
          //  catch (Exception ex)
          //  {
          //    _logger.Error("Job [StreamUpdatePriorityId] collections end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
          //  }
          //});

          //Parallel.ForEach(cases, new ParallelOptions
          //{
          //  MaxDegreeOfParallelism = 15
          //}, streamCase =>
          //{
          //  _logger.Debug("Job [StreamUpdateCategory] collections - " + streamCase.CaseId);
          //  try
          //  {
          //    _streamService.SetCaseCategory(streamCase.CaseId, global::Stream.Framework.Enumerators.Stream.GroupType.Collections);
          //  }
          //  catch (Exception ex)
          //  {
          //    _logger.Error("Job [StreamUpdateCategory] collections end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
          //  }
          //});

          _logger.Information("Finished Job [StreamUpdateCategory]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamUpdateCategory]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}
