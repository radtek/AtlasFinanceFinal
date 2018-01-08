using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_SettlementType : XPLiteObject
  {
    private long _settlementTypeId;
    [Key(AutoGenerate = true)]
    public long SettlementTypeId
    {
      get
      {
        return _settlementTypeId;
      }
      set
      {
        SetPropertyValue("SettlementTypeId", ref _settlementTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.SettlementType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.SettlementType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.SettlementType>();
      }
    }

    private string _description;
    [Persistent,Size(40)]
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

    public ACC_SettlementType() : base() { }
    public ACC_SettlementType(Session session) : base(session) { }

    #endregion
  }
}
