using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class WFL_JobState: XPLiteObject
  {
    private int _jobStateId;
    [Key]
    public int JobStateId
    {
      get
      {
        return _jobStateId;
      }
      set
      {
        SetPropertyValue("JobStateId", ref _jobStateId, value);
      }
    }
    
    [NonPersistent]
    public Enumerators.Workflow.JobState Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.JobState>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.JobState>();
      }
    } 

    private string _name;
    [Persistent, Size(10)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
      }
    }

    #region Constructors

    public WFL_JobState() : base() { }
    public WFL_JobState(Session session) : base(session) { }

    #endregion
}
}