using System;
using Quartz;
using Serilog;

using Atlas.ThirdParty.CompuScan.Batch;


namespace Atlas.Credit.Engine.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class PickUpBatch : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<PickUpBatch>();

    public void Execute(IJobExecutionContext context)
    {
      _log.Information("Job PickUpBatch Executing...");

      BatchServletImpl batchImpl = new BatchServletImpl();
      try
      {
        batchImpl.RetrieveJobStatus();
      }
      catch (Exception exception)
      {
        _log.Error(string.Format("Error retrieving batch status: {0} - {1}", exception.Message, exception.StackTrace));
      }
      try
      {
        batchImpl.RetrieveJob();
      }
      catch (Exception exception)
      {
        _log.Error(string.Format("Error retrieving batch: {0} - {1}", exception.Message, exception.StackTrace));
      }
      _log.Information("PickUpBatch finished executing.");
    }
  }
}
