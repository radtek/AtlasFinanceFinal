using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_SettlementStatus : XPLiteObject
  {
    private long _settlementStatusId;
    [Key(AutoGenerate = true)]
    public long SettlementStatusId
    {
      get
      {
        return _settlementStatusId;
      }
      set
      {
        SetPropertyValue("SettlementStatusId", ref _settlementStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.SettlementStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.SettlementStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.SettlementStatus>();
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

    public ACC_SettlementStatus() : base() { }
    public ACC_SettlementStatus(Session session) : base(session) { }

    #endregion
  }
}
