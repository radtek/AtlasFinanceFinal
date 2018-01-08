namespace Falcon.Common.Interfaces.Structures
{
  public interface IBranch
  {
    long BranchId { get; set; }
    string LegacyBranchNum { get; set; }
    long RegionId { get; set; }
    string Region { get; set; }
    string Name { get; set; }
  }
}
