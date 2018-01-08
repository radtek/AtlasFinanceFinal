using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class Stream : IStream
  {
    public int StreamId { get; set; }
    public Framework.Enumerators.Stream.StreamType StreamType { get; set; }
    public string Description { get; set; }
    public Framework.Enumerators.Stream.PriorityType Priority { get; set; }
    public Framework.Enumerators.Stream.PriorityType DefaultCaseStreamPriority { get; set; }
  }
}