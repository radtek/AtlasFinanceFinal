using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_Batch : XPLiteObject
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

    private AVS_Service _service;
    [Persistent("ServiceId")]
    public AVS_Service Service
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

    private int _totalRecords;
    [Persistent]
    public int TotalRecords
    {
      get
      {
        return _totalRecords;
      }
      set
      {
        SetPropertyValue("TotalRecords", ref _totalRecords, value);
      }
    }

    private int _transmissionNo;
    [Persistent]
    [Indexed]
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

    private int _firstSequenceNo;
    [Persistent]
    public int FirstSequenceNo
    {
      get
      {
        return _firstSequenceNo;
      }
      set
      {
        SetPropertyValue("FirstSequenceNo", ref _firstSequenceNo, value);
      }
    }

    private int _lastSequenceNo;
    [Persistent]
    public int LastSequenceNo
    {
      get
      {
        return _lastSequenceNo;
      }
      set
      {
        SetPropertyValue("LastSequenceNo", ref _lastSequenceNo, value);
      }
    }

    private bool? _transmissionAccepted;
    [Persistent]
    public bool? TransmissionAccepted
    {
      get
      {
        return _transmissionAccepted;
      }
      set
      {
        SetPropertyValue("TransmissionAccepted", ref _transmissionAccepted, value);
      }
    }

    private bool? _generationAccepted;
    [Persistent]
    public bool? GenerationAccepted
    {
      get
      {
        return _generationAccepted;
      }
      set
      {
        SetPropertyValue("GenerationAccepted", ref _generationAccepted, value);
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

    private DateTime? _submitDate;
    [Persistent]
    [Indexed]
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

    private DateTime? _replyDate;
    [Persistent]
    [Indexed]
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

    private string _replyFilename;
    [Persistent, Size(80)]
    public string ReplyFilename
    {
      get
      {
        return _replyFilename;
      }
      set
      {
        SetPropertyValue("ReplyFilename", ref _replyFilename, value);
      }
    }

    private string _errorMessage;
    [Persistent, Size(100)]
    public string ErrorMessage
    {
      get
      {
        return _errorMessage;
      }
      set
      {
        SetPropertyValue("ErrorMessage", ref _errorMessage, _errorMessage);
      }
    }

    #region Constructors

    public AVS_Batch() : base() { }
    public AVS_Batch(Session session) : base(session) { }

    #endregion
  }
}
