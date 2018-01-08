using Falcon.Common.Interfaces.Jobs;
using Falcon.Common.Interfaces.Repositories;
using Falcon.DuckHawk.Jobs.Attributes;
using Quartz;
using Serilog;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.UserTracking
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("UserTrackingViolationReset")]
  [TriggerName("UserTrackingViolationReset")]
  [CronExpression("0 30 22 ? * MON-FRI *")]
  public class UserTrackingViolationReset : IUserTrackingViolationReset
  {
    readonly ILogger _logger;
    readonly IUserTrackingRepository _userTrackingRepository;
    public UserTrackingViolationReset(ILogger logger, IUserTrackingRepository userTrackingRepository)
    {
      _logger = logger;
      _userTrackingRepository = userTrackingRepository;
    }


    public  void Execute(IJobExecutionContext context)
    {
      _logger.Information("[UserTrackingViolationReset] - Start");

      if (_userTrackingRepository.ResetViolations(true))
        _logger.Information("[UserTrackingViolationReset] - Daily Violations Reset.");

      _logger.Information("[UserTrackingViolationReset] - End");
    }
  }
}
