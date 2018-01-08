using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_DebtorContact : XPLiteObject
  {
    private Int64 _debtorContactId;
    [Key(AutoGenerate = true)]
    public Int64 DebtorContactId
    {
      get
      {
        return _debtorContactId;
      }
      set
      {
        SetPropertyValue("DebtorContactId", ref _debtorContactId, value);
      }
    }

    private STR_Debtor _debtor;
    [Persistent("DebtorId")]
    [Indexed]
    [Association("STR_DebtorContact")]
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

    private Contact _contact;
    [Persistent("ContactId")]
    public Contact Contact
    {
      get
      {
        return _contact;
      }
      set
      {
        SetPropertyValue("Contact", ref _contact, value);
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

    public STR_DebtorContact() : base() { }
    public STR_DebtorContact(Session session) : base(session) { }

    #endregion

  }
}