using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AffordabilityCategoryType:XPLiteObject
  {
    private Int32 _affordabilityCategoryTypeId;
    [Key(AutoGenerate = false)]
    public Int32 AffordabilityCategoryTypeId
    {
      get
      {
        return _affordabilityCategoryTypeId;
      }
      set
      {
        SetPropertyValue("AffordabilityCategoryTypeId", ref _affordabilityCategoryTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.AffordabilityCategoryType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.AffordabilityCategoryType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.AffordabilityCategoryType>();
      }
    }

    private string _description;
    [Persistent, Size(20)]
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

    public ACC_AffordabilityCategoryType() : base() { }
    public ACC_AffordabilityCategoryType(Session session) : base(session) { }

    #endregion
  }
}
