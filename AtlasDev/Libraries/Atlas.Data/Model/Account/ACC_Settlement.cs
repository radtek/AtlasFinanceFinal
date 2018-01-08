using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_Settlement : XPLiteObject
  {
    private long _settlementId;
    [Key(AutoGenerate = true)]
    public long SettlementId
    {
      get
      {
        return _settlementId;
      }
      set
      {
        SetPropertyValue("SettlementId", ref _settlementId, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    public ACC_Account Account
    {
      get
      {
        return _account;
      }
      set
      {
        SetPropertyValue("Account", ref _account, value);
      }
    }

    private ACC_SettlementStatus _settlementStatus;
    [Persistent("SettlementStatusId")]
    public ACC_SettlementStatus SettlementStatus
    {
      get
      {
        return _settlementStatus;
      }
      set
      {
        SetPropertyValue("SettlementStatus", ref _settlementStatus, value);
      }
    }

    private ACC_SettlementType _settlementType;
    [Persistent("SettlementTypeId")]
    public ACC_SettlementType SettlementType
    {
      get
      {
        return _settlementType;
      }
      set
      {
        SetPropertyValue("SettlementType", ref _settlementType, value);
      }
    }

    private DateTime _lastStatusChange;
    [Persistent]
    public DateTime LastStatusChange
    {
      get
      {
        return _lastStatusChange;
      }
      set
      {
        SetPropertyValue("LastStatusChange", ref _lastStatusChange, value);
      }
    }

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

    private decimal _fees;
    [Persistent]
    public decimal Fees
    {
      get
      {
        return _fees;
      }
      set
      {
        SetPropertyValue("Fees", ref _fees, value);
      }
    }

    private decimal _interest;
    [Persistent]
    public decimal Interest
    {
      get
      {
        return _interest;
      }
      set
      {
        SetPropertyValue("Interest", ref _interest, value);
      }
    }

    private decimal _totalAmount;
    [Persistent]
    public decimal TotalAmount
    {
      get
      {
        return _totalAmount;
      }
      set
      {
        SetPropertyValue("TotalAmount", ref _totalAmount, value);
      }
    }

    private DateTime _settlementDate;
    [Persistent]
    public DateTime SettlementDate
    {
      get
      {
        return _settlementDate;
      }
      set
      {
        SetPropertyValue("SettlementDate", ref _settlementDate, value);
      }
    }

    private DateTime _expirationDate;
    [Persistent]
    public DateTime ExpirationDate
    {
      get
      {
        return _expirationDate;
      }
      set
      {
        SetPropertyValue("ExpirationDate", ref _expirationDate, value);
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

    public ACC_Settlement() : base() { }
    public ACC_Settlement(Session session) : base(session) { }

    #endregion
  }
}
