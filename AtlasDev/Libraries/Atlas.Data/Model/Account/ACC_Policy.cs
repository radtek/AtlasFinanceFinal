using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class ACC_Policy : XPLiteObject
  {
    private Int64 _policyId;
    [Key(AutoGenerate = false)]
    public Int64 PolicyId
    {
      get
      {
        return _policyId;
      }
      set
      {
        SetPropertyValue("PolicyId", ref _policyId, value);
      }
    }

    private string _description;
    [Persistent, Size(100)]
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
    [Persistent("Enabled")]
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

    public ACC_Policy() : base() { }
    public ACC_Policy(Session session) : base(session) { }

    #endregion
  }
}
