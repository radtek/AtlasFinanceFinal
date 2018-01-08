using System;
using System.Linq;
using System.ServiceProcess;

using Serilog;
using Quartz;

using ASSSyncClient.Utils;


namespace ASSSyncClient.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class PostgresCheckService : IJob
  {
    /// <summary>
    /// Checks that the PostgreSQL service is running- if cannot start after 5 consecutive failed attempts, it initiates a reboot
    /// </summary>
    /// <param name="context"></param>
    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      var methodName = "PostgresCheckService.Execute";
      try
      {
        var postgreSQLService = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName.ToLower().Contains("postgresql"));
        if (postgreSQLService == null)
        {
          var error = new Exception("PostgreSQL Server service must be installed- service could not be located");
          LogEvents.Log(DateTime.Now, "PostgresCheckService.Execute", error.Message, 5);
          _log.Fatal(error, "{MethodName}", methodName);
          return;
        }

        if (postgreSQLService.Status != ServiceControllerStatus.Running)
        {
          LogEvents.Log(DateTime.Now, "PostgresCheckService.Execute", "Service not running- attempting to start", 10);
          postgreSQLService.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
          LogEvents.Log(DateTime.Now, "PostgresCheckService.Execute", "Service successfully started", 10);
        }

        _failCount = 0;
      }
      catch (Exception err)
      {
        _failCount++;         
        LogEvents.Log(DateTime.Now, "PostgresCheckService.Execute", err.Message, _failCount + 10);
        _log.Error(err, "{MethodName}", methodName);
      }

      if (_failCount >= 5)
      {
        _failCount = 0;

        _log.Warning("{MethodName} PostgreSQL not starting- forcing a reboot", methodName);
        LogEvents.Log(DateTime.Now, "PostgresCheckService.Execute", "PostgreSQL did not respond to start requests- forcing a reboot", 15);

        var info = ASSSyncClient.Utils.WCF.SyncSourceRequest.CreateSourceRequest();
        ASSSyncClient.Utils.Alerting.SendEMail.SendError(_log,
          string.Format(
            "WARNING: PostgreSQL server for branch '{0}' (machine {1})- service not responding- server is being forcefully rebooted", info.BranchCode, info.MachineName),
          string.Format(
            "The branch PostgreSQL database service is unresponsive to multiple start requests.\r\n" +
            "To ensure reliable operation, the machine will now be forcefully re-booted. If this error\r\n" +
            "persists, please manually check the machine:\r\n" +
            "Machine Name: {0}\r\n" +
            "Machine IPs: {1}\r\n" +
            "Machine date/time: {2:yyyy-MM-dd HH:mm:ss}", info.MachineName, info.MachineIPAddresses, info.MachineDateTime), false);
        ASSSyncClient.API.Windows.NativeMethods.Reboot();
      }     
    }

        
    #region Private vars

    /// <summary>
    /// Logging
    /// </summary>
    private static readonly ILogger _log = Log.Logger.ForContext<PostgresCheckService>();

    /// <summary>
    /// Consecutive fail counts
    /// </summary>
    private static int _failCount = 0;

    #endregion

  }
}
