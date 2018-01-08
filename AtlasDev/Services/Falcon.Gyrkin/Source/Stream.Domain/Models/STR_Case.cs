using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
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

    private string _reference;
    [Persistent, Size(20)]
    [Indexed]
    public string Reference
    {
      get
      {
        return _reference;
      }
      set
      {
        SetPropertyValue("Reference", ref _reference, value);
      }
    }


    private Host _host;
    [Persistent("HostId")]
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

    private BRN_Branch _branch;
    [Persistent("BranchId")]
    [Indexed]
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

    private STR_Debtor _debtor;
    [Persistent("DebtorId")]
    [Indexed]
    public STR_Debtor Debtor
    {
      get
      {
        return _debtor;
      }
      set
      {
        SetPropertyValue("Debtor", ref _debtor, value);
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

    private decimal _totalLoanAmount;
    [Persistent]
    public decimal TotalLoanAmount
    {
      get
      {
        return _totalLoanAmount;
      }
      set
      {
        SetPropertyValue("TotalLoanAmount", ref _totalLoanAmount, value);
      }
    }

    private decimal _totalBalance;
    [Persistent]
    public decimal TotalBalance
    {
      get
      {
        return _totalBalance;
      }
      set
      {
        SetPropertyValue("TotalBalance", ref _totalBalance, value);
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

    private decimal _totalRequiredPayment;
    [Persistent]
    public decimal TotalRequiredPayment
    {
      get
      {
        return _totalRequiredPayment;
      }
      set
      {
        SetPropertyValue("TotalRequiredPayment", ref _totalRequiredPayment, value);
      }
    }

    private int _totalInstalmentsOutstanding;
    [Persistent]
    public int TotalInstalmentsOutstanding
    {
      get
      {
        return _totalInstalmentsOutstanding;
      }
      set
      {
        SetPropertyValue("TotalInstalmentsOutstanding", ref _totalInstalmentsOutstanding, value);
      }
    }

    private decimal _totalArrearsAmount;
    [Persistent]
    public decimal TotalArrearsAmount
    {
      get
      {
        return _totalArrearsAmount;
      }
      set
      {
        SetPropertyValue("TotalArrearsAmount", ref _totalArrearsAmount, value);
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

    private bool? _workableCase;
    [Persistent]
    public bool? WorkableCase
    {
      get
      {
        return _workableCase;
      }
      set
      {
        SetPropertyValue("WorkableCase", ref _workableCase, value);
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

    private STR_Priority _priority;
    [Persistent("PriorityId")]
    [Indexed]
    public STR_Priority Priority
    {
      get
      {
        return _priority;
      }
      set
      {
        SetPropertyValue("Priority", ref _priority, value);
      }
    }


    #region Constructors

    public STR_Case() : base() { }
    public STR_Case(Session session) : base(session) { }

    #endregion

  }
}