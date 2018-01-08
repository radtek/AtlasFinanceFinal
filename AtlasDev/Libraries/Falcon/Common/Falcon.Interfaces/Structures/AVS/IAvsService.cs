namespace Falcon.Common.Interfaces.Structures.AVS
{
  public interface IAvsService
  {
    int ServiceId { get; set; }
    string Description { get; set; }
    int NextGenerationNo { get; set; }
    int NextTransmissionNo { get; set; }
    bool SaveSequenceNo { get; set; }
  }
}