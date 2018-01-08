using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures
{
  public class Region : IRegion
  {
    public long RegionId { get; set; }
    public string Description { get; set; }
  }
}
