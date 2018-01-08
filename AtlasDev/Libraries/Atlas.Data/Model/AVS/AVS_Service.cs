using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_Service:XPLiteObject
  {
    private Int32 _serviceId;
    [Key]
    public Int32 ServiceId
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

    [NonPersistent]
    public Enumerators.AVS.Service Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.Service>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.Service>();
      }
    }

    private string _description;
    [Persistent, Size(30)]
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

    private string _userCode;
    [Persistent, Size(10)]
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
    [Persistent, Size(35)]
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

    private string _bankServCode;
    [Persistent, Size(10)]
    public string BankServCode
    {
      get
      {
        return _bankServCode;
      }
      set
      {
        SetPropertyValue("BankServCode", ref _bankServCode, value);
      }
    }

    private bool _awaitReply;
    [Persistent]
    public bool AwaitReply
    {
      get
      {
        return _awaitReply;
      }
      set
      {
        SetPropertyValue("AwaitReply", ref _awaitReply, value);
      }
    }

    private bool _enabled;
    [Persistent]
    [Indexed]
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

    private int _sendInterval;
    [Persistent]
    public int SendInterval
    {
      get
      {
        return _sendInterval;
      }
      set
      {
        SetPropertyValue("SendInterval", ref _sendInterval, value);
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

    private char _environmentFlag;
    [Persistent]
    public char EnvironmentFlag
    {
      get
      {
        return _environmentFlag;
      }
      set
      {
        SetPropertyValue("EnvironmentFlag", ref _environmentFlag, value);
      }
    }

    #region Constructors

    public AVS_Service() : base() { }
    public AVS_Service(Session session) : base(session) { }

    #endregion
  }
}
