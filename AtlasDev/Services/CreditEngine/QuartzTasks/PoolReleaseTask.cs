using Quartz;
using Serilog;

using Atlas.ThirdParty.CompuScan.Batch;


namespace Atlas.Credit.Engine.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class PoolReleaseTask : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<PoolReleaseTask>();

    public void Execute(IJobExecutionContext context)
    {
      _log.Information("PoolReleaseTask is executing...");

      Npgsql.NpgsqlConnection.ClearAllPools();

      _log.Information("PoolReleaseTask done.");
    }
  }
}
