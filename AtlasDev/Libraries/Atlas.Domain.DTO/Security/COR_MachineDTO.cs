using System;


namespace Atlas.Domain.DTO
{
  [Serializable]
  public class COR_MachineDTO 
  {
    public Int64 MachineId { get; set; }
    public string MachineIPAddresses  { get; set; }
    public string MachineName { get; set; }
    public DateTime LastAccessDT { get; set; }
    public BRN_BranchDTO LastBranchCode { get; set; }
    public string HardwareKey { get; set; }
  }
}
