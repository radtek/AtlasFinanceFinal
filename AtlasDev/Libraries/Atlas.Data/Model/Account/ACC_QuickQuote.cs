using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_QuickQuote : XPCustomObject
  {
    private Int64 _quickQuoteId;
    [Key(AutoGenerate = true)]
    public Int64 QuickQuoteId
    {
      get
      {
        return _quickQuoteId;
      }
      set
      {
        SetPropertyValue("QuickQuoteId", ref _quickQuoteId, value);
      }
    }

    private string _idNumber;
    [Persistent, Size(13)]
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

    private string _surname;
    [Persistent, Size(80)]
    public string Surname
    {
      get
      {
        return _surname;
      }
      set
      {
        SetPropertyValue("Surname", ref _surname, value);
      }
    }

    private PER_Person _person;
    [Persistent("PersonId")]
    public PER_Person Person
    {
      get
      {
        return _person;
      }
      set
      {
        SetPropertyValue("Person", ref _person, value);
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

    private Decimal _appliedAmount;
    [Persistent]
    public Decimal AppliedAmount
    {
      get
      {
        return _appliedAmount;
      }
      set
      {
        SetPropertyValue("AppliedAmount", ref _appliedAmount, value);
      }
    }

    private int _period;
    [Persistent]
    public int Period
    {
      get
      {
        return _period;
      }
      set
      {
        SetPropertyValue("Period", ref _period, value);
      }
    }

    private ACC_PeriodFrequency _periodFrequency;
    [Persistent("PeriodFrequencyId")]
    public ACC_PeriodFrequency PeriodFrequency
    {
      get
      {
        return _periodFrequency;
      }
      set
      {
        SetPropertyValue("PeriodFrequency", ref _periodFrequency, value);
      }
    }

    private BUR_Enquiry _enquiry;
    [Persistent("EnquiryId")]
    public BUR_Enquiry Enquiry
    {
      get
      {
        return _enquiry;
      }
      set
      {
        SetPropertyValue("Enquiry", ref _enquiry, value);
      }
    }

    private string _homeNo1;
    [Persistent, Size(80)]
    public string HomeNo1
    {
      get
      {
        return _homeNo1;
      }
      set
      {
        SetPropertyValue("HomeNo1", ref _homeNo1, value);
      }
    }

    private string _homeNo2;
    [Persistent, Size(80)]
    public string HomeNo2
    {
      get
      {
        return _homeNo2;
      }
      set
      {
        SetPropertyValue("HomeNo2", ref _homeNo2, value);
      }
    }

    private string _workNo1;
    [Persistent, Size(80)]
    public string WorkNo1
    {
      get
      {
        return _workNo1;
      }
      set
      {
        SetPropertyValue("WorkNo1", ref _workNo1, value);
      }
    }

    private string _workNo2;
    [Persistent, Size(80)]
    public string WorkNo2
    {
      get
      {
        return _workNo2;
      }
      set
      {
        SetPropertyValue("WorkNo2", ref _workNo2, value);
      }
    }

    private string _cellNo1;
    [Persistent, Size(80)]
    public string CellNo1
    {
      get
      {
        return _cellNo1;
      }
      set
      {
        SetPropertyValue("CellNo1", ref _cellNo1, value);
      }
    }

    private string _cellNo2;
    [Persistent, Size(80)]
    public string CellNo2
    {
      get
      {
        return _cellNo2;
      }
      set
      {
        SetPropertyValue("CellNo2", ref _cellNo2, value);
      }
    }

    private string _email;
    [Persistent, Size(80)]
    public string Email
    {
      get
      {
        return _email;
      }
      set
      {
        SetPropertyValue("Email", ref _email, value);
      }
    }

    private bool _isLead;
    [Persistent]
    public bool IsLead
    {
      get
      {
        return _isLead;
      }
      set
      {
        SetPropertyValue("IsLead", ref _isLead, value);
      }
    }

    private CPY_Company _leadCompany;
    [Persistent("LeadCompanyId")]
    public CPY_Company LeadCompany
    {
      get
      {
        return _leadCompany;
      }
      set
      {
        SetPropertyValue("LeadCompany", ref _leadCompany, value);
      }
    }

    private DateTime _createdDate;
    [Persistent]
    public DateTime CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }

    private PER_Person _createdBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _createdBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _createdBy, value);
      }
    }

    #region Constructors

    public ACC_QuickQuote() : base() { }
    public ACC_QuickQuote(Session session) : base(session) { }

    #endregion

  }
}
