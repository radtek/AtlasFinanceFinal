using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_Payout:XPLiteObject
  {
    private long _payoutId;
    [Key(AutoGenerate=true)]
    public long PayoutId
    {
      get
      {
        return _payoutId;
      }
      set
      {
        SetPropertyValue("PayoutId", ref _payoutId, value);
      }
    }

    private PYT_Service _service;
    [Persistent("ServiceId")]
    public PYT_Service Service
    {
      get
      {
        return _service;
      }
      set
      {
        SetPropertyValue("Service", ref _service, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    [Association]
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

    private CPY_Company _company;
    [Persistent("CompanyId")]
    public CPY_Company Company
    {
      get
      {
        return _company;
      }
      set
      {
        SetPropertyValue("Company", ref _company, value);
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

    private DateTime _actionDate;
    [Persistent]
    public DateTime ActionDate
    {
      get
      {
        return _actionDate;
      }
      set
      {
        SetPropertyValue("ActionDate", ref _actionDate, value);
      }
    }

    private PYT_Batch _batch;
    [Persistent("BatchId")]
    [Association]
    public PYT_Batch Batch
    {
      get
      {
        return _batch;
      }
      set
      {
        SetPropertyValue("Batch", ref _batch, value);
      }
    }

    private PYT_PayoutStatus _payoutStatus;
    [Persistent("PayoutStatusId")]
    public PYT_PayoutStatus PayoutStatus
    {
      get
      {
        return _payoutStatus;
      }
      set
      {
        SetPropertyValue("PayoutStatus", ref _payoutStatus, value);
      }
    }

    private BNK_Detail _bankDetail;
    [Persistent("BankDetailId")]
    public BNK_Detail BankDetail
    {
      get
      {
        return _bankDetail;
      }
      set
      {
        SetPropertyValue("BankDetail", ref _bankDetail, value);
      }
    }

    private DateTime _lastStatusDate;
    [Persistent]
    public DateTime LastStatusDate
    {
      get
      {
        return _lastStatusDate;
      }
      set
      {
        SetPropertyValue("LastStatusDate", ref _lastStatusDate, value);
      }
    }

    private PER_Person _statusUser;
    [Persistent("StatusUserId")]
    public PER_Person StatusUser
    {
      get
      {
        return _statusUser;
      }
      set
      {
        SetPropertyValue("StatusUser", ref _statusUser, value);
      }
    }

    private bool _isValid;
    [Persistent]
    public bool IsValid
    {
      get
      {
        return _isValid;
      }
      set
      {
        SetPropertyValue("IsValid", ref _isValid, value);
      }
    }

    private PYT_ResultCode _resultCode;
    [Persistent("ResultCodeId")]
    public PYT_ResultCode ResultCode
    {
      get
      {
        return _resultCode;
      }
      set
      {
        SetPropertyValue("ResultCode", ref _resultCode, value);
      }
    }

    private DateTime? _resultDate;
    [Persistent]
    public DateTime? ResultDate
    {
      get
      {
        return _resultDate;
      }
      set
      {
        SetPropertyValue("ResultDate", ref _resultDate, value);
      }
    }

    private PYT_ResultQualifier _resultQualifier;
    [Persistent("ResultQualifierId")]
    public PYT_ResultQualifier ResultQualifier
    {
      get
      {
        return _resultQualifier;
      }
      set
      {
        SetPropertyValue("ResultQualifier", ref _resultQualifier, value);
      }
    }

    private string _resultMessage;
    [Persistent]
    public string ResultMessage
    {
      get
      {
        return _resultMessage;
      }
      set
      {
        SetPropertyValue("ResultMessage", ref _resultMessage, value);
      }
    }

    private bool? _paid;
    [Persistent]
    public bool? Paid
    {
      get
      {
        return _paid;
      }
      set
      {
        SetPropertyValue("Paid", ref _paid, value);
      }
    }

    private DateTime? _paidDate;
    [Persistent]
    public DateTime? PaidDate
    {
      get
      {
        return _paidDate;
      }
      set
      {
        SetPropertyValue("PaidDate", ref _paidDate, value);
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

    public PYT_Payout() : base() { }
    public PYT_Payout(Session session) : base(session) { }

    #endregion
  }
}
