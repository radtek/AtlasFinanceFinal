using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessStepJobEscalation :XPLiteObject
  {
    private int _processStepJobEscalation;
    [Key(AutoGenerate = true)]
    public int ProcessStepJobEscalationId
    {
      get
      {
        return _processStepJobEscalation;
      }
      set
      {
        SetPropertyValue("ProcessStepJobEscalationId", ref _processStepJobEscalation, value);
      }
    }

    private WFL_ProcessStepJob _processStepJob;
    [Persistent("ProcessStepJobId")]
    public WFL_ProcessStepJob ProcessStepJob
    {
      get
      {
        return _processStepJob;
      }
      set
      {
        SetPropertyValue("ProcessStepJob", ref _processStepJob, value);
      }
    }

    private WFL_ProcessStepEscalation _processStepEscalation;
    [Persistent("ProcessStepEscalationId")]
    public WFL_ProcessStepEscalation ProcessStepEscalation
    {
      get
      {
        return _processStepEscalation;
      }
      set
      {
        SetPropertyValue("ProcessStepEscalation", ref _processStepEscalation, value);
      }
    }

    private WFL_EscalationLevel _escalationLevel;
    [Persistent("EscalationLevelId")]
    public WFL_EscalationLevel EscalationLevel
    {
      get
      {
        return _escalationLevel;
      }
      set
      {
        SetPropertyValue("EscalationLevel", ref _escalationLevel, value);
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

    public WFL_ProcessStepJobEscalation() : base() { }
    public WFL_ProcessStepJobEscalation(Session session) : base(session) { }

    #endregion
  }
}
