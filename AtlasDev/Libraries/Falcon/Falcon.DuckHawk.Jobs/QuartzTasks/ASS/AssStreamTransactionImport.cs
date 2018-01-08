using System;
using Atlas.Enumerators;
using Autofac;
using Falcon.Common.Interfaces.Jobs;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;
using Stream.Framework.Services;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.ASS
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("AssStreamTransactionImport")]
  [TriggerName("AssStreamTransactionImport")]
  //[CronExpression("0 0 8,20 1/1 * ? *")] // run every day @ 20:00 AND 07:00
  [CronExpression("0 0/1 * 1/1 * ? *")] // runs every 1 minute for testing
  public class AssStreamTransactionImport : IAssStreamTransactionImportJob
  {
    private readonly IStreamService _streamService;
    private readonly ILogger _logger;
    private readonly ILifetimeScope _scope;

    public AssStreamTransactionImport(ILogger logger, ILifetimeScope scope, IStreamService streamService)
    {
      _scope = scope;
      _logger = logger;
      _streamService = streamService;
    }

    public void Execute(IJobExecutionContext context)
    {
      using (_scope.BeginLifetimeScope())
      {
        try
        {
          _logger.Information("Started Job [AssStreamTransactionImport]");
          try
          {
            _logger.Information("Working Job [AssStreamTransactionImport] - Importing New Transactions");

            _streamService.ImportCollectionsTransactions();
            _streamService.ImportSalesTransactions();


            _logger.Information("Finished Job [AssStreamTransactionImport]");
          }
          catch (Exception ex)
          {
            _logger.Error(
              string.Format("Error in Job [AssStreamTransactionImport] - Importing New Transactions: {0} - {1}",
                ex.Message, ex.StackTrace));
          }
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("Error in Job [AssStreamTransactionImport]: {0} - {1}", ex.Message, ex.StackTrace));
        }
      }
    }
  }
}