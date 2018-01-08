using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class LGR_TransactionType:XPLiteObject
  {
    private int _transactionTypeId;
    [Key(AutoGenerate=true)]
    public int TransactionTypeId
    {
      get
      {
        return _transactionTypeId;
      }
      set
      {
        SetPropertyValue("TransactionTypeId", ref _transactionTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.GeneralLedger.TransactionType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionType>();
      }
    }

    private LGR_TransactionTypeGroup _transactionTypeGroup;
    [Persistent("TransactionTypeGroupId")]
    public LGR_TransactionTypeGroup TransactionTypeGroup
    {
      get
      {
        return _transactionTypeGroup;
      }
      set
      {
        SetPropertyValue("TransactionTypeGroup", ref _transactionTypeGroup, value);
      }
    }

    private string _description;
    [Persistent, Size(40)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    private int _sortKey;
    [Persistent]
    public int SortKey
    {
      get
      {
        return _sortKey;
      }
      set
      {
        SetPropertyValue("SortKey", ref _sortKey, value);
      }
    }

    #region Constructors

    public LGR_TransactionType() : base() { }
    public LGR_TransactionType(Session session) : base(session) { }

    #endregion
  }
}
