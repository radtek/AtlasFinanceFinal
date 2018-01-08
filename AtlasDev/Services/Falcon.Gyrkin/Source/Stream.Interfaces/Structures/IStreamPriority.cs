namespace Stream.Framework.Structures
{
  public interface IStreamPriority
  {
    int PriorityId { get; set; }
    Enumerators.Stream.PriorityType PriorityType { get; set; }
    string Description { get; set; }
    int Value { get; set; }
  }
}
