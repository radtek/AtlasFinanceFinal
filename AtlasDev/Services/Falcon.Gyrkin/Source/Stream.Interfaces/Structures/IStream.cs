namespace Stream.Framework.Structures
{
  public interface IStream
  {
    int StreamId { get; set; }
    Enumerators.Stream.StreamType StreamType { get; set; }
    string Description { get; set; }
    Enumerators.Stream.PriorityType Priority { get; set; }
    Enumerators.Stream.PriorityType DefaultCaseStreamPriority { get; set; }
  }
}