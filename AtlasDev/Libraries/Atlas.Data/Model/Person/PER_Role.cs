
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for PersonRoles
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

  public class PER_Role : XPLiteObject
  {

    private Int64 _PersonRoleId;
    [Key(AutoGenerate = true)]
    public Int64 PersonRoleId
    {
      get
      {
        return _PersonRoleId;
      }
      set
      {
        SetPropertyValue("PersonRoleId", ref _PersonRoleId, value);
      }
    }

    private RoleType _RoleType;
    [Persistent("RoleTypeId")]
    public RoleType RoleType
    {
      get
      {
        return _RoleType;
      }
      set
      {
        SetPropertyValue("RoleType", ref _RoleType, value);
      }
    }

    private PER_Person _Person;
    [Persistent("PersonId")]
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
    
    #region Constructors

    public PER_Role() : base() { }
    public PER_Role(Session session)
      : base(session)
    {

    }

    #endregion
  }
}
