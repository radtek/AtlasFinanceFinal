using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.Branch
{
  public class Branch : IBranch
  {
    public long BranchId { get; set; }
    public long RegionId { get; set; }
    public string LegacyBranchNum { get; set; }
    public string Region { get; set; }
    public string Name { get; set; }
  }
}
