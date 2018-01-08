
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for PersonSecurity
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
using DevExpress.Xpo;
using Atlas.Domain.Interface;
using Atlas.Domain.Security;

namespace Atlas.Domain.Model
{
  public class PER_Security : XPCustomObject, IAudit
  {

    private Int64 _SecurityId;
    [Key(AutoGenerate = true)]
    public Int64 SecurityId
    {
      get
      {
        return _SecurityId;
      }
      set
      {
        SetPropertyValue("SecurityId", ref _SecurityId, value);
      }
    }

    private PER_Person _Person;
    [Persistent("PersonId"), Indexed]
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

    private string _LegacyOperatorId;
    [Persistent, Size(4), Indexed]
    public string LegacyOperatorId
    {
      get
      {
        return _LegacyOperatorId;
      }
      set
      {
        SetPropertyValue("LegacyOperatorId", ref _LegacyOperatorId, value);
      }
    }

    private string _Username;

    [Persistent, Size(20)]
    public string Username
    {
      get
      {
        return _Username;
      }
      set
      {
        SetPropertyValue("Username", ref _Username, value);
      }
    }

    private string _Salt;
    [Persistent, Size(150)]
    public string Salt
    {
      get
      {
        return _Salt;
      }
      set
      {
        SetPropertyValue("Salt", ref _Salt, value);
      }
    }

    private string _Hash;
    [Persistent, Size(200)]
    public string Hash
    {
      get
      {
        return _Hash;
      }
      set
      {
        SetPropertyValue("Hash", ref _Hash, value);
      }
    }

    private string _IP;
    [Persistent, Size(20)]
    public string IP
    {
      get
      {
        return _IP;
      }
      set
      {
        SetPropertyValue("IP", ref _IP, value);
      }
    }

    private bool _IsActive;
    [Persistent]
    public bool IsActive
    {
      get { return _IsActive; }
      set { SetPropertyValue("IsActive", ref _IsActive, value); }
    }

    private bool _IsLocked;
    [Persistent]
    public bool IsLocked
    {
      get { return _IsLocked; }
      set { SetPropertyValue("IsLocked", ref _IsLocked, value); }        
    }

    [NonPersistent]
    public bool InvalidUserNameOrPassword { get; set; }

    private DateTime? _LastLoggedIn;
    [Persistent]
    public DateTime? LastLoggedIn
    {
      get
      {
        return _LastLoggedIn;
      }
      set
      {
        SetPropertyValue("LastLoggedIn", ref _LastLoggedIn, value);
      }
    }

    private bool _LoggedIn;
    [Persistent]
    public bool LoggedIn
    {
      get
      {
        return _LoggedIn;
      }
      set
      {
        SetPropertyValue("LoggedIn", ref _LoggedIn, value);
      }
    }

    private int _LogInAttemptCount;
    [Persistent]
    public int LogInAttemptCount
    {
      get { return _LogInAttemptCount; }
      set { SetPropertyValue("LogInAttemptCount", ref _LogInAttemptCount, value); }
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

    private PER_Person _DeletedBy;
    [Persistent]
    public PER_Person DeletedBy
    {
      get
      {
        return _DeletedBy;
      }
      set
      {
        SetPropertyValue("DeletedBy", ref _DeletedBy, value);
      }
    }

    private PER_Person _LastEditedBy;
    [Persistent]
    public PER_Person LastEditedBy
    {
      get
      {
        return _LastEditedBy;
      }
      set
      {
        SetPropertyValue("LastEditedBy", ref _LastEditedBy, value);
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

    private string _HREmployeeCode;
    [Persistent, Size(20)]
    public string HREmployeeCode
    {
      get
      {
        return _HREmployeeCode;
      }
      set
      {
        SetPropertyValue("HREmployeeCode", ref _HREmployeeCode, value);
      }
    }
  

    #region Constructors

    public PER_Security() : base() { }
    public PER_Security(Session session) : base(session) { }

    #endregion
  }
}
