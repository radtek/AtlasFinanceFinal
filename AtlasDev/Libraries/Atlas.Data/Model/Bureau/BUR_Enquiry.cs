// -----------------------------------------------------------------------
// <copyright file="RiskEnquiry.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class BUR_Enquiry : XPLiteObject
  {
    private Int64 _enquiryId;
    [Key(AutoGenerate = true)]
    public Int64 EnquiryId
    {
      get
      {
        return _enquiryId;
      }
      set
      {
        SetPropertyValue("EnquiryId", ref _enquiryId, value);
      }
    }

    private int _objectVersion;
    [Indexed]
    [Persistent("ObjectVersion")]
    public int ObjectVersion
    {
      get
      {

        return _objectVersion;
      }
      set
      {
        SetPropertyValue("ObjectVersion", ref _objectVersion, value);
      }
    }

    private BRN_Branch _branch;
    [Indexed]
    [Persistent("BranchId")]
    public BRN_Branch Branch
    {
      get
      {

        return _branch;
      }
      set
      {
        SetPropertyValue("Branch", ref _branch, value);
      }
    }


    private Guid _correlationId;
    [Indexed]
    [Persistent("CorrelationId")]
    public Guid CorrelationId
    {
      get
      {

        return _correlationId;
      }
      set
      {
        SetPropertyValue("CorrelationId", ref _correlationId, value);
      }
    }

    private BUR_Enquiry _previousEnquiry;
    [Indexed]
    [Persistent("PreviousEnquiryId")]
    public BUR_Enquiry PreviousEnquiry
    {
      get { return _previousEnquiry; }
      set
      {
        SetPropertyValue("PreviousEnquiryId", ref _previousEnquiry, value);
      }
    }


    private ACC_Account _account;
    [Indexed]
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

    private BUR_Service _service;
    [Indexed]
    [Persistent("ServiceId")]
    public BUR_Service Service
    {
      get
      {
        return _service;
      }
      set
      {
        SetPropertyValue("Service", ref _service, value);
      }
    }

    private Enumerators.Risk.RiskEnquiryType _EnquiryType;
    [Persistent]
    public Enumerators.Risk.RiskEnquiryType EnquiryType
    {
      get { return _EnquiryType; }
      set
      {
        SetPropertyValue("EnquiryType", ref _EnquiryType, value);
      }
    }

    private Enumerators.Risk.RiskTransactionType _riskTransactionType;
    [Persistent]
    public Enumerators.Risk.RiskTransactionType TransactionType
    {
      get { return _riskTransactionType; }
      set
      {
        SetPropertyValue("TransactionType", ref _riskTransactionType, value);
      }
    }

    private string _IdentityNum;
    [Persistent]
    public string IdentityNum
    {
      get
      {
        return _IdentityNum;
      }
      set
      {
        SetPropertyValue("IdentityNum", ref _IdentityNum, value);
      }
    }

    private string _FirstName;
    [Persistent]
    public string FirstName
    {
      get
      {
        return _FirstName;
      }
      set
      {
        SetPropertyValue("FirstName", ref _FirstName, value);
      }
    }

    private string _LastName;
    [Persistent]
    public string LastName
    {
      get
      {
        return _LastName;
      }
      set
      {
        SetPropertyValue("LastName", ref _LastName, value);
      }
    }

    private string _message;
    [Persistent]
    public string Message
    {
      get
      {
        return _message;
      }
      set
      {
        SetPropertyValue("Message", ref _message, value);
      }
    }

    private bool _IsSucess;
    [Persistent]
    public bool IsSucess
    {
      get
      {
        return _IsSucess;
      }
      set
      {
        SetPropertyValue("IsSucess", ref _IsSucess, value);
      }
    }

    private DateTime _EnquiryDate;
    [Persistent]
    public DateTime EnquiryDate
    {
      get
      {
        return _EnquiryDate;
      }
      set
      {
        SetPropertyValue("EnquiryDate", ref _EnquiryDate, value);
      }
    }

    private PER_Person _createdUser;
    [Persistent]
    public PER_Person CreatedUser
    {
      get
      {
        return _createdUser;
      }
      set
      {
        SetPropertyValue("CreatedUser", ref _createdUser, value);
      }
    }

    private DateTime? _createDate;
    [Persistent]
    public DateTime? CreateDate
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

    [Association("BUR_StorageEnquiry", typeof(BUR_Storage))]
    public XPCollection<BUR_Storage> Storage { get { return GetCollection<BUR_Storage>("Storage"); } }

    #region Constructors

    public BUR_Enquiry() : base() { }
    public BUR_Enquiry(Session session) : base(session) { }

    #endregion
  }
}
