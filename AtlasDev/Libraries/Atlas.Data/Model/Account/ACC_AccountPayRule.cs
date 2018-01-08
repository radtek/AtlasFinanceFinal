using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AccountPayRule: XPLiteObject
  {
    private int _accountPayRuleId;
    [Key(AutoGenerate = true)]
    public int AccountPayRuleId
    {
      get
      {
        return _accountPayRuleId;
      }
      set
      {
        SetPropertyValue("AccountPayRuleId", ref _accountPayRuleId, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    [Association]
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

    private ACC_PayRule _payRule;
    [Persistent("PayRuleId")]
    public ACC_PayRule PayRule
    {
      get
      {
        return _payRule;
      }
      set
      {
        SetPropertyValue("PayRule", ref _payRule, value);
      }
    }

    private ACC_PayDate _payDate;
    [Persistent("PayDateId")]
    public ACC_PayDate PayDate
    {
      get
      {
        return _payDate;
      }
      set
      {
        SetPropertyValue("PayDate", ref _payDate, value);
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

     private DateTime? _disabledDate;
    [Persistent]
    public DateTime? DisabledDate
    {
      get
      {
        return _disabledDate;
      }
      set
      {
        SetPropertyValue("DisabledDate", ref _disabledDate, value);
      }
    }

    private DateTime? _createDate;
    [Persistent]
    public DateTime? CreateDate
    {
      get
      {
        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }

    private PER_Person _createUser;
    [Persistent("CreateUserId")]
    public PER_Person CreateUser
    {
      get
      {
        return _createUser;
      }
      set
      {
        SetPropertyValue("CreateUser", ref _createUser, value);
      }
    }

    

    #region Constructors

    public ACC_AccountPayRule() : base() { }
    public ACC_AccountPayRule(Session session) : base(session) { }

    #endregion
  }
}
