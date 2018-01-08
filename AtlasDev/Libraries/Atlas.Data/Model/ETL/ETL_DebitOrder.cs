using DevExpress.Xpo;
using System;

namespace Atlas.Domain.Model
{
  public class ETL_DebitOrder : XPLiteObject
  {
    private long _debitOrderId;
    [Key(AutoGenerate = true)]
    public long DebitOrderId
    {
      get
      {
        return _debitOrderId;
      }
      set
      {
        SetPropertyValue("DebitOrderId", ref _debitOrderId, value);
      }
    }

    private ETL_Stage _stage;
    [Persistent("StageId")]
    [Indexed]
    public ETL_Stage Stage
    {
      get
      {
        return _stage;
      }
      set
      {
        SetPropertyValue("Stage", ref _stage, value);
      }
    }

    private DateTime _lastStageDate;
    [Persistent]
    public DateTime LastStageDate
    {
      get
      {
        return _lastStageDate;
      }
      set
      {
        SetPropertyValue("LastStageDate", ref _lastStageDate, value);
      }
    }

    private string _errorMessage;
    [Persistent, Size(200)]
    public string ErrorMessage
    {
      get
      {
        return _errorMessage;
      }
      set
      {
        SetPropertyValue("ErrorMessage", ref _errorMessage, value);
      }
    }

    private ETL_DebitOrderBatch _debitOrderBatch;
    [Persistent("DebitOrderBatchId")]
    [Indexed]
    public ETL_DebitOrderBatch DebitOrderBatch
    {
      get
      {
        return _debitOrderBatch;
      }
      set
      {
        SetPropertyValue("DebitOrderBatch", ref _debitOrderBatch, value);
      }
    }

    private DBT_Control _debitOrderControl;
    [Persistent("DebitOrderControlId")]
    [Indexed]
    public DBT_Control DebitOrderControl
    {
      get
      {
        return _debitOrderControl;
      }
      set
      {
        SetPropertyValue("DebitOrderControl", ref _debitOrderControl, value);
      }
    }

    private string _thirdPartyReference;
    [Persistent, Size(20)]
    public string ThirdPartyReference
    {
      get
      {
        return _thirdPartyReference;
      }
      set
      {
        SetPropertyValue("ThirdPartyReference", ref _thirdPartyReference, value);
      }
    }

    private string _bankStatementReference;
    [Persistent, Size(30)]
    public string BankStatementReference
    {
      get
      {
        return _bankStatementReference;
      }
      set
      {
        SetPropertyValue("BankStatementReference", ref _bankStatementReference, value);
      }
    }

    private string _idNumber;
    [Persistent, Size(13)]
    public string IdNumber
    {
      get
      {
        return _idNumber;
      }
      set
      {
        SetPropertyValue("IdNumber", ref _idNumber, value);
      }
    }

    private string _accountNumber;
    [Persistent, Size(13)]
    public string AccountNumber
    {
      get
      {
        return _accountNumber;
      }
      set
      {
        SetPropertyValue("AccountNumber", ref _accountNumber, value);
      }
    }

    private string _accountName;
    [Persistent, Size(80)]
    public string AccountName
    {
      get
      {
        return _accountName;
      }
      set
      {
        SetPropertyValue("AccountName", ref _accountName, value);
      }
    }

    private BNK_Bank _bank;
    [Persistent("BankId")]
    public BNK_Bank Bank
    {
      get
      {
        return _bank;
      }
      set
      {
        SetPropertyValue("Bank", ref _bank, value);
      }
    }

    private string _bankBranchCode;
    [Persistent, Size(6)]
    public string BankBranchCode
    {
      get
      {
        return _bankBranchCode;
      }
      set
      {
        SetPropertyValue("BankBranchCode", ref _bankBranchCode, value);
      }
    }

    private DateTime _firstActionDate;
    [Persistent]
    public DateTime FirstActionDate
    {
      get
      {
        return _firstActionDate;
      }
      set
      {
        SetPropertyValue("FirstActionDate", ref _firstActionDate, value);
      }
    }

    private int _repititions;
    [Persistent]
    public int Repititions
    {
      get
      {
        return _repititions;
      }
      set
      {
        SetPropertyValue("Repititions", ref _repititions, value);
      }
    }

    private BNK_AccountType _bankAccountType;
    [Persistent("BankAccountTypeId")]
    public BNK_AccountType BankAccountType
    {
      get
      {
        return _bankAccountType;
      }
      set
      {
        SetPropertyValue("BankAccountType", ref _bankAccountType, value);
      }
    }

    private decimal _instalmentAmount;
    [Persistent]
    public decimal InstalmentAmount
    {
      get
      {
        return _instalmentAmount;
      }
      set
      {
        SetPropertyValue("InstalmentAmount", ref _instalmentAmount, value);
      }
    }

    private ACC_PeriodFrequency _periodFrequency;
    [Persistent("PeriodFrequencyId")]
    public ACC_PeriodFrequency PeriodFrequency
    {
      get
      {
        return _periodFrequency;
      }
      set
      {
        SetPropertyValue("PeriodFrequency", ref _periodFrequency, value);
      }
    }

    private int _trackingDays;
    [Persistent]
    public int TrackingDays
    {
      get
      {
        return _trackingDays;
      }
      set
      {
        SetPropertyValue("TrackingDays", ref _trackingDays, value);
      }
    }

    private ACC_PayRule _payRule;
    [Persistent("PayRuleId")]
    public ACC_PayRule PayRule
    {
      get
      {
        return _payRule;
      }
      set
      {
        SetPropertyValue("PayRule", ref _payRule, value);
      }
    }

    private ACC_PayDateType _payDateType;
    [Persistent("PayDateTypeId")]
    public ACC_PayDateType PayDateType
    {
      get
      {
        return _payDateType;
      }
      set
      {
        SetPropertyValue("PayDateType", ref _payDateType, value);
      }
    }

    private int _payDateNo;
    [Persistent]
    public int PayDateNo
    {
      get
      {
        return _payDateNo;
      }
      set
      {
        SetPropertyValue("PayDateNo", ref _payDateNo, value);
      }
    }

    public ETL_DebitOrder() : base() { }
    public ETL_DebitOrder(Session session) : base(session) { }
  }
}