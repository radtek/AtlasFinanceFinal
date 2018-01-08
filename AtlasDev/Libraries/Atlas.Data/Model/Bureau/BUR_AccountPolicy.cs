using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class BUR_AccountPolicy : XPLiteObject
  {
    private Int64 _accountPolicyId;
    [Key(AutoGenerate = true)]
    public Int64 AccountPolicyId
    {
      get
      {
        return _accountPolicyId;
      }
      set
      {
        SetPropertyValue("AccountPolicyId", ref _accountPolicyId, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    public ACC_Account Account
    {
      get
      {
        return _account;
      }
      set
      {
        SetPropertyValue("Account", ref _account, value);
      }
    }

    private BUR_Policy _policy;
    [Persistent("PolicyId")]
    public BUR_Policy Policy
    {
      get
      {
        return _policy;
      }
      set
      {
        SetPropertyValue("Policy", ref _policy, value);
      }
    }


    #region Constructors

    public BUR_AccountPolicy() : base() { }
    public BUR_AccountPolicy(Session session) : base(session) { }

    #endregion
  }
}
