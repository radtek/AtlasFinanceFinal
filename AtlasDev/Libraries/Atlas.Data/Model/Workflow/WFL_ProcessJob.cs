using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessJob : XPLiteObject
  {
    private long _processJobId;
    [Key(AutoGenerate = true)]
    public long ProcessJobId
    {
      get
      {
        return _processJobId;
      }
      set
      {
        SetPropertyValue("ProcessJobId", ref _processJobId, value);
      }
    }

    private WFL_Process _process;
    [Persistent("ProcessId")]
    public WFL_Process Process
    {
      get
      {
        return _process;
      }
      set
      {
        SetPropertyValue("Process", ref _process, value);
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

    private DateTime _lastStateDate;
    [Persistent]
    public DateTime LastStateDate
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

    public WFL_ProcessJob() : base() { }
    public WFL_ProcessJob(Session session) : base(session) { }

    #endregion
  }
}
