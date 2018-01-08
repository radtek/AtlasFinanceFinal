using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class ACC_Status : XPLiteObject
  {
    private Int32 _statusId;
    [Key]
    public Int32 StatusId
    {
      get
      {
        return _statusId;
      }
      set
      {
        SetPropertyValue("StatusId", ref _statusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.AccountStatus Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.AccountStatus>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.AccountStatus>(); }
    }

    private string _description;
    [Persistent, Size(20)]
    [Indexed]
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

    public ACC_Status() : base() { }
    public ACC_Status(Session session) : base(session) { }

    #endregion
  }
}
