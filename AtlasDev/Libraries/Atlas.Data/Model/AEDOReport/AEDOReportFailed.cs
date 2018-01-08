using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class AEDOReportFailed : XPBaseObject
  {
    public struct ReportFailedKey
    {
      [Persistent("TransactionId")]
      public Int64 TransactionId;

      [Persistent("ContractRef"), Size(20)]
      public string ContractRef;

      [Persistent("ServiceType"), Size(15)]
      public string ServiceType;

      [Persistent("ValueDT")]
      public DateTime ValueDT;

      [Persistent("FailDT")]
      public DateTime FailDT;
    }

    public AEDOReportFailed() : base() { }
    public AEDOReportFailed(Session session) : base(session) { }

    [Key, Persistent]
    public ReportFailedKey ReportFailed;

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

    private string _reason;
    [Persistent, Size(10)]
    public string Reason
    {
      get
      {
        return _reason;
      }
      set
      {
        SetPropertyValue("Reason", ref _reason, value);
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

    private int _instalmentNum;
    [Persistent]
    public int InstalmentNum
    {
      get
      {
        return _instalmentNum;
      }
      set
      {
        SetPropertyValue("InstalmentNum", ref _instalmentNum, value);
      }
    }

    private decimal _instalmentAmount;
    [Persistent]
    public decimal InstalmentAmount
    {
      get
      {
        return _instalmentAmount;
      }
      set
      {
        SetPropertyValue("InstalmentAmount", ref _instalmentAmount, value);
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

    private bool _resubmit;
    [Persistent]
    public bool Resubmit
    {
      get
      {
        return _resubmit;
      }
      set
      {
        SetPropertyValue("Resubmit", ref _resubmit, value);
      }
    }

    private string _cardAcceptor;
    [Persistent, Size(20)]
    public string CardAcceptor
    {
      get
      {
        return _cardAcceptor;
      }
      set
      {
        SetPropertyValue("CardAcceptor", ref _cardAcceptor, value);
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