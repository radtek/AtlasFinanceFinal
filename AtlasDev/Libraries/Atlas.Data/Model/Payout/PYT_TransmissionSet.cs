using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_TransmissionSet:XPLiteObject
  {
    private long _transmissionSetId;
    [Key(AutoGenerate = true)]
    public long TransmissionSetId
    {
      get
      {
        return _transmissionSetId;
      }
      set
      {
        SetPropertyValue("TransmissionSetId", ref _transmissionSetId, value);
      }
    }

    private PYT_Transmission _transmission;
    [Persistent("TransmissionId")]
    [Association]
    public PYT_Transmission Transmission
    {
      get
      {
        return _transmission;
      }
      set
      {
        SetPropertyValue("Transmission", ref _transmission, value);
      }
    }

    private int _generationNo;
    [Persistent]
    public int GenerationNo
    {
      get
      {
        return _generationNo;
      }
      set
      {
        SetPropertyValue("GenerationNo", ref _generationNo, value);
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

    [Association]
    public XPCollection<PYT_TransmissionTransaction> TransmissionTransactions { get { return GetCollection<PYT_TransmissionTransaction>("TransmissionTransactions"); } }

    #region Constructors

    public PYT_TransmissionSet() : base() { }
    public PYT_TransmissionSet(Session session) : base(session) { }

    #endregion
  }
}
