using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AEDOReportCancelled : XPBaseObject
  {
    public struct ReportCancelledKey
    {
      [Persistent("TransactionId")]
      public Int64 TransactionId;

      [Persistent("ContractRef"), Size(20)]
      public string ContractRef;

      [Persistent("ServiceType"), Size(15)]
      public string ServiceType;

      [Persistent("ValueDT")]
      public DateTime ValueDT;

      [Persistent("CancelDT")]
      public DateTime CancelDT;
    }


    public AEDOReportCancelled() : base() { }
    public AEDOReportCancelled(Session session) : base(session) { }


    [Key, Persistent]
    public ReportCancelledKey ReportCancelled;

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

    private string _trackingIndicator;
    [Persistent, Size(5)]
    public string TrackingIndicator
    {
      get
      {
        return _trackingIndicator;
      }
      set
      {
        SetPropertyValue("TrackingIndicator", ref _trackingIndicator, value);
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

    private string _cancellationType;
    [Persistent, Size(25)]
    public string CancellationType
    {
      get
      {
        return _cancellationType;
      }
      set
      {
        SetPropertyValue("CancellationType", ref _cancellationType, value);
      }
    }

    private string _cancelMerchant;
    [Persistent, Size(20)]
    public string CancelMerchant
    {
      get
      {
        return _cancelMerchant;
      }
      set
      {
        SetPropertyValue("CancelMerchant", ref _cancelMerchant, value);
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

    private int _terminalNum;
    [Persistent]
    public int TerminalNum
    {
      get
      {
        return _terminalNum;
      }
      set
      {
        SetPropertyValue("TerminalNum", ref _terminalNum, value);
      }
    }

    private string _institutionId;
    [Persistent, Size(5)]
    public string InstitutionId
    {
      get
      {
        return _institutionId;
      }
      set
      {
        SetPropertyValue("InstitutionId", ref _institutionId, value);
      }
    }

    private string _idNumber;
    [Persistent, Size(20)]
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