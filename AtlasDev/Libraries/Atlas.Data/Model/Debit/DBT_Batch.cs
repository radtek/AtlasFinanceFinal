using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_Batch : XPLiteObject
  {
    private Int64 _batchId;
    [Key(AutoGenerate = true)]
    public Int64 BatchId
    {
      get
      {
        return _batchId;
      }
      set
      {
        SetPropertyValue("BatchId", ref _batchId, value);
      }
    }

    private DBT_Service _service;
    [Persistent("ServiceId")]
    [Indexed]
    public DBT_Service Service
    {
      get
      {
        return _service;
      }
      set
      {
        SetPropertyValue("Service", ref _service, value);
      }
    }

    private DBT_BatchStatus _batchStatus;
    [Persistent("BatchStatusId")]
    [Indexed]
    public DBT_BatchStatus BatchStatus
    {
      get
      {
        return _batchStatus;
      }
      set
      {
        SetPropertyValue("BatchStatus", ref _batchStatus, value);
      }
    }

    private DateTime? _lastStatusDate;
    [Persistent]
    public DateTime? LastStatusDate
    {
      get
      {
        return _lastStatusDate;
      }
      set
      {
        SetPropertyValue("LastStatusDate", ref _lastStatusDate, value);
      }
    }

    private DateTime? _submitDate;
    [Persistent]
    public DateTime? SubmitDate
    {
      get
      {
        return _submitDate;
      }
      set
      {
        SetPropertyValue("SubmitDate", ref _submitDate, value);
      }
    }

    private PER_Person _submitUser;
    [Persistent("SubmitUserId")]
    [Indexed]
    public PER_Person SubmitUser
    {
      get
      {
        return _submitUser;
      }
      set
      {
        SetPropertyValue("SubmitUser", ref _submitUser, value);
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


    public DBT_Batch() : base() { }
    public DBT_Batch(Session session) : base(session) { }
  }
}