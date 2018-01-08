using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_Transmission : XPLiteObject
  {
    private Int64 _transmissionId;
    [Key(AutoGenerate = true)]
    public Int64 TransmissionId
    {
      get
      {
        return _transmissionId;
      }
      set
      {
        SetPropertyValue("TransmissionId", ref _transmissionId, value);
      }
    }

    private DBT_Batch _batch;
    [Persistent("BatchId")]
    [Indexed]
    public DBT_Batch Batch
    {
      get
      {
        return _batch;
      }
      set
      {
        SetPropertyValue("Batch", ref _batch, value);
      }
    }

    private int _transmissionNo;
    [Persistent]
    public int TransmissionNo
    {
      get
      {
        return _transmissionNo;
      }
      set
      {
        SetPropertyValue("TransmissionNo", ref _transmissionNo, value);
      }
    }

    private bool? _accepted;
    [Persistent]
    public bool? Accepted
    {
      get
      {
        return _accepted;
      }
      set
      {
        SetPropertyValue("Accepted", ref _accepted, value);
      }
    }

    private DateTime? _replyDate;
    [Persistent]
    public DateTime? ReplyDate
    {
      get
      {
        return _replyDate;
      }
      set
      {
        SetPropertyValue("ReplyDate", ref _replyDate, value);
      }
    }

    private DateTime? _createDate;
    [Persistent]
    public DateTime? CreateDate
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


    public DBT_Transmission() : base() { }
    public DBT_Transmission(Session session) : base(session) { }
  }
}