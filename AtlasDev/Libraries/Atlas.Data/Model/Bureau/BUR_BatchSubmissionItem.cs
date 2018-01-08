namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Atlas.Domain.Interface;
  using DevExpress.Xpo;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class BUR_BatchSubmissionItem : XPLiteObject
  {
    private Int64 _batchSubmissionItemId;
    [Key(AutoGenerate = true)]
    public Int64 BatchSubmissionItemId
    {
      get
      {
        return _batchSubmissionItemId;
      }
      set
      {
        SetPropertyValue("BatchSubmissionItemId", ref _batchSubmissionItemId, value);
      }
    }

    private BUR_BatchSubmission _submissionBatch;
    [Persistent("BatchSubmissionId")]
    public BUR_BatchSubmission SubmissionBatch
    {
      get
      {
        return _submissionBatch;
      }
      set
      {
        SetPropertyValue("SubmissionBatch", ref _submissionBatch, value);
      }
    }

    private BUR_BatchItem _batchItem;
    [Persistent("BatchItemId")]
    public BUR_BatchItem BatchItem
    {
      get
      {
        return _batchItem;
      }
      set
      {
        SetPropertyValue("BatchItem", ref _batchItem, value);
      }
    }

    private string _transmissionStatus;
    [Persistent]
    public string TransmissionStatus
    {
      get
      {
        return _transmissionStatus;
      }
      set
      {
        SetPropertyValue("TransmissionStatus", ref _transmissionStatus, value);
      }
    }

    private string _batchReferenceNo;
    [Persistent]
    [Indexed]
    public string BatchReferenceNo
    {
      get
      {
        return _batchReferenceNo;
      }
      set
      {
        SetPropertyValue("BatchReferenceNo", ref _batchReferenceNo, value);
      }
    }

    private byte[] _submissionXML;
    [Persistent]
    public byte[] SubmissionXML
    {
      get
      {
        return _submissionXML;
      }
      set
      {
        SetPropertyValue("SubmissionXML", ref _submissionXML, value);
      }
    }

    private byte[] _responseXML;
    [Persistent]
    public byte[] ResponseXML
    {
      get
      {
        return _responseXML;
      }
      set
      {
        SetPropertyValue("ResponseXML", ref _responseXML, value);
      }
    }

    private int _errorCode;
    [Persistent, ]
    public int ErrorCode
    {
      get
      {
        return _errorCode;
      }
      set
      {
        SetPropertyValue("ErrorCode", ref _errorCode, value);
      }
    }

    private string _errorMessage;
    [Persistent,Size(int.MaxValue)]
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

      private PER_Person _createUser;
    [Persistent]
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

    private DateTime? _createdDate;
    [Persistent]
    public DateTime? CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }

    #region Constructors

    public BUR_BatchSubmissionItem() : base() { }
    public BUR_BatchSubmissionItem(Session session) : base(session) { }

    #endregion
  }
}
