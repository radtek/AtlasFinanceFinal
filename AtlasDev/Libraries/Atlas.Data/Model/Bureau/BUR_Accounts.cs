// -----------------------------------------------------------------------
// <copyright file="Accounts.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;
  using Atlas.Domain.Interface;

  public sealed class BUR_Accounts : XPLiteObject
  {
    private Int64 _bureauAccountId;
    [Key(AutoGenerate = true)]
    public Int64 BureauAccountId
    {
      get
      {
        return _bureauAccountId;
      }
      set
      {
        SetPropertyValue("BureauAccountId", ref _bureauAccountId, value);
      }
    }

    private ACC_Account _account;
    [Indexed]
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

    private Enumerators.Risk.BureauAccountType _Type;
    public Enumerators.Risk.BureauAccountType Type
    {
      get { return _Type; }
      set
      {
        SetPropertyValue("Type", ref _Type, value);
      }
    }

    private BUR_Enquiry _enquiry;
    [Indexed]
    [Persistent("EnquiryId")]
    public BUR_Enquiry Enquiry
    {
      get
      {
        return _enquiry;
      }
      set
      {
        SetPropertyValue("Enquiry", ref _enquiry, value);
      }
    }

    private Enumerators.Risk.BureauAccountType _AccountSource;
    [Persistent]
    public Enumerators.Risk.BureauAccountType AccountSource
    {
      get
      {
        return _AccountSource;
      }
      set
      {
        SetPropertyValue("AccountSource", ref _AccountSource, value);
      }
    }

    private BUR_AccountTypeCode _AccountType;
    [Indexed]
    [Persistent("BureauTypeCodeId")]
    public BUR_AccountTypeCode AccountType
    {
      get
      {
        return _AccountType;
      }
      set
      {
        SetPropertyValue("AccountType", ref _AccountType, value);
      }
    }

    private string _Subscriber;
    [Size(128)]
    [Persistent]
    public string Subscriber
    {
      get
      {
        return _Subscriber;
      }
      set
      {
        SetPropertyValue("Subscriber", ref _Subscriber, value);
      }
    }

    private string _AccountNo;
    [Persistent]
    [Size(50)]
    public string AccountNo
    {
      get
      {
        return _AccountNo;
      }
      set
      {
        SetPropertyValue("AccountNo", ref _AccountNo, value);
      }
    }

    private string _SubAccountNo;
    [Size(50)]
    [Persistent]
    public string SubAccountNo
    {
      get { return _SubAccountNo; }
      set { SetPropertyValue("SubAccountNo", ref _SubAccountNo, value); }
    }

    private BUR_AccountStatusCode _AccountStatusCode;
    [Persistent("StatusCode")]
    public BUR_AccountStatusCode AccountStatusCode
    {
      get
      {
        return _AccountStatusCode;
      }
      set
      {
        SetPropertyValue("AccountStatusCode", ref _AccountStatusCode, value);
      }
    }

    private string _Status;
    [Size(64)]
    public string Status
    {
      get
      {
        return _Status;
      }
      set
      {
        SetPropertyValue("Status", ref _Status, value);
      }
    }

    private int? _JointParticipants;
    [Persistent]
    public int? JointParticipants
    {
      get
      {
        return _JointParticipants;
      }
      set
      {
        SetPropertyValue("JointParticipants", ref _JointParticipants, value);
      }
    }

    private string _Period;
    [Persistent]
    [Size(11)]
    public string Period
    {
      get
      {
        return _Period;
      }
      set
      {
        SetPropertyValue("Period", ref _Period, value);
      }
    }

    private string _PeriodType;
    [Size(16)]
    public string PeriodType
    {
      get
      {
        return _PeriodType;
      }
      set
      {
        SetPropertyValue("PeriodType", ref _PeriodType, value);
      }
    }

    private bool _Enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _Enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _Enabled, value);
      }
    }

    private DateTime? _OpenDate;
    [Persistent]
    public DateTime? OpenDate
    {
      get
      {
        return _OpenDate;
      }
      set
      {
        SetPropertyValue("OpenDate", ref _OpenDate, value);
      }
    }

    private Decimal? _Instalment;
    [Persistent]
    public Decimal? Instalment
    {
      get
      {
        return _Instalment;
      }
      set
      {
        SetPropertyValue("Instalment", ref _Instalment, value);
      }
    }

    private Decimal? _OpenBalance;
    [Persistent]
    public Decimal? OpenBalance
    {
      get
      {
        return _OpenBalance;
      }
      set
      {
        SetPropertyValue("OpenBalance", ref _OpenBalance, value);
      }
    }

    private Decimal? _CurrentBalance;
    [Persistent]
    public Decimal? CurrentBalance
    {
      get
      {
        return _CurrentBalance;
      }
      set
      {
        SetPropertyValue("CurrentBalance", ref _CurrentBalance, value);
      }
    }

    private Decimal? _OverdueAmount;
    [Persistent]
    public Decimal? OverdueAmount
    {
      get
      {
        return _OverdueAmount;
      }
      set
      {
        SetPropertyValue("OverdueAmount", ref _OverdueAmount, value);
      }
    }

    private DateTime? _BalanceDate;
    public DateTime? BalanceDate
    {
      get
      {

        return _BalanceDate;
      }
      set
      {
        SetPropertyValue("BalanceDate", ref _BalanceDate, value);
      }
    }

    private DateTime? _LastPayDate;
    [Persistent]
    public DateTime? LastPayDate
    {
      get
      {
        return _LastPayDate;
      }
      set
      {
        SetPropertyValue("LastPayDate", ref _LastPayDate, value);
      }
    }

    private DateTime? _StatusDate;
    [Persistent]
    public DateTime? StatusDate
    {
      get
      {
        return _StatusDate;
      }
      set
      {
        SetPropertyValue("StatusDate", ref _StatusDate, value);
      }
    }

    private DateTime? _CreatedDate;
    [Persistent]
    public DateTime? CreatedDate
    {
      get
      {
        return _CreatedDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _CreatedDate, value);
      }
    }

    private PER_Person _createUser;
    [Persistent]
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

    private DateTime? _overrideDate;
    [Persistent]
    public DateTime? OverrideDate
    {
      get
      {
        return _overrideDate;
      }
      set
      {
        SetPropertyValue("OverrideDate", ref _overrideDate, value);
      }
    }

    private PER_Person _overrideUser;
    [Persistent]
    public PER_Person OverrideUser
    {
      get
      {
        return _overrideUser;
      }
      set
      {
        SetPropertyValue("OverrideUser", ref _overrideUser, value);
      }
    }

    #region Constructors

    public BUR_Accounts() : base() { }
    public BUR_Accounts(Session session) : base(session) { }

    #endregion
  }
}
