using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_DebtorAddress : XPLiteObject
  {
    private Int64 _debtorAddressId;
    [Key(AutoGenerate = true)]
    public Int64 DebtorAddressId
    {
      get
      {
        return _debtorAddressId;
      }
      set
      {
        SetPropertyValue("DebtorAddressId", ref _debtorAddressId, value);
      }
    }

    private STR_Debtor _debtor;
    [Persistent("DebtorId")]
    [Indexed]
    [Association("STR_DebtorAddress")]
    public STR_Debtor Debtor
    {
      get
      {
        return _debtor;
      }
      set
      {
        SetPropertyValue("Debtor", ref _debtor, value);
      }
    }

    private ADR_Address _address;
    [Persistent("AddressId")]
    public ADR_Address Address
    {
      get
      {
        return _address;
      }
      set
      {
        SetPropertyValue("Address", ref _address, value);
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


    #region Constructors

    public STR_DebtorAddress() : base() { }
    public STR_DebtorAddress(Session session) : base(session) { }

    #endregion

  }
}
