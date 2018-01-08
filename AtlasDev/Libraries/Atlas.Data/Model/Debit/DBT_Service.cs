using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class DBT_Service : XPLiteObject
  {
    private int _serviceId;
    [Key(AutoGenerate = true)]
    public int ServiceId
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

    private string _referenceName;
    [Persistent, Size(30)]
    public string ReferenceName
    {
      get
      {
        return _referenceName;
      }
      set
      {
        SetPropertyValue("ReferenceName", ref _referenceName, value);
      }
    }

    private string _userCode;
    [Persistent, Size(30)]
    public string UserCode
    {
      get
      {
        return _userCode;
      }
      set
      {
        SetPropertyValue("UserCode", ref _userCode, value);
      }
    }

    private string _userName;
    [Persistent, Size(30)]
    public string UserName
    {
      get
      {
        return _userName;
      }
      set
      {
        SetPropertyValue("UserName", ref _userName, value);
      }
    }

    private string _userReference;
    [Persistent, Size(50)]
    public string UserReference
    {
      get
      {
        return _userReference;
      }
      set
      {
        SetPropertyValue("UserReference", ref _userReference, value);
      }
    }

    private char _environment;
    [Persistent, Size(1)]
    public char Environment
    {
      get
      {
        return _environment;
      }
      set
      {
        SetPropertyValue("Environment", ref _environment, value);
      }
    }

    private string _accountNo;
    [Persistent, Size(25)]
    public string AccountNo
    {
      get
      {
        return _accountNo;
      }
      set
      {
        SetPropertyValue("AccountNo", ref _accountNo, value);
      }
    }

    private string _branchCode;
    [Persistent, Size(15)]
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

    private int _nextTransmissionNo;
    [Persistent]
    public int NextTransmissionNo
    {
      get
      {
        return _nextTransmissionNo;
      }
      set
      {
        SetPropertyValue("NextTransmissionNo", ref _nextTransmissionNo, value);
      }
    }

    private int _nextGenerationNo;
    [Persistent]
    public int NextGenerationNo
    {
      get
      {
        return _nextGenerationNo;
      }
      set
      {
        SetPropertyValue("NextGenerationNo", ref _nextGenerationNo, value);
      }
    }

    private int _nextSequenceNo;
    [Persistent]
    public int NextSequenceNo
    {
      get
      {
        return _nextSequenceNo;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _nextSequenceNo, value);
      }
    }

    private DateTime _lastSequenceUpdate;
    [Persistent]
    public DateTime LastSequenceUpdate
    {
      get
      {
        return _lastSequenceUpdate;
      }
      set
      {
        SetPropertyValue("LastSequenceUpdate", ref _lastSequenceUpdate, value);
      }
    }

    private string _outgoingFilePath;
    [Persistent, Size(256)]
    public string OutgoingFilePath
    {
      get
      {
        return _outgoingFilePath;
      }
      set
      {
        SetPropertyValue("OutgoingFilePath", ref _outgoingFilePath, value);
      }
    }

    private string _incomingFilePath;
    [Persistent, Size(256)]
    public string IncomingFilePath
    {
      get
      {
        return _incomingFilePath;
      }
      set
      {
        SetPropertyValue("IncomingFilePath", ref _incomingFilePath, value);
      }
    }

    private string _archiveFilePath;
    [Persistent, Size(256)]
    public string ArchiveFilePath
    {
      get
      {
        return _archiveFilePath;
      }
      set
      {
        SetPropertyValue("ArchiveFilePath", ref _archiveFilePath, value);
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

    public DBT_Service() : base() { }
    public DBT_Service(Session session) : base(session) { }
  }
}