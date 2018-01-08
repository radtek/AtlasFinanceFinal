using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AccountType : XPLiteObject
  {
    private Int64 _accountTypeId;
    [Key(AutoGenerate = true)]
    public Int64 AccountTypeId
    {
      get
      {
        return _accountTypeId;
      }
      set
      {
        SetPropertyValue("AccountTypeId", ref _accountTypeId, value);
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

    private string _description;
    [Persistent, Size(20)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    private decimal _closeBalance;
    [Persistent]
    public decimal CloseBalance
    {
      get
      {
        return _closeBalance;
      }
      set
      {
        SetPropertyValue("CloseBalance", ref _closeBalance, value);
      }
    }

    private int _minPeriod;
    [Persistent]
    public int MinPeriod
    {
      get
      {
        return _minPeriod;
      }
      set
      {
        SetPropertyValue("MinPeriod", ref _minPeriod, value);
      }
    }

    private int? _maxPeriod;
    [Persistent]
    public int? MaxPeriod
    {
      get
      {
        return _maxPeriod;
      }
      set
      {
        SetPropertyValue("MaxPeriod", ref _maxPeriod, value);
      }
    }

    private decimal _minAmount;
    [Persistent]
    public decimal MinAmount
    {
      get
      {
        return _minAmount;
      }
      set
      {
        SetPropertyValue("MinAmount", ref _minAmount, value);
      }
    }

    private decimal _maxAmount;
    [Persistent]
    public decimal MaxAmount
    {
      get
      {
        return _maxAmount;
      }
      set
      {
        SetPropertyValue("MaxAmount", ref _maxAmount, value);
      }
    }

    private float _interestRate;
    [Persistent]
    public float InterestRate
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

    private float? _repoRate;
    [Persistent]
    public float? RepoRate
    {
      get
      {
        return _repoRate;
      }
      set
      {
        SetPropertyValue("RepoRate", ref _repoRate, value);
      }
    }

    private float? _repoFactor;
    [Persistent]
    public float? RepoFactor
    {
      get
      {
        return _repoFactor;
      }
      set
      {
        SetPropertyValue("RepoFactor", ref _repoFactor, value);
      }
    }

    private int? _bufferDaysFirstInstalmentDate;
    [Persistent]
    public int? BufferDaysFirstInstalmentDate
    {
      get
      {
        return _bufferDaysFirstInstalmentDate;
      }
      set
      {
        SetPropertyValue("BufferDaysFirstInstalmentDate", ref _bufferDaysFirstInstalmentDate, value);
      }
    }

    private int? _interestFreePeriods;
    [Persistent]
    public int? InterestFreePeriods
    {
      get
      {
        return _interestFreePeriods;
      }
      set
      {
        SetPropertyValue("InterestFreePeriods", ref _interestFreePeriods, value);
      }
    }

    private int _quotationExpiryPeriod;
    [Persistent]
    public int QuotationExpiryPeriod
    {
      get
      {
        return _quotationExpiryPeriod;
      }
      set
      {
        SetPropertyValue("QuotationExpiryPeriod", ref _quotationExpiryPeriod, value);
      }
    }

    private bool? _allowAffordabilityOptions;
    [Persistent]
    public bool? AllowAffordabilityOptions
    {
      get
      {
        return _allowAffordabilityOptions;
      }
      set
      {
        SetPropertyValue("AllowAffordabilityOptions", ref _allowAffordabilityOptions, value);
      }
    }

    private float? _affordabilityPercentBuffer;
    [Persistent]
    public float? AffordabilityPercentBuffer
    {
      get
      {
        return _affordabilityPercentBuffer;
      }
      set
      {
        SetPropertyValue("AffordabilityPercentBuffer", ref _affordabilityPercentBuffer, value);
      }
    }

    private int _defaultTrackingDays;
    [Persistent]
    public int DefaultTrackingDays
    {
      get
      {
        return _defaultTrackingDays;
      }
      set
      {
        SetPropertyValue("DefaultTrackingDays", ref _defaultTrackingDays, value);
      }
    }

    private int _arrearageBufferPeriod;
    [Persistent]
    public int ArrearageBufferPeriod
    {
      get
      {
        return _arrearageBufferPeriod;
      }
      set
      {
        SetPropertyValue("ArrearageBufferPeriod", ref _arrearageBufferPeriod, value);
      }
    }

    private int _settlementExpirationBuffer;
    [Persistent]
    public int SettlementExpirationBuffer
    {
      get
      {
        return _settlementExpirationBuffer;
      }
      set
      {
        SetPropertyValue("SettlementExpirationBuffer", ref _settlementExpirationBuffer, value);
      }
    }

    private int _ordinal;
    [Persistent]
    public int Ordinal
    {
      get
      {
        return _ordinal;
      }
      set
      {
        SetPropertyValue("Ordinal", ref _ordinal, value);
      }
    }

    private bool _enabled;
    [Persistent]
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

    private DateTime? _disabledDate;
    [Persistent]
    public DateTime? DisabledDate
    {
      get
      {
        return _disabledDate;
      }
      set
      {
        SetPropertyValue("DisabledDate", ref _disabledDate, value);
      }
    }

    #region Constructors

    public ACC_AccountType() : base() { }
    public ACC_AccountType(Session session) : base(session) { }

    #endregion
  }
}