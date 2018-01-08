
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Company
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
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class CPY_Company : XPCustomObject
  {
    private Int64 _CompanyId;
    [Key(AutoGenerate = true)]
    public Int64 CompanyId
    {
      get
      {
        return _CompanyId;
      }
      set
      {
        SetPropertyValue("CompanyId", ref _CompanyId, value);
      }
    }

    private CPY_Company _ParentId;
    [Persistent]
    [Indexed]
    public CPY_Company ParentId
    {
      get
      {
        return _ParentId;
      }
      set
      {
        SetPropertyValue("ParentId", ref _ParentId, value);
      }
    }

    private int _EmployerCode;
    [Persistent]
    public int EmployerCode
    {
      get
      {
        return _EmployerCode;
      }
      set
      {
        SetPropertyValue("EmployerCode", ref _EmployerCode, value);
      }
    }

    private string _Name;
    [Persistent]
    public string Name
    {
      get
      {
        return _Name;
      }
      set
      {
        SetPropertyValue("Name", ref _Name, value);
      }
    }

    private PER_Person _CreatedBy;
    [Persistent]
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

     private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get
      {
        return _CreatedDT;
      }
      set
      {
        SetPropertyValue("CreatedDT", ref _CreatedDT, value);
      }
    }

    private DateTime? _DeletedDT;
    [Persistent]
    public DateTime? DeletedDT
    {
      get
      {
        return _DeletedDT;
      }
      set
      {
        SetPropertyValue("DeletedDT", ref _DeletedDT, value);
      }
    }

    private DateTime? _LastEditedDT;
    [Persistent]
    public DateTime? LastEditedDT
    {
      get
      {
        return _LastEditedDT;
      }
      set
      {
        SetPropertyValue("LastEditedDT", ref _LastEditedDT, value);
      }
    }

    private List<CPY_Addresses> _Addresses;
    [NonPersistent]
    public List<CPY_Addresses> GetAddresses
    {
      get
      {
        if (_Addresses == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _Addresses = Session.Query<CPY_Addresses>().Where(b => b.Company.CompanyId == CompanyId).ToList();
          }
        }
        return _Addresses;
      }
    }

    private List<CPY_BankDetails> _BankDetail;
    [NonPersistent]
    public List<CPY_BankDetails> GetBankDetails
    {
      get
      {
        if (_BankDetail == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _BankDetail = Session.Query<CPY_BankDetails>().Where(b => b.Company.CompanyId == CompanyId).ToList();
          }
        }
        return _BankDetail;
      }
    }

    private List<CPY_Contacts> _Contacts;
    [NonPersistent]
    public List<CPY_Contacts> GetContacts
    {
      get
      {
        if (_Contacts == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _Contacts = Session.Query<CPY_Contacts>().Where(b => b.Company.CompanyId == CompanyId).ToList();
          }
        }
        return _Contacts;
      }
    }

    private List<CPY_Branches> _companies;
    [NonPersistent]
    public List<CPY_Branches> GetCompanies
    {
      get
      {
        if (_companies == null)
        {
          if (Session != null && Session.IsConnected)
          {
            _companies = Session.Query<CPY_Branches>().Where(b => b.Company.CompanyId == CompanyId).ToList();
          }
        }
        return _companies;
      }
    }


    #region Constructors

    public CPY_Company() : base() { }
    public CPY_Company(Session session) : base(session) { }

    #endregion

  }
}
