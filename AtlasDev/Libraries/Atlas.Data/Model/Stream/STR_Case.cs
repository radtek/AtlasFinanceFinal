using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_Case : XPLiteObject
  {
    private Int64 _caseId;
    [Key(AutoGenerate = true)]
    public Int64 CaseId
    {
      get
      {
        return _caseId;
      }
      set
      {
        SetPropertyValue("CaseId", ref _caseId, value);
      }
    }

    private STR_Account _account;
    [Persistent("AccountId")]
    [Indexed]
    public STR_Account Account
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

    private STR_SubCategory _subCategory;
    [Persistent("SubCategoryId")]
    [Indexed]
    public STR_SubCategory SubCategory
    {
      get
      {
        return _subCategory;
      }
      set
      {
        SetPropertyValue("SubCategory", ref _subCategory, value);
      }
    }

    private STR_Group _group;
    [Persistent("GroupId")]
    [Indexed]
    public STR_Group Group
    {
      get
      {
        return _group;
      }
      set
      {
        SetPropertyValue("Group", ref _group, value);
      }
    }

    private STR_CaseStatus _caseStatus;
    [Persistent("CaseStatusId")]
    [Indexed]
    public STR_CaseStatus CaseStatus
    {
      get
      {
        return _caseStatus;
      }
      set
      {
        SetPropertyValue("CaseStatus", ref _caseStatus, value);
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

    private DateTime _openDate;
    [Persistent]
    public DateTime OpenDate
    {
      get
      {
        return _openDate;
      }
      set
      {
        SetPropertyValue("OpenDate", ref _openDate, value);
      }
    }

    private DateTime _closeDate;
    [Persistent]
    public DateTime CloseDate
    {
      get
      {
        return _closeDate;
      }
      set
      {
        SetPropertyValue("CloseEndDate", ref _closeDate, value);
      }
    }

    private decimal _balance;
    [Persistent]
    public decimal Balance
    {
      get
      {
        return _balance;
      }
      set
      {
        SetPropertyValue("Balance", ref _balance, value);
      }
    }

    private DateTime? _lastReceiptDate;
    [Persistent]
    public DateTime? LastReceiptDate
    {
      get
      {
        return _lastReceiptDate;
      }
      set
      {
        SetPropertyValue("LastReceiptDate", ref _lastReceiptDate, value);
      }
    }

    private decimal? _lastReceiptAmount;
    [Persistent]
    public decimal? LastReceiptAmount
    {
      get
      {
        return _lastReceiptAmount;
      }
      set
      {
        SetPropertyValue("LastReceiptAmount", ref _lastReceiptAmount, value);
      }
    }

    private decimal _requiredPayment;
    [Persistent]
    public decimal RequiredPayment
    {
      get
      {
        return _requiredPayment;
      }
      set
      {
        SetPropertyValue("RequiredPayment", ref _requiredPayment, value);
      }
    }

    private int _instalmentsOutstanding;
    [Persistent]
    public int InstalmentsOutstanding
    {
      get
      {
        return _instalmentsOutstanding;
      }
      set
      {
        SetPropertyValue("InstalmentsOutstanding", ref _instalmentsOutstanding, value);
      }
    }

    private decimal _arrearsAmount;
    [Persistent]
    public decimal ArrearsAmount
    {
      get
      {
        return _arrearsAmount;
      }
      set
      {
        SetPropertyValue("ArrearsAmount", ref _arrearsAmount, value);
      }
    }

    private PER_Person _allocatedUser;
    [Persistent("AllocatedUserId")]
    public PER_Person AllocatedUser
    {
      get
      {
        return _allocatedUser;
      }
      set
      {
        SetPropertyValue("AllocatedUser", ref _allocatedUser, value);
      }
    }

    private int _smsCount;
    [Persistent]
    public int SMSCount
    {
      get
      {
        return _smsCount;
      }
      set
      {
        SetPropertyValue("SMSCount", ref _smsCount, value);
      }
    }

    private DateTime? _completeDate;
    [Persistent]
    public DateTime? CompleteDate
    {
      get
      {
        return _completeDate;
      }
      set
      {
        SetPropertyValue("CompleteDate", ref _completeDate, value);
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

    public STR_Case() : base() { }
    public STR_Case(Session session) : base(session) { }

    #endregion

  }
}