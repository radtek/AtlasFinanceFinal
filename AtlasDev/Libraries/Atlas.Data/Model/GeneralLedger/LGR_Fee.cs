using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class LGR_Fee : XPLiteObject
  {
    private Int64 _feeId;
    [Key(AutoGenerate = true)]
    public Int64 FeeId
    {
      get
      {
        return _feeId;
      }
      set
      {
        SetPropertyValue("FeeId", ref _feeId, value);
      }
    }

    private LGR_TransactionType _transactionType;
    [Persistent("TransactionTypeId")]
    public LGR_TransactionType TransactionType
    {
      get
      {
        return _transactionType;
      }
      set
      {
        SetPropertyValue("TransactionType", ref _transactionType, value);
      }
    }

    private decimal? _amount;
    [Persistent]
    public decimal? Amount
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

    private float? _percentage;
    [Persistent]
    public float? Percentage
    {
      get
      {
        return _percentage;
      }
      set
      {
        SetPropertyValue("Percentage", ref _percentage, value);
      }
    }

    private float? _vat;
    [Persistent]
    public float? VAT
    {
      get
      {
        return _vat;
      }
      set
      {
        SetPropertyValue("VAT", ref _vat, value);
      }
    }

    private bool? _calculateOnAccountBalance;
    [Persistent]
    public bool? CalculateOnAccountBalance
    {
      get
      {
        return _calculateOnAccountBalance;
      }
      set
      {
        SetPropertyValue("CalculateOnAccountBalance", ref _calculateOnAccountBalance, value);
      }
    }

    private Enumerators.GeneralLedger.FeeRangeType _feeRangeType;
    [Persistent("FeeRangeTypeId")]
    public Enumerators.GeneralLedger.FeeRangeType FeeRangeType
    {
      get
      {
        return _feeRangeType;
      }
      set
      {
        SetPropertyValue("FeeRangeType", ref _feeRangeType, value);
      }
    }

    private decimal? _rangeStart;
    [Persistent]
    public decimal? RangeStart
    {
      get
      {
        return _rangeStart;
      }
      set
      {
        SetPropertyValue("RangeStart", ref _rangeStart, value);
      }
    }

    private decimal? _rangeEnd;
    [Persistent]
    public decimal? RangeEnd
    {
      get
      {
        return _rangeEnd;
      }
      set
      {
        SetPropertyValue("RangeEnd", ref _rangeEnd, value);
      }
    }

    private bool _isInitial;
    [Persistent]
    public bool IsInitial
    {
      get
      {
        return _isInitial;
      }
      set
      {
        SetPropertyValue("IsInitial", ref _isInitial, value);
      }
    }

    private bool _isRecurring;
    [Persistent]
    public bool IsRecurring
    {
      get
      {
        return _isRecurring;
      }
      set
      {
        SetPropertyValue("IsRecurring", ref _isRecurring, value);
      }
    }

    private bool _onSettlement;
    [Persistent]
    public bool OnSettlement
    {
      get
      {
        return _onSettlement;
      }
      set
      {
        SetPropertyValue("OnSettlement", ref _onSettlement, value);
      }
    }

    private bool _isArrearageFee;
    [Persistent]
    public bool IsArrearageFee
    {
      get
      {
        return _isArrearageFee;
      }
      set
      {
        SetPropertyValue("IsArrearageFee", ref _isArrearageFee, value);
      }
    }

    private bool _addWithNewTopUp;
    [Persistent]
    public bool AddWithNewTopUp
    {
      get
      {
        return _addWithNewTopUp;
      }
      set
      {
        SetPropertyValue("AddWithNewTopUp", ref _addWithNewTopUp, value);
      }
    }

    private decimal? _lessAmount;
    [Persistent]
    public decimal? LessAmount
    {
      get
      {
        return _lessAmount;
      }
      set
      {
        SetPropertyValue("LessAmount", ref _lessAmount, value);
      }
    }

    private decimal? _maxAmount;
    [Persistent]
    public decimal? MaxAmount
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

    #region Constructors

    public LGR_Fee() : base() { }
    public LGR_Fee(Session session) : base(session) { }

    #endregion
  }
}