using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class LGR_TransactionTypeGroup:XPLiteObject
  {
    private int _transactionTypeGroupId;
    [Key]
    public int TransactionTypeGroupId
    {
      get
      {
        return _transactionTypeGroupId;
      }
      set
      {
        SetPropertyValue("TransactionTypeGroupId", ref _transactionTypeGroupId, value);
      }
    }

    [NonPersistent]
    public Enumerators.GeneralLedger.TransactionTypeGroup TType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionTypeGroup>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.GeneralLedger.TransactionTypeGroup>();
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

    #region Constructors

    public LGR_TransactionTypeGroup() : base() { }
    public LGR_TransactionTypeGroup(Session session) : base(session) { }

    #endregion
  }
}
