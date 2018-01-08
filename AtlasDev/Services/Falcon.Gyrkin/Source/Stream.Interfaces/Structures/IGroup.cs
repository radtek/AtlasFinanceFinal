namespace Stream.Framework.Structures
{
  public interface IGroup
  {
    int GroupId { get; set; }
    Enumerators.Stream.GroupType GroupType { get; set; }
    string Description { get; set; }
  }
}