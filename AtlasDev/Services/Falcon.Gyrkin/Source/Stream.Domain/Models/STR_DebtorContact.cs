using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_DebtorContact : XPLiteObject
  {
    private long _debtorContactId;

    [Key(AutoGenerate = true)]
    public long DebtorContactId
    {
      get { return _debtorContactId; }
      set { SetPropertyValue("DebtorContactId", ref _debtorContactId, value); }
    }

    private STR_Debtor _debtor;

    [Persistent("DebtorId")]
    [Indexed]
    [Association("STR_DebtorContact")]
    public STR_Debtor Debtor
    {
      get { return _debtor; }
      set { SetPropertyValue("Debtor", ref _debtor, value); }
    }

    private ContactType _contactType;

    [Persistent("ContactTypeId")]
    public ContactType ContactType
    {
      get { return _contactType; }
      set { SetPropertyValue("ContactType", ref _contactType, value); }
    }

    private string _value;

    [Persistent]
    public string Value
    {
      get { return _value; }
      set { SetPropertyValue("Value", ref _value, value); }
    }

    private bool _isActive;

    [Persistent]
    public bool IsActive
    {
      get { return _isActive; }
      set { SetPropertyValue("IsActive", ref _isActive, value); }
    }

    private PER_Person _createUser;

    [Persistent("CreateUserId")]
    public PER_Person CreateUser
    {
      get { return _createUser; }
      set { SetPropertyValue("CreateUser", ref _createUser, value); }
    }

    private DateTime _createDate;

    [Persistent]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue("CreateDate", ref _createDate, value); }
    }


    #region Constructors

    public STR_DebtorContact()
    {
    }

    public STR_DebtorContact(Session session) : base(session)
    {
    }

    #endregion

  }
}