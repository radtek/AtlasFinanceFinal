using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_TransmissionTransaction:XPLiteObject
  {
    private long _transmissionTransactionId;
    [Key(AutoGenerate = true)]
    public long TransmissionTransactionId
    {
      get
      {
        return _transmissionTransactionId;
      }
      set
      {
        SetPropertyValue("TransmissionTransactionId", ref _transmissionTransactionId, value);
      }
    }

    private PYT_TransmissionSet _transmissionSet;
    [Persistent("TransmissionSetId")]
    [Association]
    public PYT_TransmissionSet TransmissionSet
    {
      get
      {
        return _transmissionSet;
      }
      set
      {
        SetPropertyValue("TransmissionSet", ref _transmissionSet, value);
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

    private PYT_Payout _payoutId;
    [Persistent("PayoutId")]
    public PYT_Payout Payout
    {
      get
      {
        return _payoutId;
      }
      set
      {
        SetPropertyValue("Payout", ref _payoutId, value);
      }
    }

    private int _sequenceNo;
    [Persistent]
    public int SequenceNo
    {
      get
      {
        return _sequenceNo;
      }
      set
      {
        SetPropertyValue("SequenceNo", ref _sequenceNo, value);
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

    #region Constructors

    public PYT_TransmissionTransaction() : base() { }
    public PYT_TransmissionTransaction(Session session) : base(session) { }

    #endregion
  }
}
