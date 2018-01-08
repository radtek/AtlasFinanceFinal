using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_Transaction : XPLiteObject
  {
    private Int64 _transactionId;
    [Key(AutoGenerate = true)]
    public Int64 TransactionId
    {
      get
      {
        return _transactionId;
      }
      set
      {
        SetPropertyValue("TransactionId", ref _transactionId, value);
      }
    }

    private Int64? _transactionRef;
    [Persistent]
    public Int64? TransactionRef
    {
      get
      {
        return _transactionRef;
      }
      set
      {
        SetPropertyValue("TransactionRef", ref _transactionRef, value);
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

    private AVS_Service _service;
    [Persistent("ServiceId")]
    public AVS_Service Service
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

    private AVS_Status _status;
    [Persistent("StatusId")]
    public AVS_Status Status
    {
      get
      {
        return _status;
      }
      set
      {
        SetPropertyValue("Status", ref _status, value);
      }
    }

    private DateTime _lastStatusDate;
    [Persistent]
    [Indexed]
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

    private CPY_Company _company;
    [Persistent("CompanyId")]
    [Indexed]
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

    private AVS_BankAccountPeriod _bankAccountPeriod;
    [Persistent("BankAccountPeriodId")]
    public AVS_BankAccountPeriod BankAccountPeriod
    {
      get
      {
        return _bankAccountPeriod;
      }
      set
      {
        SetPropertyValue("BankAccountPeriod", ref _bankAccountPeriod, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    [Indexed]
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

    private AVS_Batch _batch;
    [Persistent("BatchId")]
    public AVS_Batch Batch
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

    private string _initials;
    [Persistent, Size(10)]
    public string Initials
    {
      get
      {
        return _initials;
      }
      set
      {
        SetPropertyValue("Initials", ref _initials, value);
      }
    }

    private string _lastName;
    [Persistent, Size(80)]
    public string LastName
    {
      get
      {
        return _lastName;
      }
      set
      {
        SetPropertyValue("LastName", ref _lastName, value);
      }
    }

    private string _idNumber;
    [Persistent, Size(20)]
    [Indexed]
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

    private BNK_Bank _bank;
    [Persistent("BankId")]
    [Indexed]
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

    private string _branchCode;
    [Persistent, Size(10)]
    public string BranchCode
    {
      get
      {
        return _branchCode;
      }
      set
      {
        SetPropertyValue("BranchCode", ref _branchCode, value);
      }
    }

    private string _accountNo;
    [Persistent, Size(20)]
    [Indexed]
    public string AccountNo
    {
      get
      {
        return _accountNo;
      }
      set
      {
        SetPropertyValue("AccountNo", ref _accountNo, value);
      }
    }

    private string _responseAccountNumber;
    [Persistent, Size(2)]
    public string ResponseAccountNumber
    {
      get
      {
        return _responseAccountNumber;
      }
      set
      {
        SetPropertyValue("ResponseAccountNumber", ref _responseAccountNumber, value);
      }
    }

    private string _responseIdNumber;
    [Persistent, Size(2)]
    public string ResponseIdNumber
    {
      get
      {
        return _responseIdNumber;
      }
      set
      {
        SetPropertyValue("ResponseIdNumber", ref _responseIdNumber, value);
      }
    }

    private string _responseInitials;
    [Persistent, Size(2)]
    public string ResponseInitials
    {
      get
      {
        return _responseInitials;
      }
      set
      {
        SetPropertyValue("ResponseInitials", ref _responseInitials, value);
      }
    }

    private string _responseLastName;
    [Persistent, Size(2)]
    public string ResponseLastName
    {
      get
      {
        return _responseLastName;
      }
      set
      {
        SetPropertyValue("ResponseLastName", ref _responseLastName, value);
      }
    }

    private string _responseAccountOpen;
    [Persistent, Size(2)]
    public string ResponseAccountOpen
    {
      get
      {
        return _responseAccountOpen;
      }
      set
      {
        SetPropertyValue("ResponseAccountOpen", ref _responseAccountOpen, value);
      }
    }

    private string _responseAcceptsDebit;
    [Persistent, Size(2)]
    public string ResponseAcceptsDebit
    {
      get
      {
        return _responseAcceptsDebit;
      }
      set
      {
        SetPropertyValue("ResponseAcceptsDebit", ref _responseAcceptsDebit, value);
      }
    }

    private string _responseAcceptsCredit;
    [Persistent, Size(2)]
    public string ResponseAcceptsCredit
    {
      get
      {
        return _responseAcceptsCredit;
      }
      set
      {
        SetPropertyValue("ResponseAcceptsCredit", ref _responseAcceptsCredit, value);
      }
    }

    private string _responseOpenThreeMonths;
    [Persistent, Size(2)]
    public string ResponseOpenThreeMonths
    {
      get
      {
        return _responseOpenThreeMonths;
      }
      set
      {
        SetPropertyValue("ResponseOpenThreeMonths", ref _responseOpenThreeMonths, value);
      }
    }

    private string _thirdPartyRef;
    [Persistent, Size(30)]
    [Indexed]
    public string ThirdPartyRef
    {
      get
      {
        return _thirdPartyRef;
      }
      set
      {
        SetPropertyValue("ThirdPartyRef", ref _thirdPartyRef, value);
      }
    }

    private bool _enabled;
    [Persistent]
    [Indexed]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    private AVS_Result _result;
    [Persistent("ResultId")]
    public AVS_Result Result
    {
      get
      {
        return _result;
      }
      set
      {
        SetPropertyValue("Result", ref _result, value);
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
    [Indexed]
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

    private DateTime? _responseDate;
    [Persistent]
    [Indexed]
    public DateTime? ResponseDate
    {
      get
      {
        return _responseDate;
      }
      set
      {
        SetPropertyValue("ResponseDate", ref _responseDate, value);
      }
    }

    private string _errorMessage;
    [Persistent, Size(100)]
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

    #region Constructors

    public AVS_Transaction() : base() { }
    public AVS_Transaction(Session session) : base(session) { }

    #endregion
  }
}
