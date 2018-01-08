using DevExpress.Xpo;
using System;

namespace Atlas.Domain.Model
{
  public class ETL_DebitOrderBatch : XPLiteObject
  {
    private long _debitOrderBatchId;
    [Key(AutoGenerate = true)]
    public long DebitOrderBatchId
    {
      get
      {
        return _debitOrderBatchId;
      }
      set
      {
        SetPropertyValue("DebitOrderBatchId", ref _debitOrderBatchId, value);
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

    private CPY_Company _company;
    [Persistent("CompanyId")]
    public CPY_Company Company
    {
      get
      {
        return _company;
      }
      set
      {
        SetPropertyValue("Company", ref _company, value);
      }
    }

    private ETL_Stage _stage;
    [Persistent("StageId")]
    [Indexed]
    public ETL_Stage Stage
    {
      get
      {
        return _stage;
      }
      set
      {
        SetPropertyValue("Stage", ref _stage, value);
      }
    }

    private DateTime _lastStageDate;
    [Persistent]
    public DateTime LastStageDate
    {
      get
      {
        return _lastStageDate;
      }
      set
      {
        SetPropertyValue("LastStageDate", ref _lastStageDate, value);
      }
    }

    private string _file;
    [Persistent, Size(int.MaxValue)]
    public string File
    {
      get
      {
        return _file;
      }
      set
      {
        SetPropertyValue("File", ref _file, value);
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

    public ETL_DebitOrderBatch() : base() { }
    public ETL_DebitOrderBatch(Session session) : base(session) { }
  }
}