using Atlas.Notification.Server.Cache;
using log4net;
using Quartz;

namespace Atlas.Notification.Server.Tasks
{
  public sealed class PersistNotificationCache : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(PersistNotificationCache));

    public void Execute(IJobExecutionContext context)
    {
      PendingCache.Write();
    }
  }
}