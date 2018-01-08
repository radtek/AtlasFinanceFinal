using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class LGR_Transaction:XPLiteObject
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

    private ACC_Account _account;
    [Persistent("AccountId")]
    public ACC_Account Account
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

    private PER_Person _person;
    [Persistent("PersonId")]
    public PER_Person Person
    {
      get
      {
        return _person;
      }
      set
      {
        SetPropertyValue("Person", ref _person, value);
      }
    }

    private DateTime? _calculatedArrearsDate;
    [Persistent]
    public DateTime? CalculatedArrearsDate
    {
      get
      {
        return _calculatedArrearsDate;
      }
      set
      {
        SetPropertyValue("CalculatedArrearsDate", ref _calculatedArrearsDate, value);
      }
    }

    private LGR_TransactionType _transactionType;
    [Persistent("TransactionTypeId")]
    public LGR_TransactionType TransactionType
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

    private LGR_Fee _fee;
    [Persistent("FeeId")]
    public LGR_Fee Fee
    {
      get
      {
        return _fee;
      }
      set
      {
        SetPropertyValue("Fee", ref _fee, value);
      }
    }

    private LGR_Type _type;
    [Persistent("TypeId")]
    public LGR_Type Type
    {
      get
      {
        return _type;
      }
      set
      {
        SetPropertyValue("Type", ref _type, value);
      }
    }

    private decimal _amount;
    [Persistent]
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

    private DateTime _transactionDate;
    [Persistent]
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

    private DateTime? _accPacExportDate;
    [Persistent]
    public DateTime? AccPacExportDate
    {
      get
      {
        return _accPacExportDate;
      }
      set
      {
        SetPropertyValue("AccPacExportDate", ref _accPacExportDate, value);
      }
    }
    
    #region Constructors

    public LGR_Transaction() : base() { }
    public LGR_Transaction(Session session) : base(session) { }

    #endregion
  }
}
