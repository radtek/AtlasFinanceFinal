using Falcon.Common.Interfaces.Structures.AVS;

namespace Falcon.Common.Structures.Avs
{
  public class AvsService : IAvsService
  {
    public int ServiceId { get; set; }
    public string Description { get; set; }
    public int NextGenerationNo { get; set; }
    public int NextTransmissionNo { get; set; }
    public bool SaveSequenceNo { get; set; }
  }
}
