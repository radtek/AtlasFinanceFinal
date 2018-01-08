using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class BUR_Service:XPLiteObject
  {
    private Int64 _serviceId;
    [Key(AutoGenerate = true)]
    public Int64 ServiceId
    {
      get
      {
        return _serviceId;
      }
      set
      {
        SetPropertyValue("ServiceId", ref _serviceId, value);
      }
    }

    private BRN_Branch _branch;
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
    private Risk.ServiceType _serviceType;
    [Persistent("ServiceTypeId")]
    public Risk.ServiceType ServiceType
    {
      get
      {
        return _serviceType;
      }
      set
      {
        SetPropertyValue("ServiceType", ref _serviceType, value);
      }
    } 

    private string _name;
    [Persistent, Size(50)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
      }
    }

    private string _username;
    [Persistent, Size(30)]
    public string Username
    {
      get
      {
        return _username;
      }
      set
      {
        SetPropertyValue("Username", ref _username, value);
      }
    }

    private string _password;
    [Persistent, Size(50)]
    public string Password
    {
      get
      {
        return _password;
      }
      set
      {
        SetPropertyValue("Password", ref _password, value);
      }
    }

    private string _branchCode;
    [Persistent, Size(30)]
    public string BranchCode
    {
      get
      {
        return _branchCode;
      }
      set
      {
        SetPropertyValue("BranchCode", ref _branchCode, value);
      }
    }

    private int _days;
    [Persistent]
    public int Days
    {
      get
      {
        return _days;
      }
      set
      {
        SetPropertyValue("Days", ref _days, value);
      }
    }

    private char _destination;
    [Persistent, Size(1)]
    public char Destination
    {
      get
      {
        return _destination;
      }
      set
      {
        SetPropertyValue("Destination", ref _destination, value);
      }
    }

    private bool _enabled;
    [Persistent]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    #region Constructors

    public BUR_Service() : base() { }
    public BUR_Service(Session session) : base(session) { }

    #endregion
  }
}
