using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_StatusSubReason : XPLiteObject
  {
    private Int64 _statusSubReasonId;
    [Key]
    public Int64 StatusSubReasonId
    {
      get
      {
        return _statusSubReasonId;
      }
      set
      {
        SetPropertyValue("StatusSubReasonId", ref _statusSubReasonId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.AccountStatusSubReason Type
    {
      get { return Description.FromStringToEnum<Enumerators.Account.AccountStatusSubReason>(); }
      set { value = Description.FromStringToEnum<Enumerators.Account.AccountStatusSubReason>(); }
    }

    private ACC_StatusReason _statusReason;
    [Persistent("StatusReasonId")]
    public ACC_StatusReason StatusReason
    {
      get
      {
        return _statusReason;
      }
      set
      {
        SetPropertyValue("StatusReason", ref _statusReason, value);
      }
    }

    private string _description;
    [Persistent, Size(50)]
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

    #region Constructors

    public ACC_StatusSubReason() : base() { }
    public ACC_StatusSubReason(Session session) : base(session) { }

    #endregion
  }
}