using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_Batch : XPLiteObject
  {
    private long _batchId;
    [Key(AutoGenerate = true)]
    public long BatchId
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

    private PYT_Service _service;
    [Persistent("ServiceId")]
    public PYT_Service Service
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

    private PYT_BatchStatus _batchStatus;
    [Persistent("BatchStatusId")]
    public PYT_BatchStatus BatchStatus
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

    private DateTime _lastStatusDate;
    [Persistent]
    public DateTime LastStatusDate
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

    private PER_Person _authoriseUser;
    [Persistent("AuthoriseUserId")]
    public PER_Person AuthoriseUser
    {
      get
      {
        return _authoriseUser;
      }
      set
      {
        SetPropertyValue("AuthoriseUser", ref _authoriseUser, value);
      }
    }

    private DateTime? _authoriseDate;
    [Persistent]
    public DateTime? AuthoriseDate
    {
      get
      {
        return _authoriseDate;
      }
      set
      {
        SetPropertyValue("AuthoriseDate", ref _authoriseDate, value);
      }
    }

    private PER_Person _submitUser;
    [Persistent("SubmitUserId")]
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

    private PER_Person _createUser;
    [Persistent("CreateUserId")]
    public PER_Person CreateUser
    {
      get
      {
        return _createUser;
      }
      set
      {
        SetPropertyValue("CreateUser", ref _createUser, value);
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
    public XPCollection<PYT_Payout> Payouts { get { return GetCollection<PYT_Payout>("Payouts"); } }

    #region Constructors

    public PYT_Batch() : base() { }
    public PYT_Batch(Session session) : base(session) { }

    #endregion
  }
}