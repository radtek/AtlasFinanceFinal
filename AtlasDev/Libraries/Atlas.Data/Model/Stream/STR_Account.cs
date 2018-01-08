using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
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


    #region Constructors

    public STR_Account() : base() { }
    public STR_Account(Session session) : base(session) { }

    #endregion

  }
}