/*
 * 
 * 
 * 
 *   Hardware/software audit for a machine
 *   
 * 
 * */

using System;

using DevExpress.Xpo;

using Atlas.Domain.Model;


namespace Atlas.Domain.Security
{
  public class COR_LogMachineInfo : XPCustomObject
  { 
    [Persistent, Key(AutoGenerate = true)]
    public Int64 LogMachineInfoId { get; set; }

    private DateTime _CreatedDT;
    [Persistent, Indexed]
    public DateTime CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue(nameof(CreatedDT), ref _CreatedDT, value); }
    }

    private COR_Machine _Machine;
    [Persistent("MachineId"), Indexed]
    public COR_Machine Machine
    {
      get { return _Machine; }
      set { SetPropertyValue(nameof(Machine), ref _Machine, value); }
    }
    
    private string _AuditJson;
    [Persistent("AuditJson"), Size(int.MaxValue)]
    public string AuditJson
    {
      get { return _AuditJson; }
      set { SetPropertyValue(nameof(AuditJson), ref _AuditJson, value); }
    }

    private int _FPDeviceCount;
    [Persistent("FPDeviceCount")]
    public int FPDeviceCount
    {
      get { return _FPDeviceCount; }
      set { SetPropertyValue(nameof(FPDeviceCount), ref _FPDeviceCount, value); }
    }

    
    #region public constructors
          
    public COR_LogMachineInfo(Session session) : base(session) { }
    public COR_LogMachineInfo() : base() { }

    #endregion

  }
}

