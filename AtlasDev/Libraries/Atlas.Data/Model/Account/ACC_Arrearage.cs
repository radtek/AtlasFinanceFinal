using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_Arrearage : XPLiteObject
  {
    private Int64 _arrearageId;
    [Key(AutoGenerate = true)]
    public Int64 ArrearageId
    {
      get
      {
        return _arrearageId;
      }
      set
      {
        SetPropertyValue("ArrearageId", ref _arrearageId, value);
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

    private DateTime _periodStart;
    [Persistent]
    public DateTime PeriodStart
    {
      get
      {
        return _periodStart;
      }
      set
      {
        SetPropertyValue("PeriodStart", ref _periodStart, value);
      }
    }

    private DateTime _periodEnd;
    [Persistent]
    public DateTime PeriodEnd
    {
      get
      {
        return _periodEnd;
      }
      set
      {
        SetPropertyValue("PeriodEnd", ref _periodEnd, value);
      }
    }

    private int _arrearageCycle;
    [Persistent]
    public int ArrearageCycle
    {
      get
      {
        return _arrearageCycle;
      }
      set
      {
        SetPropertyValue("ArrearageCycle", ref _arrearageCycle, value);
      }
    }

    private int _delinquencyRank;
    [Persistent]
    public int DelinquencyRank
    {
      get
      {
        return _delinquencyRank;
      }
      set
      {
        SetPropertyValue("DelinquencyRank", ref _delinquencyRank, value);
      }
    }

    private decimal _instalmentDue;
    [Persistent]
    public decimal InstalmentDue
    {
      get
      {
        return _instalmentDue;
      }
      set
      {
        SetPropertyValue("InstalmentDue", ref _instalmentDue, value);
      }
    }

    private decimal _amountPaid;
    [Persistent]
    public decimal AmountPaid
    {
      get
      {
        return _amountPaid;
      }
      set
      {
        SetPropertyValue("AmountPaid", ref _amountPaid, value);
      }
    }

    private decimal _totalPaid;
    [Persistent]
    public decimal TotalPaid
    {
      get
      {
        return _totalPaid;
      }
      set
      {
        SetPropertyValue("TotalPaid", ref _totalPaid, value);
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

    #region Constructors

    public ACC_Arrearage() : base() { }
    public ACC_Arrearage(Session session) : base(session) { }

    #endregion
  }
}
