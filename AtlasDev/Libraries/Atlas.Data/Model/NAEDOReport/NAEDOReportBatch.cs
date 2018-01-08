using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class NAEDOReportBatch : XPBaseObject
  {
    public NAEDOReportBatch() : base() { }
    public NAEDOReportBatch(Session session) : base(session) { }

    private Int64 _naedoReportBatchId;
    [Key(AutoGenerate = true)]
    public Int64 NAEDOReportBatchId
    {
      get
      {
        return _naedoReportBatchId;
      }
      set
      {
        SetPropertyValue("NAEDOReportBatchId", ref _naedoReportBatchId, value);
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

    private string _serviceType;
    [Persistent, Size(20)]
    public string ServiceType
    {
      get
      {
        return _serviceType;
      }
      set
      {
        SetPropertyValue("ServiceType", ref _serviceType, value);
      }
    }

    private int _reportType;
    [Persistent]
    public int ReportType
    {
      get
      {
        return _reportType;
      }
      set
      {
        SetPropertyValue("ReportType", ref _reportType, value);
      }
    }

    private DateTime _reportFromDT;
    [Persistent]
    public DateTime ReportFromDT
    {
      get
      {
        return _reportFromDT;
      }
      set
      {
        SetPropertyValue("ReportFromDT", ref _reportFromDT, value);
      }
    }

    private DateTime _reportToDT;
    [Persistent]
    public DateTime ReportToDT
    {
      get
      {
        return _reportToDT;
      }
      set
      {
        SetPropertyValue("ReportToDT", ref _reportToDT, value);
      }
    }

    private Int64 _tokenNum;
    [Persistent]
    public Int64 TokenNum
    {
      get
      {
        return _tokenNum;
      }
      set
      {
        SetPropertyValue("TokenNum", ref _tokenNum, value);
      }
    }

    private Int64 _blockNum;
    [Persistent]
    public Int64 BlockNum
    {
      get
      {
        return _blockNum;
      }
      set
      {
        SetPropertyValue("BlockNum", ref _blockNum, value);
      }
    }

    private DateTime? _reportGenerationDT;
    [Persistent]
    public DateTime? ReportGenerationDT
    {
      get
      {
        return _reportGenerationDT;
      }
      set
      {
        SetPropertyValue("ReportGenerationDT", ref _reportGenerationDT, value);
      }
    }

    private DateTime _createDT;
    [Persistent]
    public DateTime CreateDT
    {
      get
      {
        return _createDT;
      }
      set
      {
        SetPropertyValue("CreateDT", ref _createDT, value);
      }
    }

    private Int64 _naedoLoginId;
    [Persistent]
    public Int64 NAEDOLoginId
    {
      get
      {
        return _naedoLoginId;
      }
      set
      {
        SetPropertyValue("NAEDOLoginId", ref _naedoLoginId, value);
      }
    }

    private bool _isSuccess;
    [Persistent]
    public bool IsSuccess
    {
      get
      {
        return _isSuccess;
      }
      set
      {
        SetPropertyValue("IsSuccess", ref _isSuccess, value);
      }
    }

    private string _errorMessage;
    [Persistent, Size(150)]
    public string ErrorMessage
    {
      get
      {
        return _errorMessage;
      }
      set
      {
        SetPropertyValue("ErrorMessage", ref _errorMessage, value);
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

    private DateTime _endDT;
    [Persistent]
    public DateTime EndDT
    {
      get
      {
        return _endDT;
      }
      set
      {
        SetPropertyValue("EndDT", ref _endDT, value);
      }
    }
  }
}