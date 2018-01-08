using System;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class AEDOReportUnmatched : XPBaseObject
  {
    public AEDOReportUnmatched() : base() { }
    public AEDOReportUnmatched(Session session) : base(session) { }

    private Int64 _unmatchedTransactiondId;
    [Key(AutoGenerate = true)]
    public Int64 UnmatchedTransactiondId
    {
      get
      {
        return _unmatchedTransactiondId;
      }
      set
      {
        SetPropertyValue("UnmatchedTransactiondId", ref _unmatchedTransactiondId, value);
      }
    }

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

    private DateTime _settlementDT;
    [Persistent]
    public DateTime SettlementDT
    {
      get
      {
        return _settlementDT;
      }
      set
      {
        SetPropertyValue("SettlementDT", ref _settlementDT, value);
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
  }
}