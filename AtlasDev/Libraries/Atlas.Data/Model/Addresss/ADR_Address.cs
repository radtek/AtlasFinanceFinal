/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Address
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
using Atlas.Domain.Interface;


namespace Atlas.Domain.Model
{
  public class ADR_Address : XPCustomObject
  {
    private Int64 _AddressId;
    [Key(AutoGenerate = true)]
    public Int64 AddressId
    {
      get
      {
        return _AddressId;
      }
      set
      {
        SetPropertyValue("AddressId", ref _AddressId, value);
      }
    }

    private ADR_Type _AddressType;
    [Persistent("AddressTypeId")]
    public ADR_Type AddressType
    {
      get
      {
        return _AddressType;
      }
      set
      {
        SetPropertyValue("AddressType", ref _AddressType, value);
      }
    }

    private string _Line1;
    [Persistent]
    public string Line1
    {
      get
      {
        return _Line1;
      }
      set
      {
        SetPropertyValue("Line1", ref _Line1, value);
      }
    }

    private string _Line2;
    [Persistent]
    public string Line2
    {
      get
      {
        return _Line2;
      }
      set
      {
        SetPropertyValue("Line2", ref _Line2, value);
      }
    }

    private string _Line3;
    [Persistent]
    public string Line3
    {
      get
      {
        return _Line3;
      }
      set
      {
        SetPropertyValue("Line3", ref _Line3, value);
      }
    }

    private string _Line4;
    [Persistent]
    public string Line4
    {
      get
      {
        return _Line4;
      }
      set
      {
        SetPropertyValue("Line4", ref _Line4, value);
      }
    }

    private Province _Province;
    [Persistent]
    public Province Province
    {
      get
      {
        return _Province;
      }
      set
      {
        SetPropertyValue("Province", ref _Province, value);
      }
    }

    private string _PostalCode;
    [Persistent]
    public string PostalCode
    {
      get
      {
        return _PostalCode;
      }
      set
      {
        SetPropertyValue("PostalCode", ref _PostalCode, value);
      }
    }

    private bool _IsActive;
    [Persistent]
    public bool IsActive
    {
      get
      {
        return _IsActive;
      }
      set
      {
        SetPropertyValue("IsActive", ref _IsActive, value);
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

    private DateTime _CreatedDT;
    [Persistent]
    public DateTime CreatedDT
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

    #region Constructors

    public ADR_Address() : base() { }
    public ADR_Address(Session session) : base(session) { }

    #endregion

    public override void AfterConstruction()
    {
      base.AfterConstruction();
      // Place here your initialization code. 
    }  

  }
}
