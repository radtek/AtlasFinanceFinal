using Falcon.Common.Interfaces.Structures.Reports.General;

namespace Falcon.Common.Structures.Report.General
{
  public class RegionBranch : IRegionBranch
  {
    public long RegionId { get; set; }
    public long BranchId { get; set; }
    public string Name { get; set; }
    public bool Ticked { get; set; }
    public bool MultiSelectGroup { get; set; }
  }
}