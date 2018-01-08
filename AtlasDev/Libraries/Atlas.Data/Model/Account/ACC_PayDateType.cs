using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_PayDateType : XPLiteObject
  {
    private int _payDateTypeId;
    [Key]
    public int PayDateTypeId
    {
      get
      {
        return _payDateTypeId;
      }
      set
      {
        SetPropertyValue("PayDateTypeId", ref _payDateTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.PayDateType Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.PayDateType>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.PayDateType>(); }
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

    public ACC_PayDateType() : base() { }
    public ACC_PayDateType(Session session) : base(session) { }

    #endregion
  }
}
