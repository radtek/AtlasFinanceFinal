using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_Service:XPLiteObject
  {
    private int _serviceId;
    [Key]
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

    private PYT_ServiceType _serviceType;
    [Persistent("ServiceTypeId")]
    public PYT_ServiceType ServiceType
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

    private string _userCode;
    [Persistent, Size(20)]
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

    private string _username;
    [Persistent, Size(20)]
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

    private string _description;
    [Persistent, Size(20)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    private string _referenceName;
    [Persistent, Size(20)]
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

    private string _branchCode;
    [Persistent, Size(8)]
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

    private string _accountNo;
    [Persistent, Size(15)]
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

    private string _swiftCode;
    [Persistent, Size(15)]
    public string SwiftCode
    {
      get
      {
        return _swiftCode;
      }
      set
      {
        SetPropertyValue("SwiftCode", ref _swiftCode, value);
      }
    }

    private string _bankServeCode;
    [Persistent, Size(10)]
    public string BankServeCode
    {
      get
      {
        return _bankServeCode;
      }
      set
      {
        SetPropertyValue("BankServeCode", ref _bankServeCode, value);
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
        SetPropertyValue("NextSequenceNo", ref _nextSequenceNo, value);
      }
    }

    private string _address1;
    [Persistent, Size(35)]
    public string Address1
    {
      get
      {
        return _address1;
      }
      set
      {
        SetPropertyValue("Address1", ref _address1, value);
      }
    }

    private string _address2;
    [Persistent, Size(35)]
    public string Address2
    {
      get
      {
        return _address2;
      }
      set
      {
        SetPropertyValue("Address2", ref _address2, value);
      }
    }

    private string _address3;
    [Persistent, Size(35)]
    public string Address3
    {
      get
      {
        return _address3;
      }
      set
      {
        SetPropertyValue("Address3", ref _address3, value);
      }
    }

    private string _address4;
    [Persistent, Size(35)]
    public string Address4
    {
      get
      {
        return _address4;
      }
      set
      {
        SetPropertyValue("Address4", ref _address4, value);
      }
    }

    private string _outgoingPath;
    [Persistent, Size(250)]
    public string OutgoingPath
    {
      get
      {
        return _outgoingPath;
      }
      set
      {
        SetPropertyValue("OutgoingPath", ref _outgoingPath, value);
      }
    }

    private string _incomingPath;
    [Persistent, Size(250)]
    public string IncomingPath
    {
      get
      {
        return _incomingPath;
      }
      set
      {
        SetPropertyValue("IncomingPath", ref _incomingPath, value);
      }
    }

    private string _archivePath;
    [Persistent, Size(250)]
    public string ArchivePath
    {
      get
      {
        return _archivePath;
      }
      set
      {
        SetPropertyValue("ArchivePath", ref _archivePath, value);
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

    public PYT_Service() : base() { }
    public PYT_Service(Session session) : base(session) { }

    #endregion
  }
}
