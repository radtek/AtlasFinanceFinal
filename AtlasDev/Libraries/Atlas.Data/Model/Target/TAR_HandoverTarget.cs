using System;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class TAR_HandoverTarget : XPLiteObject
  {
    private int _targetId;
    [Key(AutoGenerate = true)]
    public int TargetId
    {
      get
      {
        return _targetId;
      }
      set
      {
        SetPropertyValue("TargetId", ref _targetId, value);
      }
    }

    private BRN_Branch _branch;
    [Persistent("BranchId")]
    public BRN_Branch Branch
    {
      get
      {
        return _branch;
      }
      set
      {
        SetPropertyValue("Branch", ref _branch, value);
      }
    }

    private Host _host;
    [Persistent("HostId")]
    [Indexed]
    public Host Host
    {
      get
      {
        return _host;
      }
      set
      {
        SetPropertyValue("Host", ref _host, value);
      }
    }

    private DateTime _startRange;
    [Persistent]
    public DateTime StartRange
    {
      get
      {
        return _startRange;
      }
      set
      {
        SetPropertyValue("StartRange", ref _startRange, value);
      }
    }

    private DateTime _endRange;
    [Persistent]
    public DateTime EndRange
    {
      get
      {
        return _endRange;
      }
      set
      {
        SetPropertyValue("EndRange", ref _endRange, value);
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

    private DateTime _activeDate;
    [Persistent]
    public DateTime ActiveDate
    {
      get
      {
        return _activeDate;
      }
      set
      {
        SetPropertyValue("ActiveDate", ref _activeDate, value);
      }
    }

    private decimal _handoverBudget;
    [Persistent]
    public decimal HandoverBudget
    {
      get
      {
        return _handoverBudget;
      }
      set
      {
        SetPropertyValue("HandoverBudget", ref _handoverBudget, value);
      }
    }

    private float _arrearTarget;
    [Persistent]
    public float ArrearTarget
    {
      get
      {
        return _arrearTarget;
      }
      set
      {
        SetPropertyValue("ArrearTarget", ref _arrearTarget, value);
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

    private PER_Person _createUser;
    [Persistent("CreateUserId")]
    public PER_Person CreateUser
    {
      get
      {
        return _createUser;
      }
      set
      {
        SetPropertyValue("CreateUser", ref _createUser, value);
      }
    }
    public TAR_HandoverTarget() : base() { }
    public TAR_HandoverTarget(Session session) : base(session) { }
  }
}