using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class TAR_BranchCIMonthly : XPLiteObject
  {
    private long _branchCIMonthlyId;
    [Key(AutoGenerate = true)]
    public long BranchCIMonthlyId
    {
      get
      {
        return _branchCIMonthlyId;
      }
      set
      {
        SetPropertyValue("BranchCIMonthlyId", ref _branchCIMonthlyId, value);
      }
    }

    private BRN_Branch _branch;
    [Persistent("BranchId"),Indexed]
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
    
    private DateTime _targetDate;
    [Persistent,Indexed]
    public DateTime TargetDate
    {
      get
      {
        return _targetDate;
      }
      set
      {
        SetPropertyValue("TargetDate", ref _targetDate, value);
      }
    }

    // only used at the moment
    private decimal _amount;
    [Persistent]
    public decimal Amount
    {
      get
      {
        return _amount;
      }
      set
      {
        SetPropertyValue("Amount", ref _amount, value);
      }
    }

    private float _percent;
    [Persistent]
    public float Percent
    {
      get
      {
        return _percent;
      }
      set
      {
        SetPropertyValue("Percent", ref _percent, value);
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

    #region Constructors

    public TAR_BranchCIMonthly() : base() { }
    public TAR_BranchCIMonthly(Session session) : base(session) { }

    #endregion
  }
}
