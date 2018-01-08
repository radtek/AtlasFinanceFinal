using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessStepEscalation : XPLiteObject
  {
    private int _processStepEscalationId;
    [Key(AutoGenerate = true)]
    public int ProcessStepEscalationId
    {
      get
      {
        return _processStepEscalationId;
      }
      set
      {
        SetPropertyValue("ProcessStepEscalationId", ref _processStepEscalationId, value);
      }
    }

    private WFL_ProcessStep _processStep;
    [Persistent("ProcessStepId"), Indexed("EscalationLevel", Unique = true)]
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

    private WFL_EscalationTemplate _escalationTemplate;
    [Persistent("EscalationTemplateId")]
    public WFL_EscalationTemplate EscalationTemplate
    {
      get
      {
        return _escalationTemplate;
      }
      set
      {
        SetPropertyValue("EscalationTemplate", ref _escalationTemplate, value);
      }
    }

    private int _escalationTime;
    [Persistent]
    public int EscalationTime
    {
      get
      {
        return _escalationTime;
      }
      set
      {
        SetPropertyValue("EscalationTime", ref _escalationTime, value);
      }
    }

    private WFL_PeriodFrequency _escalationTimePeriodFrequency;
    [Persistent("EscalationTimePeriodFrequencyId")]
    public WFL_PeriodFrequency EscalationTimePeriodFrequency
    {
      get
      {
        return _escalationTimePeriodFrequency;
      }
      set
      {
        SetPropertyValue("EscalationTimePeriodFrequency", ref _escalationTimePeriodFrequency, value);
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

    public WFL_ProcessStepEscalation() : base() { }
    public WFL_ProcessStepEscalation(Session session) : base(session) { }

    #endregion
  }
}
