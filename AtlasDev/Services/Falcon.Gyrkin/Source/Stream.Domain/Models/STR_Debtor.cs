using System;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_Debtor : XPLiteObject
  {
    private long _debtorId;
    [Key(AutoGenerate = true)]
    public long DebtorId
    {
      get
      {
        return _debtorId;
      }
      set
      {
        SetPropertyValue("DebtorId", ref _debtorId, value);
      }
    }

    private string _idNumber;
    [Persistent, Size(20)]
    [Indexed]
    public string IdNumber
    {
      get
      {
        return _idNumber;
      }
      set
      {
        SetPropertyValue("IdNumber", ref _idNumber, value);
      }
    }

    private DateTime _dateOfBirth;
    [Persistent]
    public DateTime DateOfBirth
    {
      get
      {
        return _dateOfBirth;
      }
      set
      {
        SetPropertyValue("DateOfBirth", ref _dateOfBirth, value);
      }
    }

    private string _title;
    [Persistent, Size(4)]
    public string Title
    {
      get
      {
        return _title;
      }
      set
      {
        SetPropertyValue("Title", ref _title, value);
      }
    }

    private string _firstName;
    [Persistent, Size(80)]
    public string FirstName
    {
      get
      {
        return _firstName;
      }
      set
      {
        SetPropertyValue("FirstName", ref _firstName, value);
      }
    }

    private string _lastName;
    [Persistent, Size(80)]
    public string LastName
    {
      get
      {
        return _lastName;
      }
      set
      {
        SetPropertyValue("LastName", ref _lastName, value);
      }
    }

    private string _otherName;
    [Persistent, Size(60)]
    public string OtherName
    {
      get
      {
        return _otherName;
      }
      set
      {
        SetPropertyValue("OtherName", ref _otherName, value);
      }
    }

    private long _reference;
    [Persistent]
    [Indexed]
    public long Reference
    {
      get
      {
        return _reference;
      }
      set
      {
        SetPropertyValue("Reference", ref _reference, value);
      }
    }

    private string _employerCode;
    [Persistent, Size(40)]
    public string EmployerCode
    {
      get
      {
        return _employerCode;
      }
      set
      {
        SetPropertyValue("EmployerCode", ref _employerCode, value);
      }
    }

    private string _thirdPartyReferenceNo;
    [Persistent, Size(40)]
    public string ThirdPartyReferenceNo
    {
      get
      {
        return _thirdPartyReferenceNo;
      }
      set
      {
        SetPropertyValue("ThirdPartyReferenceNo", ref _thirdPartyReferenceNo, value);
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
    [Association("STR_DebtorContact", typeof(STR_DebtorContact))]
    public XPCollection<STR_DebtorContact> Contacts { get { return GetCollection<STR_DebtorContact>("Contacts"); } }
    [Association("STR_DebtorAddress", typeof(STR_DebtorAddress))]
    public XPCollection<STR_DebtorAddress> Addresses { get { return GetCollection<STR_DebtorAddress>("Addresses"); } }
    [Association("STR_Account", typeof(STR_Account))]
    public XPCollection<STR_Account> Accounts { get { return GetCollection<STR_Account>("Accounts"); } }


    #region Constructors

    public STR_Debtor()
    { }
    public STR_Debtor(Session session) : base(session) { }

    #endregion

  }
}