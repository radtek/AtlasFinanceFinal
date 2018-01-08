using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_Quotation : XPLiteObject
  {
    private Int64 _quotationId;
    [Key(AutoGenerate = true)]
    public Int64 QuotationId
    {
      get
      {
        return _quotationId;
      }
      set
      {
        SetPropertyValue("QuotationId", ref _quotationId, value);
      }
    }

    private string _quotationNo;
    [Persistent, Size(20)]
    public string QuotationNo
    {
      get
      {
        return _quotationNo;
      }
      set
      {
        SetPropertyValue("QuotationNo", ref _quotationNo, value);
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

    private ACC_AffordabilityOption _affordabilityOption;
    [Persistent("AffordabilityOptionId")]
    public ACC_AffordabilityOption AffordabilityOption
    {
      get
      {
        return _affordabilityOption;
      }
      set
      {
        SetPropertyValue("AffordabilityOption", ref _affordabilityOption, value);
      }
    }

    private ACC_QuotationStatus _quotationStatus;
    [Persistent("QuotationStatusId")]
    public ACC_QuotationStatus QuotationStatus
    {
      get
      {
        return _quotationStatus;
      }
      set
      {
        SetPropertyValue("QuotationStatus", ref _quotationStatus, value);
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

    private DateTime _expiryDate;
    [Persistent]
    public DateTime ExpiryDate
    {
      get
      {
        return _expiryDate;
      }
      set
      {
        SetPropertyValue("ExpiryDate", ref _expiryDate, value);
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

    public ACC_Quotation() : base() { }
    public ACC_Quotation(Session session) : base(session) { }

    #endregion
  }
}
