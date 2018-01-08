using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class CommentGroup : ICommentGroup
  {
    public int CommentGroupId { get; set; }
    public IGroup Group { get; set; }
    public Framework.Enumerators.Stream.CommentGroupType CommentGroupType { get; set; }
    public string Description { get; set; }
  }
}