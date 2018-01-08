using System;
using DevExpress.Xpo;
using Atlas.Domain.Security;


namespace Atlas.Domain.Model.Biometric
{
  public class BIO_LogHardware : XPLiteObject
  {
    [Persistent, Key(AutoGenerate = true)]
    public Int64 LogRequestId { get; set; }

    private COR_Machine _machine;
    [Persistent, Indexed]
    public COR_Machine Machine
    {
      get { return _machine; }
      set { SetPropertyValue<COR_Machine>("MachineId", ref _machine, value); }
    }

    private DateTime _firstLog;
    [Persistent, Indexed]
    public DateTime FirstLog
    {
      get { return _firstLog; }
      set { SetPropertyValue<DateTime>("FirstLog", ref _firstLog, value); }
    }
    
    private string _serial;
    [Persistent, Indexed, Size(20)]
    public string Serial
    {
      get { return _serial; }
      set { SetPropertyValue<string>("Serial", ref _serial, value); }
    }


    #region Constructors

    public BIO_LogHardware(Session session) : base(session) { }

    public BIO_LogHardware() : base() { }

    #endregion
  }
}
