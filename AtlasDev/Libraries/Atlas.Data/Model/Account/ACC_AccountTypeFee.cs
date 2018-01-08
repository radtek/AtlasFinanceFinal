using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AccountTypeFee: XPCustomObject
  {
    private Int64 _accountTypeFeeId;
    [Key(AutoGenerate = true)]
    public Int64 AccountTypeFeeId
    {
      get
      {
        return _accountTypeFeeId;
      }
      set
      {
        SetPropertyValue("AccountTypeFeeId", ref _accountTypeFeeId, value);
      }
    }

    private ACC_AccountType _accountType;
    [Persistent("AccountTypeId")]
    public ACC_AccountType AccountType
    {
      get
      {
        return _accountType;
      }
      set
      {
        SetPropertyValue("AccountType", ref _accountType, value);
      }
    }

    private LGR_Fee _fee;
    [Persistent("FeeId")]
    public LGR_Fee Fee
    {
      get
      {
        return _fee;
      }
      set
      {
        SetPropertyValue("Fee", ref _fee, value);
      }
    }

    private DateTime? _effectiveDate;
    [Persistent]
    public DateTime? EffectiveDate
    {
      get
      {
        return _effectiveDate;
      }
      set
      {
        SetPropertyValue("EffectiveDate", ref _effectiveDate, value);
      }
    }

    private bool _enabled;
    [Persistent("Enabled")]
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

    private DateTime? _disabledDate;
    [Persistent]
    public DateTime? DisabledDate
    {
      get
      {
        return _disabledDate;
      }
      set
      {
        SetPropertyValue("DisabledDate", ref _disabledDate, value);
      }
    }

    private int _ordinal;
    [Persistent]
    public int Ordinal
    {
      get
      {
        return _ordinal;
      }
      set
      {
        SetPropertyValue("Ordinal", ref _ordinal, value);
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

    public ACC_AccountTypeFee() : base() { }
    public ACC_AccountTypeFee(Session session) : base(session) { }

    #endregion
  }
}
