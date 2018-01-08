using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class   ACC_AffordabilityOption: XPLiteObject
  {
    private Int64 _affordabilityOptionId;
    [Key(AutoGenerate = true)]
    public Int64 AffordabilityOptionId
    {
      get
      {
        return _affordabilityOptionId;
      }
      set
      {
        SetPropertyValue("AffordabilityOptionId", ref _affordabilityOptionId, value);
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

    private ACC_TopUp _topUp;
    [Persistent("TopUpId")]
    public ACC_TopUp TopUp
    {
      get
      {
        return _topUp;
      }
      set
      {
        SetPropertyValue("TopUp", ref _topUp, value);
      }
    }

    private ACC_AccountType _accountType;
    [Persistent("AccountTypeId")]
    public ACC_AccountType AccountType
    {
      get
      {
        return _accountType;
      }
      set
      {
        SetPropertyValue("AccountType", ref _accountType, value);
      }
    }

    private Decimal _amount;
    [Persistent]
    public Decimal Amount
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

    private Decimal _totalFees;
    [Persistent]
    public Decimal TotalFees
    {
      get
      {
        return _totalFees;
      }
      set
      {
        SetPropertyValue("TotalFees", ref _totalFees, value);
      }
    }

    private Decimal _capitalAmount;
    [Persistent]
    public Decimal CapitalAmount
    {
      get
      {
        return _capitalAmount;
      }
      set
      {
        SetPropertyValue("CapitalAmount", ref _capitalAmount, value);
      }
    }

    private Decimal? _totalPayBack;
    [Persistent]
    public Decimal? TotalPayBack
    {
      get
      {
        return _totalPayBack;
      }
      set
      {
        SetPropertyValue("TotalPayBack", ref _totalPayBack, value);
      }
    }

    private Decimal? _instalment;
    [Persistent]
    public Decimal? Instalment
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

    private int _numOfInstalment;
    [Persistent]
    public int NumOfInstalment
    {
      get
      {
        return _numOfInstalment;
      }
      set
      {
        SetPropertyValue("NumOfInstalment", ref _numOfInstalment, value);
      }
    }

    private int _period;
    [Persistent]
    public int Period
    {
      get
      {
        return _period;
      }
      set
      {
        SetPropertyValue("Period", ref _period, value);
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

    private ACC_AffordabilityOptionStatus _affordabilityOptionStatus;
    [Persistent("AffordabilityOptionStatusId")]
    public ACC_AffordabilityOptionStatus AffordabilityOptionStatus
    {
      get
      {
        return _affordabilityOptionStatus;
      }
      set
      {
        SetPropertyValue("AffordabilityOptionStatus", ref _affordabilityOptionStatus, value);
      }
    }

    private ACC_AffordabilityOptionType _affordabilityOptionType;
    [Persistent("AffordabilityOptionTypeId")]
    public ACC_AffordabilityOptionType AffordabilityOptionType
    {
      get
      {
        return _affordabilityOptionType;
      }
      set
      {
        SetPropertyValue("AffordabilityOptionType", ref _affordabilityOptionType, value);
      }
    }

    private float? _interestRate;
    [Persistent]
    public float? InterestRate
    {
      get
      {
        return _interestRate;
      }
      set
      {
        SetPropertyValue("InterestRate", ref _interestRate, value);
      }
    }

    private bool? _canClientAfford;
    [Persistent]
    public bool? CanClientAfford
    {
      get
      {
        return _canClientAfford;
      }
      set
      {
        SetPropertyValue("CanClientAfford", ref _canClientAfford, value);
      }
    }

    private DateTime _createdDate;
    [Persistent]
    public DateTime CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }

    private PER_Person _createdBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _createdBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _createdBy, value);
      }
    }

    #region Constructors

    public ACC_AffordabilityOption() : base() { }
    public ACC_AffordabilityOption(Session session) : base(session) { }

    #endregion
  }
}
