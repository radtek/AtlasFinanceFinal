using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Common;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using Magnum;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Workflow
{
  public class JobMonitor : IDisposable
  {
    private ILogger _logger = null;
    private Timer _timer;
    private bool _canCheck;

    public ConcurrentBag<IJob> _processesToRun;

    public JobMonitor(ILogger ilogger, ConcurrentBag<IJob> processToRun)
    {
      _logger = ilogger;
      _processesToRun = processToRun;
    }

    public void Start()
    {
      _logger.Info("[Job Monitor] Starting up scheduled jobs...");

      Task.Factory.StartNew(() =>
      {
        _processesToRun.ToList().ForEach(_ => _.StartSchedule());
      });

      _logger.Info("[Job Monitor] Starting job monitoring timer");

      _timer = new Timer(CheckJob, null, 0, 3000);
      _canCheck = true;

      _logger.Info("[Timer] Job monitoring timer Started");
    }

    private void CheckJob(object state)
    {
      if (_canCheck)
      {
        _canCheck = false;

        _logger.Info("[Timer] Started monitoring jobs..");

        foreach (var processRunning in _processesToRun.Where(j => j.State == Enumerators.Workflow.JobState.Executing))
        {
          if (processRunning.ActiveStep.ProcessStep.Threshold > 0
            && processRunning.ActiveStep.ProcessStep.ThresholdPeriodFrequency != null)
          {
            var runningTime = Convert.ToInt32(DateTime.Now.Subtract(processRunning.ActiveStep.StartDate).TotalMilliseconds);
            var thresholdTime = Util.ConvertToMilliseconds(processRunning.ActiveStep.ProcessStep.Threshold, processRunning.ActiveStep.ProcessStep.ThresholdPeriodFrequency.Type);

            if (runningTime > thresholdTime)
            {
              // Process running longer than expected
              // Escalate
              _logger.Warn(string.Format("[Timer] Process{0} taking too long to execute..", processRunning.ActiveStep.ThreadId));
            }
          }
        }

        _logger.Info("[Timer] Finished monitoring jobs..");

        _canCheck = true;
      }
      else
      {
        _logger.Warn("[Timer] Cannot Monitor Jobs. Previous timer tick still monitoring jobs..");
      }
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~JobMonitor()
    {
      Dispose(false);
    }
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (_timer != null)
        {
          _timer.Dispose();
          _timer = null;
        }
      }
    }
  }
}
