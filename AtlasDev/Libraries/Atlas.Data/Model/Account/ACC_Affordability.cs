using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_Affordability : XPLiteObject
  {
    private Int64 _affordabilityId;
    [Key(AutoGenerate = true)]
    public Int64 AffordabilityId
    {
      get
      {
        return _affordabilityId;
      }
      set
      {
        SetPropertyValue("AffordabilityId", ref _affordabilityId, value);
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

    private ACC_AffordabilityCategory _affordabilityCategory;
    [Persistent("AffordabilityCategoryId")]
    public ACC_AffordabilityCategory AffordabilityCategory
    {
      get
      {
        return _affordabilityCategory;
      }
      set
      {
        SetPropertyValue("AffordabilityCategory", ref _affordabilityCategory, value);
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

    private DateTime _createDate;
    [Persistent]
    public DateTime CreateDate
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

    private DateTime? _deleteDate;
    [Persistent]
    public DateTime? DeleteDate
    {
      get
      {
        return _deleteDate;
      }
      set
      {
        SetPropertyValue("DeleteDate", ref _deleteDate, value);
      }
    }

    private PER_Person _deleteUser;
    [Persistent("DeleteUserId")]
    public PER_Person DeleteUser
    {
      get
      {
        return _deleteUser;
      }
      set
      {
        SetPropertyValue("DeleteUser", ref _deleteUser, value);
      }
    }

    #region Constructors

    public ACC_Affordability() : base() { }
    public ACC_Affordability(Session session) : base(session) { }

    #endregion
  }
}
