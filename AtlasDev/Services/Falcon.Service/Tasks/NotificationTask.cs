using System;
using Falcon.Service.Controllers;
using Quartz;
using Serilog;

namespace Falcon.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class NotificationTask : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<NotificationTask>();
    private static readonly NotificationController notificationController = new NotificationController();

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        _log.Information(string.Format("[FalconService][Task] {Job} Executing...", context.JobDetail.Key.Name));

        var priorityLevel = (DateTime.Now.Millisecond % 4);
        notificationController.Notify(priorityLevel == 0 ? 4 : priorityLevel, "Test message", "This was sent at " + DateTime.Now.ToString(), string.Empty, null, null);

        _log.Information(string.Format("[FalconService][Task] {Job} Finished...", context.JobDetail.Key.Name));
      }
      catch (Exception exception)
      {
        _log.Error(string.Format("[FalconService][Task] {Job}: {Message} - {Exception}", context.JobDetail.Key.Name, exception.Message, exception.StackTrace));
      }
    }
  }
}
