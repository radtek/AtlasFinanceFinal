using Quartz;
using Serilog;

using Atlas.ThirdParty.CompuScan.Batch;
using System;


namespace Atlas.Credit.Engine.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class DeliverBatch : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<DeliverBatch>();

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information("Job DeliverBatch is executing...");

        BatchServletImpl batchImpl = new BatchServletImpl();
        batchImpl.DeliverBatch(true);

        _log.Information("DeliverBatch execution.");
      }
      catch (Exception exception)
      {
        _log.Error(string.Format("Job DeliverBatch Failed: {0} - {1}", exception.Message, exception.StackTrace));
      }
    }
  }
}
