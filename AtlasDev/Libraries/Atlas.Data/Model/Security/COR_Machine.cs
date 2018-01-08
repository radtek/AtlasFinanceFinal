/*
 *  Stores core information about a machine- uses machine fingerprint/IP address to locate
 * 
 * */

using System;

using DevExpress.Xpo;

using Atlas.Domain.Model;


namespace Atlas.Domain.Security
{
  public class COR_Machine : XPCustomObject
  {   
    [Key(AutoGenerate = true)]
    public Int64 MachineId { get; set; }
 
    private string _MachineIPAddresses;
    [Persistent, Size(2000), Indexed]
    public string MachineIPAddresses
    {
      get { return _MachineIPAddresses; }      
      set { SetPropertyValue(nameof(MachineIPAddresses), ref _MachineIPAddresses, value); }
    }

    private string _MachineName;
    [Persistent, Size(24), Indexed]
    public string MachineName
    {
      get { return _MachineName; }
      set { SetPropertyValue(nameof(MachineName), ref _MachineName, value); }
    }

    private DateTime _LastAccessDT;
    [Persistent]
    public DateTime LastAccessDT
    {
      get { return _LastAccessDT; }
      set { SetPropertyValue(nameof(LastAccessDT), ref _LastAccessDT, value); }
    }

    private BRN_Branch _LastBranchCode;
    [Persistent("LastBranchId")]
    public BRN_Branch LastBranchCode
    {
      get { return _LastBranchCode; }
      set { SetPropertyValue(nameof(LastBranchCode), ref _LastBranchCode, value); }
    }

    private string _HardwareKey;
    [Persistent, Size(256), Indexed]
    public string HardwareKey
    {
      get { return _HardwareKey; }
      set { SetPropertyValue(nameof(HardwareKey), ref _HardwareKey, value); }
    }


    #region Constructors

    public COR_Machine(Session session) : base(session) { }
    public COR_Machine() : base() { }

    #endregion
  }
}
