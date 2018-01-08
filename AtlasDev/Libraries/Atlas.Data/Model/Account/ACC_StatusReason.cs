using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_StatusReason:XPLiteObject
  {
    private Int64 _statusReasonId;
    [Key]
    public Int64 StatusReasonId
    {
      get
      {
        return _statusReasonId;
      }
      set
      {
        SetPropertyValue("StatusReasonId", ref _statusReasonId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.AccountStatusReason Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.AccountStatusReason>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.AccountStatusReason>(); }
    }

    private ACC_Status _status;
    [Persistent("StatusId")]
    public ACC_Status Status
    {
      get
      {
        return _status;
      }
      set
      {
        SetPropertyValue("Status", ref _status, value);
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

    private bool _enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    private bool _multipleSubReasons;
    [Persistent]
    public bool MultipleSubReasons
    {
      get
      {
        return _multipleSubReasons;
      }
      set
      {
        SetPropertyValue("MultipleSubReasons", ref _multipleSubReasons, value);
      }
    }

    #region Constructors

    public ACC_StatusReason() : base() { }
    public ACC_StatusReason(Session session) : base(session) { }

    #endregion
  }
}
