using System;

using DevExpress.Xpo;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.QuartzTasks
{
  // Task to delete old PC audits
  [global::Quartz.DisallowConcurrentExecution]  
  public class DeleteOldMachineAudits : global::Quartz.IJob
  {
    public DeleteOldMachineAudits(ILogging log)
    {
      _log = log;
    }


    public void Execute(global::Quartz.IJobExecutionContext context)
    {
      _log.Information("Old PC audits clean Quartz task starting...");

      try
      {
        using (var unitOfWork = new UnitOfWork())
        {
          unitOfWork.ExecuteNonQuery("DELETE FROM \"COR_LogMachineInfo\" WHERE \"CreatedDT\" < (current_timestamp - interval '6 months')");
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "Execute");
      }

      _log.Information("Old PC audits clean Quartz task completed");
    }

    
    #region Private vars

    private readonly ILogging _log;

    #endregion

  }
}
