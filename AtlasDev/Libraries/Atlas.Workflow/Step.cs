using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Workflow.Interface;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Atlas.Common.Utils;

namespace Atlas.Workflow
{
  public abstract class Step : IStep
  {
    #region Public Properties

    public DateTime StartDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public int ThreadId { get; set; }
    public WFL_ProcessStepDTO ProcessStep { get; set; }
    public WFL_ProcessStepJobDTO ProcessStepJob { get; set; }
    public Enumerators.Workflow.JobState State { get; set; }
    public Enumerators.Workflow.Trigger Trigger { get; set; }
    public int Delay { get; set; }
    public Enumerators.Workflow.PeriodFrequency? DelayPeriodFrequency { get; set; }
    public bool UseDecisionGate { get; set; }
    public int Rank { get; set; }
    public ILogger Logger { get; set; }

    #endregion

    #region Private Properties

    //private dynamic _instance { get; set; }
    private IJob _job { get; set; }
    private int? ProcessStepJobId { get; set; }

    #endregion
    public Step()
    {

    }

    public Step(WFL_ProcessDTO process)
    {
      _job = new Job(process);
    }

    public Step(IJob job)
    {
      //_instance = instance;
      _job = job;
    }

    public virtual bool Validate()
    {
      throw new NotImplementedException();
    }

    public virtual void Start(dynamic data)
    {
    }

    public virtual WFL_ProcessStepJobDTO GetCurrentProcessStepJob(dynamic data)
    {
      throw new NotImplementedException();
    }

    public virtual void Start()
    {
      if (_job.State != Enumerators.Workflow.JobState.Executing)
      {
        //_job.StartSchedule
      }

      StartDate = DateTime.Now;
      _job.ActiveStep = this;

      using (var uow = new UnitOfWork())
      {
        WFL_ProcessStepJob processStepJob;
        if (ProcessStepJobId == null)
        {
          processStepJob = new WFL_ProcessStepJob(uow);
          processStepJob.ProcessJob = new XPQuery<WFL_ProcessJob>(uow).FirstOrDefault(_ => _.ProcessJobId == _job.ProcessJob.ProcessJobId);
          processStepJob.ProcessStep = new XPQuery<WFL_ProcessStep>(uow).FirstOrDefault(_ => _.ProcessStepId == ProcessStep.ProcessStepId);
          processStepJob.CreateDate = DateTime.Now;
        }
        else
        {
          processStepJob = new XPQuery<WFL_ProcessStepJob>(uow).FirstOrDefault(_ => _.ProcessStepJobId == (int)ProcessStepJobId);
          if (processStepJob == null)
            throw new Exception(string.Format("ProcessStepJobId {0} record does not exist in table ProcessStepJob", ProcessStepJobId));
        }

        if (processStepJob.User == null)
        {
          // Get Next eligible user
          var prevStep = _job.GetPreviousStep();

          var eligibleUser = GetNextEligibleUser(prevStep);
          if (eligibleUser != null)
            processStepJob.User = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == eligibleUser.PersonId); ;
        }

        if (processStepJob.User == null)
        {
          // No Users here??  Assign to System, Log Warning
          //this.Logger.Warn(string.Format("There are no user to assign in Process {0}. System has been assigned", ProcessStep.Name));
        }

        processStepJob.JobState = new XPQuery<WFL_JobState>(uow).FirstOrDefault(j => j.Type == Enumerators.Workflow.JobState.Executing);
        processStepJob.LastStateDate = DateTime.Now;

        UpdateState(Enumerators.Workflow.JobState.Executing);

        uow.CommitChanges();

        ProcessStepJob = AutoMapper.Mapper.Map<WFL_ProcessStepJob, WFL_ProcessStepJobDTO>(processStepJob);
      }
    }

    private PER_PersonDTO GetNextEligibleUser(IStep prevStep)
    {
      PER_PersonDTO eligibleUser = null;

      using (var uow = new UnitOfWork())
      {
        var users = (from u in uow.Query<PER_Person>()
                     where u.GetUserGroupLinks.Any(ugl => ugl.Enabled && ugl.UserGroup.Enabled && ugl.UserGroup.ProcessStepUserGroups.Any(psug => psug.Enabled && psug.ProcessStep.ProcessStepId == ProcessStep.ProcessStepId))
                     select new
                     {
                       User = u,
                       Rank = u.GetProcessStepJobs.Count(psj => psj.ProcessStep.ProcessStepId == ProcessStep.ProcessStepId) + 1
                     }).OrderBy(u => u.Rank).Select(p => p.User);

        if (prevStep != null)
        {
          if (prevStep.ProcessStepJob != null)
          {
            if (prevStep.ProcessStepJob.User != null)
            {
              if (users.Where(p => p.PersonId == prevStep.ProcessStepJob.User.PersonId).Count() > 0)
              {
                eligibleUser = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == prevStep.ProcessStepJob.User.PersonId));
              }
            }
          }
        }

        if (eligibleUser == null)
        {
          eligibleUser = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(users.FirstOrDefault());
        }
      }

      return eligibleUser;
    }

    public virtual dynamic GetProcessData(Enumerators.Workflow.WorkflowDataExtType dataExtType)
    {
      dynamic data = null;
      using (var uow = new UnitOfWork())
      {
        var processData = new XPQuery<WFL_ProcessDataExt>(uow).FirstOrDefault(p => p.ProcessJob.ProcessJobId == ProcessStepJob.ProcessJob.ProcessJobId
          && p.DataExtType.Type == dataExtType);

        if (processData != null)
        {
          Assembly assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "\\" + processData.DataExtType.Assembly);
          Type type = assembly.GetType(processData.DataExtType.Namespace);
          data = Xml.DeSerialize(type, processData.Data);
        }
      }
      return data;
    }

    public virtual void SetProcessData(dynamic data, Enumerators.Workflow.WorkflowDataExtType dataExtType)
    {
      using (var uow = new UnitOfWork())
      {
        var processData = new XPQuery<WFL_ProcessDataExt>(uow).FirstOrDefault(p => p.ProcessJob.ProcessJobId == ProcessStepJob.ProcessJob.ProcessJobId
          && p.DataExtType.Type == dataExtType);

        processData.Data = Xml.Serialize(data.GetType(), data, true);

        uow.CommitChanges();
      }
    }

    public virtual void Complete(dynamic data)
    {
      if (_job != null)
      {
        CompleteDate = DateTime.Now;
        ProcessStepJob.JobState.Type = Enumerators.Workflow.JobState.Completed;
        ProcessStepJob.CompleteDate = CompleteDate;

        using (var uow = new UnitOfWork())
        {
          var processStepJob = new XPQuery<WFL_ProcessStepJob>(uow).FirstOrDefault(p => p.ProcessStepJobId == (ProcessStepJobId ?? ProcessStepJob.ProcessStepJobId));
          processStepJob.JobState.Type = ProcessStepJob.JobState.Type;
          processStepJob.CompleteDate = CompleteDate;

          uow.CommitChanges();
        }

        _job.GotoNextStep(this, data);
      }
      else
      {
        Logger.Error(string.Format("Job is NULL for ProcessStepId {0}. Job cannot be NULL", ProcessStep == null ? 0 : ProcessStep.ProcessStepId));
      }
    }

    //public virtual void Complete()
    //{
    //  if (_job != null)
    //  {
    //    CompleteDate = DateTime.Now;
    //    ProcessStepJob.JobState.Type = Enumerators.Workflow.JobState.Completed;
    //    ProcessStepJob.CompleteDate = CompleteDate;

    //    using (var uow = new UnitOfWork())
    //    {
    //      var processStepJob = new XPQuery<WFL_ProcessStepJob>(uow).FirstOrDefault(p => p.ProcessStepJobId == (ProcessStepJobId ?? ProcessStepJob.ProcessStepJobId));
    //      processStepJob.JobState.Type = ProcessStepJob.JobState.Type;
    //      processStepJob.CompleteDate = CompleteDate;

    //      uow.CommitChanges();
    //    }

    //    _job.GotoNextStep(this);
    //  }
    //  else
    //  {
    //    Logger.Error(string.Format("Job is NULL for ProcessStepId {0}. Job cannot be NULL", ProcessStep == null ? 0 : ProcessStep.ProcessStepId));
    //  }
    //}

    public virtual void CreateProcessStepJobAccount(long accountId)
    {
      using (var uow = new UnitOfWork())
      {
        var processStepJobAccount = new WFL_ProcessStepJobAccount(uow);
        processStepJobAccount.ProcessStepJob = new XPQuery<WFL_ProcessStepJob>(uow).First(p => p.ProcessStepJobId == ProcessStepJob.ProcessStepJobId);
        processStepJobAccount.Account = new XPQuery<ACC_Account>(uow).First(a => a.AccountId == accountId);

        uow.CommitChanges();
      }
    }

    public virtual void Stop()
    {
      using (var uow = new UnitOfWork())
      {
        var processStepJob = new XPQuery<WFL_ProcessStepJob>(uow).FirstOrDefault(p => p.ProcessStepJobId == ProcessStepJob.ProcessStepJobId);

        processStepJob.JobState = new XPQuery<WFL_JobState>(uow).FirstOrDefault(j => j.Type == Enumerators.Workflow.JobState.Stopped);
        processStepJob.LastStateDate = DateTime.Now;

        UpdateState(Enumerators.Workflow.JobState.Stopped);

        uow.CommitChanges();
      }

      _job.StopScheduleJob();
    }

    public virtual void Log(string msg, Enumerators.General.Log4NetType logType)
    {
      switch (logType)
      {
        case Enumerators.General.Log4NetType.Error:
          Logger.Error(msg);
          break;
        case Enumerators.General.Log4NetType.Fatal:
          Logger.Fatal(msg);
          break;
        case Enumerators.General.Log4NetType.Trace:
          Logger.Trace(msg);
          break;
        case Enumerators.General.Log4NetType.Warning:
          Logger.Warn(msg);
          break;
        default:
          Logger.Info(msg);
          break;
      }
    }

    private void UpdateState(Enumerators.Workflow.JobState state)
    {
      State = state;
      _job.ActiveStep.State = state;
    }
  }
}