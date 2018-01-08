using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public sealed class ACC_PayRule: XPLiteObject
  {
    private int _payRuleId;
    [Key(AutoGenerate = false)]
    public int PayRuleId
    {
      get
      {
        return _payRuleId;
      }
      set
      {
        SetPropertyValue("PayRuleId", ref _payRuleId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.PayRule Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.PayRule>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.PayRule>();
      }
    }

    private string _description;
    [Persistent]
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

    public ACC_PayRule() : base() { }
    public ACC_PayRule(Session session) : base(session) { }

    #endregion
  }
}
