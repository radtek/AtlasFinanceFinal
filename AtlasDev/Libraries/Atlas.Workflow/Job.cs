using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Condition;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using Magnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.Workflow
{
  public class Job : IJob
  {
    public Guid Id { get; set; }
    public int ThreadId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? LastExecutionDate { get; set; }
    public DateTime? NextExecutionDate { get; set; }
    public IStep ActiveStep { get; set; }
    public bool Error { get; set; }
    public Enumerators.Workflow.JobState State { get; set; }
    public Enumerators.Workflow.ScheduleFrequency Frequency { get; set; }
    public ICollection<IStep> Steps { get; set; }
    public WFL_ProcessDTO Process { get; set; }
    public WFL_ProcessJobDTO ProcessJob { get; set; }
    public bool IsScheduled { get; set; }

    private IStep _lastStep { get; set; }
    private Timer _timer { get; set; }

    public Job(WFL_ProcessDTO process)
    {
      Id = CombGuid.Generate();
      ActiveStep = null;
      Steps = new List<IStep>();
      State = Enumerators.Workflow.JobState.Ready;

      Process = process;
    }

    public void StartJob()
    {
      StartNewJob();
    }

    public void GotoNextStep(IStep finishedStep, dynamic data = null)
    {
      _lastStep = finishedStep;
      var nextStep = GetNextStep();

      if (nextStep != null)
      {
        if (nextStep.ProcessStep.Process.ProcessId == ProcessJob.Process.ProcessId)
        {
          var timeToStartNextStep = 0;

          // Check Delay, and set if applicable
          if (nextStep.Delay > 0 && nextStep.DelayPeriodFrequency != null)
          {
            var delay = Common.Util.ConvertToMilliseconds(nextStep.Delay, nextStep.DelayPeriodFrequency);

            timeToStartNextStep = int.Parse((delay - Math.Round(DateTime.Now.Subtract((DateTime)_lastStep.CompleteDate).TotalMilliseconds)).ToString());
            timeToStartNextStep = timeToStartNextStep < 0 ? 0 : timeToStartNextStep;
          }

          var taskHolder = Task.Factory.StartNew(() =>
            {
              Thread.Sleep(timeToStartNextStep);
              if (data == null)
                StartStep(nextStep);
              else
                nextStep.Start(data);
            }, TaskCreationOptions.LongRunning);
        }
        else
        {
          CompleteProcessJob();

          Task.Factory.StartNew(() =>
          {
            var job = new Job(nextStep.ProcessStep.Process);

            dynamic step = Common.Step.CreateDynamicStepInstance(nextStep.ProcessStep.Process.Assembly, nextStep.ProcessStep.Namespace, new object[] { job });

            step.Rank = nextStep.ProcessStep.Rank;
            step.UseDecisionGate = true;
            step.ProcessStep = nextStep.ProcessStep;

            job.StartAtStep(step, data);
          });
        }
      }
      else
      {
        // Complete Job
        CompleteProcessJob();
        Console.WriteLine("Job Completeed");
      }
    }

    public IStep GetPreviousStep()
    {
      _lastStep = Steps.Where(s => s.Rank < ActiveStep.Rank).OrderByDescending(r => r.Rank).FirstOrDefault();

      if (_lastStep == null)
      {
        using (var uow = new UnitOfWork())
        {
          var processStepJob = new XPQuery<WFL_ProcessStepJob>(uow).Where(p => p.ProcessJob.ProcessJobId == ProcessJob.ProcessJobId
            && p.CompleteDate != null
            && p.JobState.Type == Enumerators.Workflow.JobState.Completed).OrderByDescending(p => p.CreateDate).FirstOrDefault();

          //if (processStepJob == null)
          //  throw new Exception("There is no step before this.");

          if (processStepJob != null)
          {
            dynamic step = Common.Step.CreateDynamicStepInstance(processStepJob.ProcessStep.Process.Assembly, processStepJob.ProcessStep.Namespace, new object[] { this });

            // TODO: Add logger
            //step.Logger = _logger;

            step.UseDecisionGate = true;
            step.ProcessStep = AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(processStepJob.ProcessStep);

            _lastStep = step;
          }
        }
      }
      return _lastStep;
    }

    public IStep GetNextStep()
    {
      if (ActiveStep.UseDecisionGate)
      {
        var nextStep = DecisionGate.DecisionGate.GetNextStep(ActiveStep);

        if (nextStep == null)
          return null;

        if (IsScheduled)
        {
          Steps.Where(r => r.ProcessStep.ProcessStepId == nextStep.ProcessStepId).OrderBy(r => r.Rank).FirstOrDefault();
        }
        else
        {
          // Create new Step and pass that back
          dynamic step = Common.Step.CreateDynamicStepInstance(nextStep.Process.Assembly, nextStep.Namespace, new object[] { this });

          // TODO: Add logger
          //step.Logger = _logger;

          step.UseDecisionGate = true;
          step.ProcessStep = nextStep;


          return step;
        }
      }
      else
      {
        if (Steps.Count() > 0)
        {
          return Steps.Where(r => r.Rank > ActiveStep.Rank).OrderBy(r => r.Rank).FirstOrDefault();
        }
      }

      return null;
    }

    #region Event Driven

    public void StartAtStep(IStep step, dynamic data)
    {
      StartNewJob();

      step.ThreadId = Thread.CurrentThread.ManagedThreadId;
        step.Start(data);
    }

    public void CompleteStepAndMove(IStep step, dynamic data, Enumerators.Workflow.WorkflowDirection direction, WFL_ProcessStepDTO jumpToProcessStep = null)
    {
      ActiveStep = step;
      State = Enumerators.Workflow.JobState.Executing;
      Steps.Add(step);

      WFL_ProcessStepJobDTO processStepJob = step.GetCurrentProcessStepJob(data);

      using (var uow = new UnitOfWork())
      {
        ProcessJob = AutoMapper.Mapper.Map<WFL_ProcessJob, WFL_ProcessJobDTO>(
          new XPQuery<WFL_ProcessStepJob>(uow).First(p => p.ProcessStepJobId == processStepJob.ProcessStepJobId).ProcessJob);
      }

      step.ThreadId = Thread.CurrentThread.ManagedThreadId;
      step.ProcessStep = processStepJob.ProcessStep;
      step.ProcessStepJob = processStepJob;

      switch (direction)
      {
        case Enumerators.Workflow.WorkflowDirection.Forward:
          step.UseDecisionGate = true;
          // Complete Step as per normal
          break;
        case Enumerators.Workflow.WorkflowDirection.Backward:
          // Jump to prev step that loan was in
          step.UseDecisionGate = false;

          if (_lastStep == null)
          {
            GetPreviousStep();

            if (_lastStep == null)
              throw new NullReferenceException();

            _lastStep.Rank = ActiveStep.Rank + 1;
          }

          if (!Steps.Contains(_lastStep))
            Steps.Add(_lastStep);

          break;
        case Enumerators.Workflow.WorkflowDirection.Jump:
          // Jump to Specified Step
          step.UseDecisionGate = false;

          if (jumpToProcessStep == null)
            throw new NullReferenceException();

          using (var uow = new UnitOfWork())
          {
            var nextProcessStep = new XPQuery<WFL_ProcessStep>(uow).FirstOrDefault(p => p.ProcessStepId == jumpToProcessStep.ProcessStepId);

            if (nextProcessStep == null)
              throw new NullReferenceException();

            if (!nextProcessStep.Jumpable)
              throw new Exception("Cannot Jump to this Process Step");

            dynamic nextStep = null;

            if (nextProcessStep.Process.ProcessId == ProcessJob.Process.ProcessId)
            {
              // can use same job
              nextStep = Common.Step.CreateDynamicStepInstance(nextProcessStep.Process.Assembly, nextProcessStep.Namespace, new object[] { this });

              nextStep.Rank = step.Rank + 1;
              nextStep.UseDecisionGate = true;
              nextStep.ProcessStep = AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(nextProcessStep);

              var duplicateSteps = Steps.Where(s => s.Rank == nextStep.Rank);
              foreach (var duplicateStep in duplicateSteps)
              {
                Steps.Remove(duplicateStep);
              }

              Steps.Add(nextStep);
            }
            else
            {
              // Complete this job and start a new job.. NEW THREAD!! :D
              var stepsToRemove = Steps.Where(s => s.Rank > step.Rank);
              foreach (var remove in stepsToRemove)
              {
                Steps.Remove(remove);
              }

              Task.Factory.StartNew(() =>
                {
                  var job = new Job(AutoMapper.Mapper.Map<WFL_Process, WFL_ProcessDTO>(nextProcessStep.Process));

                  nextStep = Common.Step.CreateDynamicStepInstance(nextProcessStep.Process.Assembly, nextProcessStep.Namespace, new object[] { job });

                  nextStep.Rank = nextProcessStep.Rank;
                  nextStep.UseDecisionGate = true;
                  nextStep.ProcessStep = AutoMapper.Mapper.Map<WFL_ProcessStep, WFL_ProcessStepDTO>(nextProcessStep);

                  job.StartAtStep(step, null);
                });
            }
          }

          break;
      }

      step.Complete(data);
    }

    #endregion

    #region Schedule Only

    public void StartSchedule()
    {
      _timer = new Timer(StartScheduleProcess, null, GetTimeToStart(), GetInterval());
    }
    
    public DateTime GetNextExecutionDate()
    {
      return (NextExecutionDate ?? StartDate).AddMilliseconds(GetInterval());
    }

    public void StopScheduleJob(bool restart = true)
    {
      State = Enumerators.Workflow.JobState.Stopped;
      _timer.Dispose();

      if (restart)
        StartSchedule();
    }

    #endregion


    #region Private Method

    private void StartNewJob()
    {
      State = Enumerators.Workflow.JobState.Executing;

      using (var uow = new UnitOfWork())
      {
        var processJob = new WFL_ProcessJob(uow);
        processJob.CreateDate = DateTime.Now;
        processJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Executing);
        processJob.LastStateDate = DateTime.Now;
        processJob.Process = new XPQuery<WFL_Process>(uow).First(p => p.ProcessId == Process.ProcessId);

        uow.CommitChanges();

        ProcessJob = AutoMapper.Mapper.Map<WFL_ProcessJob, WFL_ProcessJobDTO>(processJob);
      }
    }

    private void CompleteProcessJob()
    {
      using (var uow = new UnitOfWork())
      {
        var processJob = new XPQuery<WFL_ProcessJob>(uow).First(p => p.ProcessJobId == ProcessJob.ProcessJobId);
        processJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Completed);
        processJob.CompleteDate = DateTime.Now;
        processJob.LastStateDate = DateTime.Now;

        uow.CommitChanges();
      }

      State = Enumerators.Workflow.JobState.Completed;
    }

    private void StartStep(IStep step)
    {
      step.ThreadId = Thread.CurrentThread.ManagedThreadId;
      step.Start();
    }

    #region Schedule Only

    private void StartScheduleProcess(object state)
    {
      StartNewJob();
      StartStep(Steps.OrderBy(s => s.Rank).FirstOrDefault());
    }

    private int GetInterval()
    {
      switch (Frequency)
      {
        case Enumerators.Workflow.ScheduleFrequency.OnceOff:
          return 0;
        case Enumerators.Workflow.ScheduleFrequency.Hourly:
          return (60 * 60 * 1000);
        case Enumerators.Workflow.ScheduleFrequency.Daily:
        case Enumerators.Workflow.ScheduleFrequency.Weekly:
        case Enumerators.Workflow.ScheduleFrequency.Monthly:
          return (24 * 60 * 60 * 1000);
        default: return 0;
      }
    }

    private int GetTimeToStart()
    {
      // TODO TODO TODO TODO TODO TODO
      return 10000;
      //return int.Parse(Math.Round((NextExecutionDate ?? StartDate).Subtract(DateTime.Now).TotalMilliseconds).ToString());
    }


    #endregion

    #endregion

  }
}