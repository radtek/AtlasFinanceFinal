using System;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_Transaction : XPLiteObject
  {
    private long _transactionId;
    [Key(AutoGenerate = true)]
    public long TransactionId
    {
      get
      {
        return _transactionId;
      }
      set
      {
        SetPropertyValue("TransactionId", ref _transactionId, value);
      }
    }

    private STR_Account _account;
    [Persistent("AccountId")]
    [Indexed]
    public STR_Account Account
    {
      get
      {
        return _account;
      }
      set
      {
        SetPropertyValue("Account", ref _account, value);
      }
    }

    private long _reference;
    [Persistent]
    [Indexed]
    public long Reference
    {
      get
      {
        return _reference;
      }
      set
      {
        SetPropertyValue("Reference", ref _reference, value);
      }
    }

    private DateTime _transactionDate;
    [Persistent]
    [Indexed]
    public DateTime TransactionDate
    {
      get
      {
        return _transactionDate;
      }
      set
      {
        SetPropertyValue("TransactionDate", ref _transactionDate, value);
      }
    }

    private STR_TransactionType _transactionType;
    [Persistent("TransactionTypeId")]
    [Indexed]
    public STR_TransactionType TransactionType
    {
      get
      {
        return _transactionType;
      }
      set
      {
        SetPropertyValue("TransactionType", ref _transactionType, value);
      }
    }

    private STR_TransactionStatus _transactionStatus;
    [Persistent("TransactionStatusId")]
    [Indexed]
    public STR_TransactionStatus TransactionStatus
    {
      get
      {
        return _transactionStatus;
      }
      set
      {
        SetPropertyValue("TransactionStatus", ref _transactionStatus, value);
      }
    }

    private decimal _amount;
    [Persistent]
    [Indexed]
    public decimal Amount
    {
      get
      {
        return _amount;
      }
      set
      {
        SetPropertyValue("Amount", ref _amount, value);
      }
    }

    private int _instalmentNumber;
    [Persistent]
    [Indexed]
    public int InstalmentNumber
    {
      get
      {
        return _instalmentNumber;
      }
      set
      {
        SetPropertyValue("InstalmentNumber", ref _instalmentNumber, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    [Indexed]
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


    #region Constructors

    public STR_Transaction()
    { }
    public STR_Transaction(Session session) : base(session) { }

    #endregion

  }
}