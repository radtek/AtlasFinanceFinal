namespace Falcon.Common.Interfaces.Structures.Reports.General
{
  public interface IRegionBranch
  {
    long RegionId { get; set; }
    long BranchId { get; set; }
    string Name { get; set; }
    bool Ticked { get; set; }
    bool MultiSelectGroup { get; set; }
  }
}