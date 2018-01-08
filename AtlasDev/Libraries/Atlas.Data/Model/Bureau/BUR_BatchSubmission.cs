namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using Atlas.Domain.Interface;
  using DevExpress.Xpo;

  public class BUR_BatchSubmission : XPLiteObject
  {
    private Int64 _batchSubmissionId;
    [Key(AutoGenerate = true)]
    public Int64 BatchSubmissionId
    {
      get
      {
        return _batchSubmissionId;
      }
      set
      {
        SetPropertyValue("batchSubmissionId", ref _batchSubmissionId, value);
      }
    }

    private BUR_Batch _batch;
    [Persistent("BatchId")]
    [Indexed]
    public BUR_Batch Batch
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

    private Int64 _jobId;
     [Indexed]
    [Persistent]
    public Int64 JobId
    {
      get
      {
        return _jobId;
      }
      set
      {
        SetPropertyValue("JobId", ref _jobId, value);
      }
    }

    private byte[] _XML;
    [Persistent]
    public byte[] XML
    {
      get
      {
        return _XML;
      }
      set
      {
        SetPropertyValue("XML", ref _XML, value);
      }
    }

    private Enumerators.Risk.BatchJobStatus _status;
    [Persistent]
    [Indexed]
    public Enumerators.Risk.BatchJobStatus Status
    {
      get
      {
        return _status;
      }
      set
      {
        SetPropertyValue("Status", ref _status, value);
      }
    }

    private string _errorMessage;
    [Persistent, Size(int.MaxValue)]
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

    private bool _invalid;
    [Persistent]
    public bool Invalid
    {
      get
      {
        return _invalid;
      }
      set
      {
        SetPropertyValue("Invalid", ref _invalid, value);
      }
    }

    private DateTime? _submissionDate;
    [Persistent]
    public DateTime? SubmissionDate
    {
      get
      {
        return _submissionDate;
      }
      set
      {
        SetPropertyValue("SubmissionDate", ref _submissionDate, value);
      }
    }

    private DateTime? _lastStatusUpdateDate;
    [Persistent]
    public DateTime? LastStatusUpdateDate
    {
      get
      {
        return _lastStatusUpdateDate;
      }
      set
      {
        SetPropertyValue("LastStatusUpdateDate", ref _lastStatusUpdateDate, value);
      }
    }


    #region Constructors

    public BUR_BatchSubmission() : base() { }
    public BUR_BatchSubmission(Session session) : base(session) { }

    #endregion
  }
}
