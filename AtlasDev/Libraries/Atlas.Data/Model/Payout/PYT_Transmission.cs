using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_Transmission:XPLiteObject
  {
    private long _transmissionId;
    [Key(AutoGenerate = true)]
    public long TransmissionId
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

    private PYT_Batch _batch;
    [Persistent("BatchId")]
    public PYT_Batch Batch
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

    private PYT_ReplyCode _replyCode;
    [Persistent("ReplyCodeId")]
    public PYT_ReplyCode ReplyCode
    {
      get
      {
        return _replyCode;
      }
      set
      {
        SetPropertyValue("ReplyCode", ref _replyCode, value);
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

    private string _filePath;
    [Persistent, Size(300)]
    public string FilePath
    {
      get
      {
        return _filePath;
      }
      set
      {
        SetPropertyValue("FilePath", ref _filePath, value);
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

    [Association]
    public XPCollection<PYT_TransmissionSet> TransmissionSets { get { return GetCollection<PYT_TransmissionSet>("TransmissionSets"); } }

    #region Constructors

    public PYT_Transmission() : base() { }
    public PYT_Transmission(Session session) : base(session) { }

    #endregion
  }
}
