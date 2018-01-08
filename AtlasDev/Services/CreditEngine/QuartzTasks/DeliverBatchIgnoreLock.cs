using Quartz;
using Serilog;

using Atlas.ThirdParty.CompuScan.Batch;


namespace Atlas.Credit.Engine.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class DeliverBatchIgnoreLock : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<DeliverBatch>();

    public void Execute(IJobExecutionContext context)
    {
      _log.Information("Job DeliverBatchIngoreLock is executing...");

      BatchServletImpl batchImpl = new BatchServletImpl();
      batchImpl.DeliverBatch(true);

      _log.Information("DeliverBatchIngoreLock execution.");
    }
  }
}
