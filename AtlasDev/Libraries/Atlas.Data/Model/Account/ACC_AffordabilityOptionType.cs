using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AffordabilityOptionType : XPLiteObject
  {
    private Int32 _affordabilityOptionTypeId;
    [Key(AutoGenerate = false)]
    public Int32 AffordabilityOptionTypeId
    {
      get
      {
        return _affordabilityOptionTypeId;
      }
      set
      {
        SetPropertyValue("AffordabilityOptionTypeId", ref _affordabilityOptionTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.AffordabilityOptionType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionType>();
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

    public ACC_AffordabilityOptionType() : base() { }
    public ACC_AffordabilityOptionType(Session session) : base(session) { }

    #endregion
  }
}
