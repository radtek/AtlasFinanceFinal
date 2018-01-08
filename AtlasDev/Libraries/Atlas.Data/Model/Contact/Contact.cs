
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Contact
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
  public class Contact : XPLiteObject
  {
    private Int64 _ContactId;
    [Key(AutoGenerate = true)]
    public Int64 ContactId
    {
      get
      {
        return _ContactId;
      }
      set
      {
        SetPropertyValue("ContactId", ref _ContactId, value);
      }
    }


    private ContactType _ContactType;
    [Persistent("ContactTypeId")]
    public ContactType ContactType
    {
      get
      {
        return _ContactType;
      }
      set
      {
        SetPropertyValue("ContactType", ref _ContactType, value);
      }
    }

    private string _Value;
    [Persistent]
    public string Value
    {
      get
      {
        return _Value;
      }
      set
      {
        SetPropertyValue("Value", ref _Value, value);
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

    public Contact() : base() { }
    public Contact(Session session) : base(session) { }

    #endregion
  }
}
