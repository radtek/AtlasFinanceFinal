using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class LGR_Type:XPLiteObject
  {
    private int _typeId;
    [Key]
    public int TypeId
    {
      get
      {
        return _typeId;
      }
      set
      {
        SetPropertyValue("TypeId", ref _typeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.GeneralLedger.Type Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.GeneralLedger.Type>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.GeneralLedger.Type>();
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

    public LGR_Type() : base() { }
    public LGR_Type(Session session) : base(session) { }

    #endregion
  }
}
