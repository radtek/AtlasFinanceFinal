using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_TransmissionTransaction : XPLiteObject
  {
    private Int64 _transmissionTransactionId;
    [Key(AutoGenerate = true)]
    public Int64 TransmissionTransactionId
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

    private DBT_TransmissionSet _transmissionSet;
    [Persistent("TransmissionSetId")]
    public DBT_TransmissionSet TransmissionSet
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

    private DBT_Transaction _transaction;
    [Persistent("TransactionId")]
    public DBT_Transaction Transaction
    {
      get
      {
        return _transaction;
      }
      set
      {
        SetPropertyValue("Transaction", ref _transaction, value);
      }
    }


    private DBT_Batch _batch;
    [Persistent("BatchId")]
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

    private DBT_ServiceMessage _serviceMessage;
    [Persistent("ServiceMessageId")]
    public DBT_ServiceMessage ServiceMessage
    {
      get
      {
        return _serviceMessage;
      }
      set
      {
        SetPropertyValue("ServiceMessage", ref _serviceMessage, value);
      }
    }


    private DateTime _createDate;
    [Persistent("CreateDate")]
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


    public DBT_TransmissionTransaction() : base() { }
    public DBT_TransmissionTransaction(Session session) : base(session) { }
  }
}