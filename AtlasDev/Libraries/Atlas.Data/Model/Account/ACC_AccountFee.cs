using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AccountFee: XPLiteObject
  {
    private Int64 _accountFeeId;
    [Key(AutoGenerate = true)]
    public Int64 AccountFeeId
    {
      get
      {
        return _accountFeeId;
      }
      set
      {
        SetPropertyValue("AccountFeeId", ref _accountFeeId, value);
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

    private ACC_TopUp _topUp;
    [Persistent("TopUpId")]
    public ACC_TopUp TopUp
    {
      get
      {
        return _topUp;
      }
      set
      {
        SetPropertyValue("TopUp", ref _topUp, value);
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

    public ACC_AccountFee() : base() { }
    public ACC_AccountFee(Session session) : base(session) { }

    #endregion
  }
}
