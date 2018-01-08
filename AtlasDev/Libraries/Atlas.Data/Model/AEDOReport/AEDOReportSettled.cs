using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class AEDOReportSettled : XPBaseObject
  {
    public struct ReportSettledKey
    {
      [Persistent("TransactionId")]
      public Int64 TransactionId;

      [Persistent("SettlementDT")]
      public DateTime SettlementDT;

      [Persistent("Instalment")]
      public int Instalment;
    }

    public AEDOReportSettled() : base() { }
    public AEDOReportSettled(Session session) : base(session) { }

    [Key, Persistent]
    public ReportSettledKey ReportSettled;

    private AEDOReportBatch _aedoReportBatch;
    [Persistent]
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

    private DateTime _transmitDT;
    [Persistent]
    public DateTime TransmitDT
    {
      get
      {
        return _transmitDT;
      }
      set
      {
        SetPropertyValue("TransmitDT", ref _transmitDT, value);
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

    private decimal _transactionAmount;
    [Persistent]
    public decimal TransactionAmount
    {
      get
      {
        return _transactionAmount;
      }
      set
      {
        SetPropertyValue("TransactionAmount", ref _transactionAmount, value);
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