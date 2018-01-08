using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_TopUp: XPLiteObject
  {
    private Int64 _topUpId;
    [Key(AutoGenerate = true)]
    public Int64 TopUpId
    {
      get
      {
        return _topUpId;
      }
      set
      {
        SetPropertyValue("TopUpId", ref _topUpId, value);
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

    private ACC_TopUpStatus _topUpStatus;
    [Persistent("TopUpStatusId")]
    public ACC_TopUpStatus TopUpStatus
    {
      get
      {
        return _topUpStatus;
      }
      set
      {
        SetPropertyValue("TopUpStatus", ref _topUpStatus, value);
      }
    }

    private Decimal _topUpAmount;
    [Persistent]
    public Decimal TopUpAmount
    {
      get
      {
        return _topUpAmount;
      }
      set
      {
        SetPropertyValue("TopUpAmount", ref _topUpAmount, value);
      }
    }

    private Decimal _totalFees;
    [Persistent]
    public Decimal TotalFees
    {
      get
      {
        return _totalFees;
      }
      set
      {
        SetPropertyValue("TotalFees", ref _totalFees, value);
      }
    }

    private Decimal _capitalAmount;
    [Persistent]
    public Decimal CapitalAmount
    {
      get
      {
        return _capitalAmount;
      }
      set
      {
        SetPropertyValue("CapitalAmount", ref _capitalAmount, value);
      }
    }

    private DateTime? _payoutDate;
    [Persistent]
    public DateTime? PayoutDate
    {
      get
      {
        return _payoutDate;
      }
      set
      {
        SetPropertyValue("PayoutDate", ref _payoutDate, value);
      }
    }

    private DateTime _createdDate;
    [Persistent]
    public DateTime CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }

    private PER_Person _createdBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _createdBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _createdBy, value);
      }
    }

    #region Constructors

    public ACC_TopUp() : base() { }
    public ACC_TopUp(Session session) : base(session) { }

    #endregion
  }
}
