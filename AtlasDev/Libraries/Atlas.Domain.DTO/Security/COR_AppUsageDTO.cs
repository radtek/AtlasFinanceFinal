using System;


namespace Atlas.Domain.DTO
{
  [Serializable]
  public class COR_AppUsageDTO
  {
    public Int64 AppUsageId { get; set; }
    public COR_MachineDTO Machine { get; set; }
    public COR_SoftwareDTO Application { get; set; }
    public PER_SecurityDTO User { get; set; }
    public BRN_BranchDTO BranchCode { get; set; }
  }
}
