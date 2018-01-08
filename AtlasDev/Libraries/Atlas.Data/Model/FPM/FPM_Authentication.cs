
namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_Authentication : XPLiteObject
  {
    private Int64 _AuthenticationId;
    [Key(AutoGenerate = true)]
    public Int64 AuthenticationId
    {
      get
      {
        return _AuthenticationId;
      }
      set
      {
        SetPropertyValue("AuthenticationId", ref _AuthenticationId, value);
      }
    }

    private PER_Person _Person;
    [Persistent("PersonId")]
    [Indexed]
    public PER_Person Person
    {
      get
      {
        return _Person;
      }
      set
      {
        SetPropertyValue("Person", ref _Person, value);
      }
    }

    private ACC_Account _Account;
    [Indexed]
    [Persistent("AccountId")]
    public ACC_Account Account
    {
      get
      {
        return _Account;
      }
      set
      {
        SetPropertyValue("Account", ref _Account, value);
      }
    }  

    private BNK_Detail _BankDetail;
    [Persistent("BankDetailId")]
    [Indexed]
    public BNK_Detail BankDetail
    {
      get
      {
        return _BankDetail;
      }
      set
      {
        SetPropertyValue("BankDetail", ref _BankDetail, value);
      }
    }

    private Contact _Contact;
    [Persistent("ContactId")]
    [Indexed]
    public Contact Contact
    {
      get
      {
        return _Contact;
      }
      set
      {
        SetPropertyValue("Contact", ref _Contact, value);
      }
    }
    
    private bool _Authenticated;
    [Indexed]
    [Persistent]
    public bool Authenticated
    {
      get { return _Authenticated; }
      set
      {
        SetPropertyValue("Authenticated", ref _Authenticated, value);
      }
    }

    private bool _Completed;
    [Persistent]
    public bool Completed
    {
      get
      {
        return _Completed;
      }
      set
      {
        SetPropertyValue("Completed", ref _Completed, value);
      }
    }

    private string _QuestionCount;
    [Persistent, Size(10)]
    public string QuestionCount
    {
      get
      {
        return _QuestionCount;
      }
      set
      {
        SetPropertyValue("QuestionCount", ref _QuestionCount, value);
      }
    }

    private decimal? _AuthenticatedPercentage;
    [Persistent]
    public decimal? AuthenticatedPercentage
    {
      get
      {
        return _AuthenticatedPercentage;
      }
      set
      {
        SetPropertyValue("AuthenticatedPercentage", ref _AuthenticatedPercentage, value);
      }
    }

    private decimal? _RequiredPercentage;
    [Persistent]
    public decimal? RequiredPercentage
    {
      get
      {
        return _RequiredPercentage;
      }
      set
      {
        SetPropertyValue("RequiredPercentage", ref _RequiredPercentage, value);
      }
    }

    private string _Reference;
    [Persistent, Size(25)]
    public string Reference
    {
      get
      {
        return _Reference;
      }
      set
      {
        SetPropertyValue("Reference", ref _Reference, value);
      }
    }

    private bool _Enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _Enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _Enabled, value);
      }
    }


    private PER_Person _overridePerson;
    [Persistent("OverridePersonId")]
    [Indexed]
    public PER_Person OverridePerson
    {
      get
      {
        return _overridePerson;
      }
      set
      {
        SetPropertyValue("OverridePerson", ref _overridePerson, value);
      }
    }

    private DateTime? _overrideDate;
    [Persistent("OverrideDate")]
    public DateTime? OverrideDate
    {
      get
      {
        return _overrideDate;
      }
      set
      {
        SetPropertyValue("OverrideDate", ref _overrideDate, value);
      }
    }

    private string _overrideReason;
    [Persistent, Size(500)]
    public string OverrideReason
    {
      get
      {
        return _overrideReason;
      }
      set
      {
        SetPropertyValue("OverrideReason", ref _overrideReason, value);
      }
    }

    private PER_Person _CreatedBy;
    [Persistent("CreatedBy")]
    public PER_Person CreatedBy
    {
      get
      {
        return _CreatedBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _CreatedBy, value);
      }
    }
    
    private DateTime? _CreateDate;
    [Persistent("CreatedDate")]
    public DateTime? CreateDate 
    {
      get { 
        return _CreateDate;
      }
      set {
        SetPropertyValue("CreateDate", ref _CreateDate, value);
      }
    }
    
    #region Constructors

    public FPM_Authentication() : base() { }
    public FPM_Authentication(Session session) : base(session) { }

    #endregion
  }
}
