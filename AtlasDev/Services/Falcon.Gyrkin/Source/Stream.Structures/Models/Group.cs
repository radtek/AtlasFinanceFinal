using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class Group : IGroup
  {
    public int GroupId { get; set; }
    public Framework.Enumerators.Stream.GroupType GroupType { get; set; }
    public string Description { get; set; }
  }
}