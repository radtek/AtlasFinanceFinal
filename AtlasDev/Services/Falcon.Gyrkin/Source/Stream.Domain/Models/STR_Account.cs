using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  // TODO: add a active/inactive field to denote an open/closed/paid up account
  public class STR_Account : XPLiteObject
  {
    private Int64 _accountId;
    [Key(AutoGenerate = true)]
    public Int64 AccountId
    {
      get
      {
        return _accountId;
      }
      set
      {
        SetPropertyValue("AccountId", ref _accountId, value);
      }
    }

    private STR_Debtor _debtor;
    [Persistent("DebtorId")]
    [Indexed]
    [Association("STR_Account")]
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

    private long _reference2;
    [Persistent]
    [Indexed]
    public long Reference2
    {
      get
      {
        return _reference2;
      }
      set
      {
        SetPropertyValue("Reference2", ref _reference2, value);
      }
    }

    private string _lastImportReference;
    [Persistent, Size(20)]
    [Indexed]
    public string LastImportReference
    {
      get
      {
        return _lastImportReference;
      }
      set
      {
        SetPropertyValue("LastImportReference", ref _lastImportReference, value);
      }
    }

    private DateTime _loanDate;
    [Persistent]
    [Indexed]
    public DateTime LoanDate
    {
      get
      {
        return _loanDate;
      }
      set
      {
        SetPropertyValue("LoanDate", ref _loanDate, value);
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

    private decimal _loanAmount;
    [Persistent]
    [Indexed]
    public decimal LoanAmount
    {
      get
      {
        return _loanAmount;
      }
      set
      {
        SetPropertyValue("LoanAmount", ref _loanAmount, value);
      }
    }

    private int _loanTerm;
    [Persistent]
    public int LoanTerm
    {
      get
      {
        return _loanTerm;
      }
      set
      {
        SetPropertyValue("LoanTerm", ref _loanTerm, value);
      }
    }

    private STR_Frequency _frequency;
    [Persistent("FrequencyId")]
    public STR_Frequency Frequency
    {
      get
      {
        return _frequency;
      }
      set
      {
        SetPropertyValue("Frequency", ref _frequency, value);
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

     private bool _upToDate;
     [Persistent]
     public bool UpToDate
     {
       get
       {
         return _upToDate;
       }
       set
       {
         SetPropertyValue("UpToDate", ref _upToDate, value);
       }
     }

    #region Constructors

    public STR_Account()
    { }
    public STR_Account(Session session) : base(session) { }

    #endregion

  }
}