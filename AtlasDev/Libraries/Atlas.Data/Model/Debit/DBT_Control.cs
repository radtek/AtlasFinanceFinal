using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_Control : XPLiteObject
  {
    private Int64 _controlId;
    [Key(AutoGenerate = true)]
    public Int64 ControlId
    {
      get
      {
        return _controlId;
      }
      set
      {
        SetPropertyValue("ControlId", ref _controlId, value);
      }
    }

    private DBT_Service _service;
    [Persistent("ServiceId")]
    [Indexed]
    public DBT_Service Service
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

    private Host _host;
    [Persistent("HostId")]
    [Indexed]
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

    private CPY_Company _companyBranch;
    [Persistent("CompanyBranchId")]
    public CPY_Company CompanyBranch
    {
      get
      {
        return _companyBranch;
      }
      set
      {
        SetPropertyValue("CompanyBranch", ref _companyBranch, value);
      }
    }
    
    private string _thirdPartyReference;
    [Persistent, Size(50)]
    [Indexed]
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
    [Persistent, Size(10)]
    [Indexed]
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
    [Persistent, Size(20)]
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

    private string _bankBranchCode;
    [Persistent, Size(10)]
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

    private string _bankAccountNo;
    [Persistent, Size(25)]
    public string BankAccountNo
    {
      get
      {
        return _bankAccountNo;
      }
      set
      {
        SetPropertyValue("BankAccountNo", ref _bankAccountNo, value);
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

    private string _bankAccountName;
    [Persistent, Size(50)]
    public string BankAccountName
    {
      get
      {
        return _bankAccountName;
      }
      set
      {
        SetPropertyValue("BankAccountName", ref _bankAccountName, value);
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

    private DBT_ControlType _controlType;
    [Persistent("ControlTypeId")]
    [Indexed]
    public DBT_ControlType ControlType
    {
      get
      {
        return _controlType;
      }
      set
      {
        SetPropertyValue("ControlType", ref _controlType, value);
      }
    }

    private DBT_ControlStatus _controlStatus;
    [Persistent("ControlStatusId")]
    [Indexed]
    public DBT_ControlStatus ControlStatus
    {
      get
      {
        return _controlStatus;
      }
      set
      {
        SetPropertyValue("ControlStatus", ref _controlStatus, value);
      }
    }

    private DBT_FailureType _failureType;
    [Persistent("FailureTypeId")]
    [Indexed]
    public DBT_FailureType FailureType
    {
      get
      {
        return _failureType;
      }
      set
      {
        SetPropertyValue("FailureType", ref _failureType, value);
      }
    }

    private DateTime? _lastStatusDate;
    [Persistent]
    public DateTime? LastStatusDate
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

    private int _currentRepetition;
    [Persistent]
    [Indexed]
    public int CurrentRepetition
    {
      get
      {
        return _currentRepetition;
      }
      set
      {
        SetPropertyValue("CurrentRepetition", ref _currentRepetition, value);
      }
    }

    private int _repetitions;
    [Persistent]
    [Indexed]
    public int Repetitions
    {
      get
      {
        return _repetitions;
      }
      set
      {
        SetPropertyValue("Repetitions", ref _repetitions, value);
      }
    }

    private bool _isValid;
    [Persistent]
    [Indexed]
    public bool IsValid
    {
      get { return _isValid; }
      set { SetPropertyValue("IsValid", ref _isValid, value); }
    }

    private DateTime? _createDate;
    [Persistent]
    public DateTime? CreateDate
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

    private ACC_PayDate _payDate;
    [Persistent("PayDateId")]
    public ACC_PayDate PayDate
    {
      get
      {
        return _payDate;
      }
      set
      {
        SetPropertyValue("PayDate", ref _payDate, value);
      }
    }

    private DBT_AVSCheckType _avsCheckType;
    [Persistent("AVSCheckTypeId")]
    public DBT_AVSCheckType AVSCheckType
    {
      get
      {
        return _avsCheckType;
      }
      set
      {
        SetPropertyValue("AVSCheckType", ref _avsCheckType, value);
      }
    }

    private long _avsTransactionId;
    [Persistent]
    [Indexed]
    public long AVSTransactionId
    {
      get
      {
        return _avsTransactionId;
      }
      set
      {
        SetPropertyValue("AVSTransactionId", ref _avsTransactionId, value);
      }
    }

    private decimal _instalment;
    [Persistent]
    public decimal Instalment
    {
      get
      {
        return _instalment;
      }
      set
      {
        SetPropertyValue("Instalment", ref _instalment, value);
      }
    }

    private DateTime _lastInstalmentUpdate;
    [Persistent]
    public DateTime LastInstalmentUpdate
    {
      get
      {
        return _lastInstalmentUpdate;
      }
      set
      {
        SetPropertyValue("LastInstalmentUpdate", ref _lastInstalmentUpdate, value);
      }
    }

    private DateTime _firstInstalmentDate;
    [Persistent]
    public DateTime FirstInstalmentDate
    {
      get
      {
        return _firstInstalmentDate;
      }
      set
      {
        SetPropertyValue("FirstInstalmentDate", ref _firstInstalmentDate, value);
      }
    }

    [Association]
    public XPCollection<DBT_Transaction> Transactions { get { return GetCollection<DBT_Transaction>("Transactions"); } }

    public DBT_Control() : base() { }
    public DBT_Control(Session session) : base(session) { }
  }
}