using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AffordabilityOptionStatus : XPLiteObject
  {
    private Int32 _affordabilityOptionStatusId;
    [Key]
    public Int32 AffordabilityOptionStatusId
    {
      get
      {
        return _affordabilityOptionStatusId;
      }
      set
      {
        SetPropertyValue("AffordabilityOptionStatusId", ref _affordabilityOptionStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.AffordabilityOptionStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.AffordabilityOptionStatus>();
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

    public ACC_AffordabilityOptionStatus() : base() { }
    public ACC_AffordabilityOptionStatus(Session session) : base(session) { }

    #endregion
  }
}
