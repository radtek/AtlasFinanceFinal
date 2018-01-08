using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_DebtorAddress : XPLiteObject
  {
    private long _debtorAddressId;
    [Key(AutoGenerate = true)]
    public long DebtorAddressId
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

    private ADR_Type _addressType;
    [Persistent("AddressTypeId")]
    public ADR_Type AddressType
    {
      get
      {
        return _addressType;
      }
      set
      {
        SetPropertyValue("AddressType", ref _addressType, value);
      }
    }

    private string _line1;
    [Persistent]
    public string Line1
    {
      get
      {
        return _line1;
      }
      set
      {
        SetPropertyValue("Line1", ref _line1, value);
      }
    }

    private string _line2;
    [Persistent]
    public string Line2
    {
      get
      {
        return _line2;
      }
      set
      {
        SetPropertyValue("Line2", ref _line2, value);
      }
    }

    private string _line3;
    [Persistent]
    public string Line3
    {
      get
      {
        return _line3;
      }
      set
      {
        SetPropertyValue("Line3", ref _line3, value);
      }
    }

    private string _line4;
    [Persistent]
    public string Line4
    {
      get
      {
        return _line4;
      }
      set
      {
        SetPropertyValue("Line4", ref _line4, value);
      }
    }

    private Province _province;
    [Persistent]
    public Province Province
    {
      get
      {
        return _province;
      }
      set
      {
        SetPropertyValue("Province", ref _province, value);
      }
    }

    private string _postalCode;
    [Persistent]
    public string PostalCode
    {
      get
      {
        return _postalCode;
      }
      set
      {
        SetPropertyValue("PostalCode", ref _postalCode, value);
      }
    }

    private bool _isActive;
    [Persistent]
    public bool IsActive
    {
      get
      {
        return _isActive;
      }
      set
      {
        SetPropertyValue("IsActive", ref _isActive, value);
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

    public STR_DebtorAddress()
    { }
    public STR_DebtorAddress(Session session) : base(session) { }

    #endregion

  }
}
