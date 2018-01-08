using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessStepJob : XPLiteObject
  {
    private long _processStepJobId;
    [Key(AutoGenerate = true)]
    public long ProcessStepJobId
    {
      get
      {
        return _processStepJobId;
      }
      set
      {
        SetPropertyValue("ProcessStepJobId", ref _processStepJobId, value);
      }
    }

    private WFL_ProcessJob _processJob;
    [Persistent("ProcessJobId")]
    public WFL_ProcessJob ProcessJob
    {
      get
      {
        return _processJob;
      }
      set
      {
        SetPropertyValue("ProcessJob", ref _processJob, value);
      }
    }

    private WFL_ProcessStep _processStep;
    [Persistent("ProcessStepId")]
    public WFL_ProcessStep ProcessStep
    {
      get
      {
        return _processStep;
      }
      set
      {
        SetPropertyValue("ProcessStep", ref _processStep, value);
      }
    }

    private WFL_JobState _jobState;
    [Persistent("JobStateId")]
    public WFL_JobState JobState
    {
      get
      {
        return _jobState;
      }
      set
      {
        SetPropertyValue("JobState", ref _jobState, value);
      }
    }

    private PER_Person _user;
    [Persistent("UserId")]
    public PER_Person User
    {
      get
      {
        return _user;
      }
      set
      {
        SetPropertyValue("User", ref _user, value);
      }
    }

    private DateTime? _lastStateDate;
    [Persistent]
    public DateTime? LastStateDate
    {
      get
      {

        return _lastStateDate;
      }
      set
      {
        SetPropertyValue("LastStateDate", ref _lastStateDate, value);
      }
    }

    private DateTime? _completeDate;
    [Persistent]
    public DateTime? CompleteDate
    {
      get
      {

        return _completeDate;
      }
      set
      {
        SetPropertyValue("CompleteDate", ref _completeDate, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    public DateTime CreateDate
    {
      get
      {

        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }


    #region Constructors

    public WFL_ProcessStepJob() : base() { }
    public WFL_ProcessStepJob(Session session) : base(session) { }

    #endregion
  }
}
