using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AffordabilityOptionFee : XPLiteObject
  {
    private Int64 _affordabilityOptionFeeId;
    [Key(AutoGenerate = true)]
    public Int64 AffordabilityOptionFeeId   
    {
      get
      {
        return _affordabilityOptionFeeId;
      }
      set
      {
        SetPropertyValue("AffordabilityOptionFeeId", ref _affordabilityOptionFeeId, value);
      }
    }

    private ACC_AffordabilityOption _affordabilityOption;
    [Persistent("AffordabilityOptionId")]
    public ACC_AffordabilityOption AffordabilityOption
    {
      get
      {
        return _affordabilityOption;
      }
      set
      {
        SetPropertyValue("AffordabilityOption", ref _affordabilityOption, value);
      }
    }

    private ACC_AccountTypeFee _accountTypeFee;
    [Persistent("AccountTypeFeeId")]
    public ACC_AccountTypeFee AccountTypeFee
    {
      get
      {
        return _accountTypeFee;
      }
      set
      {
        SetPropertyValue("AccountTypeFee", ref _accountTypeFee, value);
      }
    }

    private Decimal _amount;
    [Persistent]
    public Decimal Amount
    {
      get
      {
        return _amount;
      }
      set
      {
        SetPropertyValue("Amount", ref _amount, value);
      }
    }

    #region Constructors

    public ACC_AffordabilityOptionFee() : base() { }
    public ACC_AffordabilityOptionFee(Session session) : base(session) { }

    #endregion
  }
}
