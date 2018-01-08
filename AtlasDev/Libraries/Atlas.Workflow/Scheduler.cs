using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Common;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using Magnum;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow
{
  public class Scheduler
  {
    public ConcurrentBag<IJob> _processesToRun;
    private ILogger _logger = null;

    public Scheduler(ILogger ilogger)
    {
      _logger = ilogger;
      _processesToRun = new ConcurrentBag<IJob>();
    }

    public ConcurrentBag<IJob> ScheduleJobs()
    {
      using (var uow = new UnitOfWork())
      {
        var nextSunday = Util.GetNextSundayDate().Date;

        List<WFL_ScheduleProcess> scheduledProcessCollection = new XPQuery<WFL_ScheduleProcess>(uow).Where(_ =>
          (_.Iteration == 0
              || _.Iteration >= _.CurrentIteration)
          && _.Enabled).ToList();

        if (scheduledProcessCollection.Count > 0)
        {
          foreach (var scheduleProcess in scheduledProcessCollection)
          {
            IJob job = new Job(AutoMapper.Mapper.Map<WFL_Process, WFL_ProcessDTO>(scheduleProcess.Process));

            job.StartDate = scheduleProcess.Start;
            job.EndDate = scheduleProcess.End;
            job.LastExecutionDate = scheduleProcess.LastExecutionDate;
            job.NextExecutionDate = scheduleProcess.NextExecutionDate;
            job.Frequency = scheduleProcess.ScheduleFrequency.Type;

            var scheduleProcessSteps = new XPQuery<WFL_ScheduleProcessStep>(uow).Where(_ =>
              _.ScheduleProcess.ScheduleProcessId == scheduleProcess.ScheduleProcessId).OrderBy(_ => _.Rank).ToList();

            if (scheduleProcessSteps.Count == 0)
            {
              _logger.Fatal(string.Format("[Workflow] - No steps found for scheduled process ({0})", scheduleProcess.ScheduleProcessId));
              job.State = Enumerators.Workflow.JobState.Faulty;
            }
            else
            {
              foreach (var scheduleProcessStep in scheduleProcessSteps)
              {
                Assembly assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\" + scheduleProcessStep.ProcessStep.Process.Assembly);
                Type type = assembly.GetType(scheduleProcessStep.ProcessStep.Namespace);
                dynamic step = Activator.CreateInstance(type, BindingFlags.Public | BindingFlags.Instance,
                                                             null, new object[] { job }, null);

                step.Logger = _logger;
                step.ProcessStep = AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(scheduleProcessStep.ProcessStep);
                step.Delay = scheduleProcessStep.Delay;
                step.DelayPeriodFrequency = scheduleProcessStep.DelayPeriodFrequency == null ? (Enumerators.Workflow.PeriodFrequency?)null : scheduleProcessStep.DelayPeriodFrequency.Type;
                step.UseDecisionGate = scheduleProcessStep.UseDecisionGate;
                step.Rank = scheduleProcessStep.Rank;

                job.Steps.Add(step);
              }
            }

            _processesToRun.Add(job);
          }
        }
        return _processesToRun;
      }
    }
  }
}
