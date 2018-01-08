
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Person
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Data.WcfLinq.Helpers;
using DevExpress.Xpo;

using Atlas.Domain.Interface;
using Atlas.Enumerators;


namespace Atlas.Domain.Model
{
  public class PER_Person : XPCustomObject
  {
    private Int64 _PersonId;

    [Key(AutoGenerate = true)]
    public Int64 PersonId
    {
      get { return _PersonId; }
      set { SetPropertyValue("PersonId", ref _PersonId, value); }
    }

    private XPDelayedProperty _Security = new XPDelayedProperty();

    [Persistent("SecurityId")]
    [Delayed("_Security")]
    public PER_Security Security
    {
      get { return (PER_Security)_Security.Value; }
      set { _Security.Value = value; }
    }


    /// <summary>
    /// Used to link to web security
    /// </summary>
    private string _webReference;

    [Persistent]
    [Indexed]
    public string WebReference
    {
      get { return _webReference; }
      set { SetPropertyValue("WebReference", ref _webReference, value); }
    }


    private XPDelayedProperty _PersonType = new XPDelayedProperty();

    [Persistent("TypeId")]
    [Delayed("_PersonType")]
    public PER_Type PersonType
    {
      get { return (PER_Type)_PersonType.Value; }
      set { _PersonType.Value = value; }
    }

    private XPDelayedProperty _Branch = new XPDelayedProperty();

    [Persistent("BranchId")]
    [Delayed("_Branch")]
    public BRN_Branch Branch
    {
      get { return (BRN_Branch)_Branch.Value; }
      set { _Branch.Value = value; }
    }

    private XPDelayedProperty _Employer = new XPDelayedProperty();

    [Persistent("EmployerId")]
    [Delayed("_Employer")]
    public CPY_Company Employer
    {
      get { return (CPY_Company)_Employer.Value; }
      set { _Employer.Value = value; }
    }

    private string _LegacyClientCode;

    [Persistent, Size(20)]
    public string LegacyClientCode
    {
      get { return _LegacyClientCode; }
      set { SetPropertyValue("LegacyClientCode", ref _LegacyClientCode, value); }
    }

    private string _ClientCode;

    [Persistent, Size(15)]
    public string ClientCode
    {
      get { return _ClientCode; }
      set { SetPropertyValue("ClientCode", ref _ClientCode, value); }
    }

    private string _Designation;

    [Persistent, Size(5)]
    public string Designation
    {
      get { return _Designation; }
      set { SetPropertyValue("Designation", ref _Designation, value); }
    }

    private string _Firstname;

    [Persistent, Size(50)]
    [Indexed]
    public string Firstname
    {
      get { return _Firstname; }
      set { SetPropertyValue("Firstname", ref _Firstname, value); }
    }

    private string _Middlename;

    [Persistent, Size(50)]
    public string Middlename
    {
      get { return _Middlename; }
      set { SetPropertyValue("Middlename", ref _Middlename, value); }
    }

    private string _Lastname;

    [Persistent, Size(50)]
    [Indexed]
    public string Lastname
    {
      get { return _Lastname; }
      set { SetPropertyValue("Lastname", ref _Lastname, value); }
    }

    private string _Othername;

    [Persistent, Size(50)]
    public string Othername
    {
      get { return _Othername; }
      set { SetPropertyValue("Othername", ref _Othername, value); }
    }

    private General.EthnicGroup _ethnicGroup;

    [Persistent]
    public General.EthnicGroup EthnicGroup
    {
      get { return _ethnicGroup; }
      set { SetPropertyValue("EthnicGroup", ref _ethnicGroup, value); }
    }

    private string _Email;

    [Persistent, Size(60)]
    public string Email
    {
      get { return _Email; }
      set { SetPropertyValue("Email", ref _Email, value); }
    }

    private string _IdNum;

    [Persistent, Size(30)]
    [Indexed]
    public string IdNum
    {
      get { return _IdNum; }
      set { SetPropertyValue("IdNum", ref _IdNum, value); }
    }

    private string _SalaryFrequency;

    [Persistent, Size(10)]
    public string SalaryFrequency
    {
      get { return _SalaryFrequency; }
      set { SetPropertyValue("SalaryFrequency", ref _SalaryFrequency, value); }
    }

    private string _Gender;

    [Persistent, Size(6)]
    public string Gender
    {
      get { return _Gender; }
      set { SetPropertyValue("Gender", ref _Gender, value); }
    }

    private string _Race;

    [Persistent, Size(1)]
    public string Race
    {
      get { return _Race; }
      set { SetPropertyValue("Race", ref _Race, value); }
    }

    private XPDelayedProperty _Host = new XPDelayedProperty();

    [Persistent]
    [Delayed("_Host")]
    public Host Host
    {
      get { return (Host)_Host.Value; }
      set { _Host.Value = value; }
    }

    private DateTime _DateOfBirth;

    [Persistent]
    public DateTime DateOfBirth
    {
      get { return _DateOfBirth; }
      set { SetPropertyValue("DateOfBirth", ref _DateOfBirth, value); }
    }

    private XPDelayedProperty _CreatedBy = new XPDelayedProperty();

    [Delayed("_CreatedBy")]
    [Persistent]
    public PER_Person CreatedBy
    {
      get { return (PER_Person)_CreatedBy.Value; }
      set { _CreatedBy.Value = value; }
    }

    private DateTime? _CreatedDT;

    [Persistent]
    public DateTime? CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }

    private DateTime? _LastEditedDT;

    [Persistent]
    public DateTime? LastEditedDT
    {
      get { return _LastEditedDT; }
      set { SetPropertyValue("LastEditedDT", ref _LastEditedDT, value); }
    }

    private List<PER_BankDetails> _bankDetails;
    [NonPersistent]
    public List<PER_BankDetails> GetBankDetails
    {
      get
      {
        if (_bankDetails == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _bankDetails = Session.Query<PER_BankDetails>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _bankDetails;
      }
    }

    private List<PER_AddressDetails> _AddressDetails;
    [NonPersistent]
    public List<PER_AddressDetails> GetAddressDetails
    {
      get
      {
        if (_AddressDetails == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _AddressDetails = Session.Query<PER_AddressDetails>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _AddressDetails;
      }
    }

    private List<PER_Contacts> _contacts;
    [NonPersistent]
    public List<PER_Contacts> GetContacts
    {
      get
      {
        if (_contacts == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _contacts = Session.Query<PER_Contacts>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _contacts;
      }
    }

    private List<NUC_NuCard> _cards;
    [NonPersistent]
    public List<NUC_NuCard> GetCards
    {
      get
      {
        if (_cards == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _cards = Session.Query<NUC_NuCard>().Where(b => b.AllocatedPerson.PersonId == PersonId).ToList();
          }
        }
        return _cards;
      }
    }

    private List<PER_EmploymentHistory> _employmentHistory;
    [NonPersistent]
    public List<PER_EmploymentHistory> GetEmploymentHistory
    {
      get
      {
        if (_employmentHistory == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _employmentHistory = Session.Query<PER_EmploymentHistory>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _employmentHistory;
      }
    }

    private List<PER_Relation> _relations;
    [NonPersistent]
    public List<PER_Relation> GetRelations
    {
      get
      {
        if (_relations == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _relations = Session.Query<PER_Relation>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _relations;
      }
    }

    private List<PER_Role> _roles;
    [NonPersistent]
    public List<PER_Role> GetRoles
    {
      get
      {
        if (_roles == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _roles = Session.Query<PER_Role>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _roles;
      }
    }

    private List<WEB_UserRole> _webRoles;
    [NonPersistent]
    public List<WEB_UserRole> GetWebRoles
    {
      get
      {
        if (_webRoles == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _webRoles = Session.Query<WEB_UserRole>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _webRoles;
      }
    }

    private List<WFL_UserGroupLink> _userGroupLinks;
    [NonPersistent]
    public List<WFL_UserGroupLink> GetUserGroupLinks
    {
      get
      {
        if (_userGroupLinks == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _userGroupLinks = Session.Query<WFL_UserGroupLink>().Where(b => b.User.PersonId == PersonId).ToList();
          }
        }
        return _userGroupLinks;
      }
    }

    private List<WFL_ProcessStepJob> _processStepJobs;
    [NonPersistent]
    public List<WFL_ProcessStepJob> GetProcessStepJobs
    {
      get
      {
        if (_processStepJobs == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _processStepJobs = Session.Query<WFL_ProcessStepJob>().Where(b => b.User.PersonId == PersonId).ToList();
          }
        }
        return _processStepJobs;
      }
    }

    private List<PER_Branch> _branches;
[NonPersistent]
    public List<PER_Branch> GetBranches
    {
      get
      {
        if (_branches == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _branches = Session.Query<PER_Branch>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _branches;
      }
    }

    private List<PER_Region> _region;
    [NonPersistent]
    public List<PER_Region> GetRegions
    {
      get
      {
        if (_region == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _region = Session.Query<PER_Region>().Where(b => b.Person.PersonId == PersonId).ToList();
          }
        }
        return _region;
      }
    }

    #region Constructors

    public PER_Person()
      : base()
    {
    }

    public PER_Person(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
