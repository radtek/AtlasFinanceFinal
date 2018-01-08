using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_EscalationGroupPerson : XPLiteObject
  {
    private int _escalationGroupPersonId;
    [Key(AutoGenerate = true)]
    public int EscalationGroupPersonId
    {
      get
      {
        return _escalationGroupPersonId;
      }
      set
      {
        SetPropertyValue("EscalationGroupPersonId", ref _escalationGroupPersonId, value);
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

    private PER_Person _person;
    [Persistent("PersonId")]
    public PER_Person Person
    {
      get
      {
        return _person;
      }
      set
      {
        SetPropertyValue("Person", ref _person, value);
      }
    }

    private NTF_Type _notificationType;
    [Persistent("NotificationTypeId")]
    public NTF_Type NotificationType
    {
      get
      {
        return _notificationType;
      }
      set
      {
        SetPropertyValue("NotificationType", ref _notificationType, value);
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

    public WFL_EscalationGroupPerson() : base() { }
    public WFL_EscalationGroupPerson(Session session) : base(session) { }

    #endregion
  }
}
