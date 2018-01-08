using System;
using Autofac;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;
using Stream.Framework.Services;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamRemoveDuplicateAccount")]
  [TriggerName("StreamRemoveDuplicateAccount")]
  [CronExpression("0 0/1 * 1/1 * ? *")] // runs every 1 minute for testing
  public class StreamRemoveDuplicateAccount : IJob
  {

    private readonly ILifetimeScope _scope;
    private readonly IStreamService _streamService;
    private readonly ILogger _logger;

    public StreamRemoveDuplicateAccount(ILifetimeScope scope, IStreamService streamService, ILogger logger)
    {
      _scope = scope;
      _streamService = streamService;
      _logger = logger;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [StreamRemoveDuplicateAccount][COLLECTIONS]");

          _streamService.RemoveDuplicateAccounts(global::Stream.Framework.Enumerators.Stream.GroupType.Collections);

          _logger.Information("Started Job [StreamRemoveDuplicateAccount][SALES]");

          _streamService.RemoveDuplicateAccounts(global::Stream.Framework.Enumerators.Stream.GroupType.Sales);

          _logger.Information("Finished Job [StreamRemoveDuplicateAccount]");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [StreamRemoveDuplicateAccount]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}
