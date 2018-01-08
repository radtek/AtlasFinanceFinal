using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessStepEscalationGroup : XPLiteObject
  {
    private int _processStepEscalationGroupId;
    [Key(AutoGenerate = true)]
    public int ProcessStepEscalationGroupId
    {
      get
      {
        return _processStepEscalationGroupId;
      }
      set
      {
        SetPropertyValue("ProcessStepEscalationGroupId", ref _processStepEscalationGroupId, value);
      }
    }

    private WFL_ProcessStepEscalation _processStepEscalation;
    [Persistent("ProcessStepEscalationId"), Indexed("EscalationGroup", Unique = true)]
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

    private WFL_EscalationGroup _escalationGroup;
    [Persistent("EscalationGroupId")]
    public WFL_EscalationGroup EscalationGroup
    {
      get
      {
        return _escalationGroup;
      }
      set
      {
        SetPropertyValue("EscalationGroup", ref _escalationGroup, value);
      }
    }

    private bool _enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    private DateTime? _disableDate;
    [Persistent]
    public DateTime? DisableDate
    {
      get
      {
        return _disableDate;
      }
      set
      {
        SetPropertyValue("DisableDate", ref _disableDate, value);
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

    public WFL_ProcessStepEscalationGroup() : base() { }
    public WFL_ProcessStepEscalationGroup(Session session) : base(session) { }

    #endregion
  }
}
