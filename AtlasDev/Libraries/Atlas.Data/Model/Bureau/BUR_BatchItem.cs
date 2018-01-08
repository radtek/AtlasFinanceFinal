namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class BUR_BatchItem : XPLiteObject
  {
    private Int64 _batchItemId;
    [Key(AutoGenerate = true)]
    public Int64 BatchItemId
    {
      get
      {
        return _batchItemId;
      }
      set
      {
        SetPropertyValue("BatchItemId", ref _batchItemId, value);
      }
    }

    private BUR_Batch _batch;
    [Persistent("BatchId")]
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

    private Enumerators.Risk.BatchTransactionType _transType;
    [Persistent]
    public Enumerators.Risk.BatchTransactionType TransType
    {
      get
      {
        return _transType;
      }
      set
      {
        SetPropertyValue("TransType", ref _transType, value);
      }
    }

    private Enumerators.Risk.BatchSubTransactionType _subTransType;
    [Persistent]
    public Enumerators.Risk.BatchSubTransactionType SubTransType
    {
      get
      {
        return _subTransType;
      }
      set
      {
        SetPropertyValue("SubTransType", ref _subTransType, value);
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

    #region Constructors

    public BUR_BatchItem() : base() { }
    public BUR_BatchItem(Session session) : base(session) { }

    #endregion
  }
}
