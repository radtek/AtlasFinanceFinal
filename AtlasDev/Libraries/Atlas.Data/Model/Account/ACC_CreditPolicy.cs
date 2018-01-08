using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_CreditPolicy : XPLiteObject
  {
    private Int64 _creditPolicyId;
    [Key(AutoGenerate = false)]
    public Int64 CreditPolicyId
    {
      get
      {
        return _creditPolicyId;
      }
      set
      {
        SetPropertyValue("CreditPolicyId", ref _creditPolicyId, value);
      }
    }

    private Enumerators.Account.CreditPolicy _creditPolicy;
    [Persistent("CreditPolicy")]
    public Enumerators.Account.CreditPolicy CreditPolicy
    {
      get
      {
        return _creditPolicy;
      }
      set
      {
        SetPropertyValue("CreditPolicy", ref _creditPolicy, value);
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

    public ACC_CreditPolicy() : base() { }
    public ACC_CreditPolicy(Session session) : base(session) { }

    #endregion
  }
}
