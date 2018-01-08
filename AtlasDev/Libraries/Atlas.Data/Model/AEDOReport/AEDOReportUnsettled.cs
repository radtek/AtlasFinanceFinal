using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class AEDOReportUnsettled : XPBaseObject
  {
    public struct ReportUnsettledKey
    {
      [Persistent("TransactionId")]
      public Int64 TransactionId;

      [Persistent("ValueDT")]
      public DateTime ValueDT;
    }

    public AEDOReportUnsettled() : base() { }
    public AEDOReportUnsettled(Session session) : base(session) { }

    [Key, Persistent]
    public ReportUnsettledKey ReportUnsettled;

    private AEDOReportBatch _aedoReportBatch;
    [Persistent]
    [Indexed]
    public AEDOReportBatch ReportBatch
    {
      get
      {
        return _aedoReportBatch;
      }
      set
      {
        SetPropertyValue("ReportBatch", ref _aedoReportBatch, value);
      }
    }

    private int _termId;
    [Persistent]
    public int TermId
    {
      get
      {
        return _termId;
      }
      set
      {
        SetPropertyValue("TermId", ref _termId, value);
      }
    }

    private string _merchantNum;
    [Persistent, Size(20)]
    public string MerchantNum
    {
      get
      {
        return _merchantNum;
      }
      set
      {
        SetPropertyValue("MerchantNum", ref _merchantNum, value);
      }
    }

    private string _pan;
    [Persistent, Size(20)]
    public string Pan
    {
      get
      {
        return _pan;
      }
      set
      {
        SetPropertyValue("Pan", ref _pan, value);
      }
    }

    private int _instalment;
    [Persistent]
    public int Instalment
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

    private decimal _contractAmount;
    [Persistent]
    public decimal ContractAmount
    {
      get
      {
        return _contractAmount;
      }
      set
      {
        SetPropertyValue("ContractAmount", ref _contractAmount, value);
      }
    }

    private DateTime _actualDT;
    [Persistent]
    public DateTime ActualDT
    {
      get
      {
        return _actualDT;
      }
      set
      {
        SetPropertyValue("ActualDT", ref _actualDT, value);
      }
    }

    private string _contractNum;
    [Persistent, Size(20)]
    public string ContractNum
    {
      get
      {
        return _contractNum;
      }
      set
      {
        SetPropertyValue("ContractNum", ref _contractNum, value);
      }
    }

    private DateTime _startDT;
    [Persistent]
    public DateTime StartDT
    {
      get
      {
        return _startDT;
      }
      set
      {
        SetPropertyValue("StartDT", ref _startDT, value);
      }
    }

    private string _currencyCode;
    [Persistent, Size(20)]
    public string CurrencyCode
    {
      get
      {
        return _currencyCode;
      }
      set
      {
        SetPropertyValue("CurrencyCode", ref _currencyCode, value);
      }
    }

    private decimal _amount;
    [Persistent]
    public decimal Amount
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

    private int _frequency;
    [Persistent]
    public int Frequency
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

    private string _employerCode;
    [Persistent, Size(20)]
    public string EmployerCode
    {
      get
      {
        return _employerCode;
      }
      set
      {
        SetPropertyValue("EmployerCode", ref _employerCode, value);
      }
    }

    private string _contractRef;
    [Persistent, Size(20)]
    public string ContractRef
    {
      get
      {
        return _contractRef;
      }
      set
      {
        SetPropertyValue("ContractRef", ref _contractRef, value);
      }
    }

    private string _idNumber;
    [Persistent, Size(15)]
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
  }
}