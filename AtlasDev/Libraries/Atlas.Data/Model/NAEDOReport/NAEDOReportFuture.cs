using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class NAEDOReportFuture : XPBaseObject
  {
    public struct ReportFutureKey
    {
      [Persistent("TransactionId")]
      public Int64 TransactionId;

      [Persistent("TransactionTypeId")]
      public int TransactionTypeId;

      [Persistent("ProcessMerchant"), Size(25)]
      public string ProcessMerchant;

      [Persistent("ClientRef1"), Size(25)]
      public string ClientRef1;

      [Persistent("ClientRef2"), Size(25)]
      public string ClientRef2;

      [Persistent("ActionDT")]
      public DateTime ActionDT;
    }

    public NAEDOReportFuture() : base() { }
    public NAEDOReportFuture(Session session) : base(session) { }

    [Key, Persistent]
    public ReportFutureKey ReportFuture;

    private NAEDOReportBatch _naedoReportBatch;
    [Persistent]
    [Indexed]
    public NAEDOReportBatch ReportBatch
    {
      get
      {
        return _naedoReportBatch;
      }
      set
      {
        SetPropertyValue("ReportBatch", ref _naedoReportBatch, value);
      }
    }

    private string _numInstallments;
    [Persistent, Size(20)]
    public string NumInstallments
    {
      get
      {
        return _numInstallments;
      }
      set
      {
        SetPropertyValue("NumInstallments", ref _numInstallments, value);
      }
    }

    private string _homingAccountName;
    [Persistent, Size(80)]
    public string HomingAccountName
    {
      get
      {
        return _homingAccountName;
      }
      set
      {
        SetPropertyValue("HomingAccountName", ref _homingAccountName, value);
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

    private string _cCardNum;
    [Persistent, Size(25)]
    public string CCardNum
    {
      get
      {
        return _cCardNum;
      }
      set
      {
        SetPropertyValue("CCardNum", ref _cCardNum, value);
      }
    }

    private string _tracking;
    [Persistent, Size(25)]
    public string Tracking
    {
      get
      {
        return _tracking;
      }
      set
      {
        SetPropertyValue("Tracking", ref _tracking, value);
      }
    }
  }
}