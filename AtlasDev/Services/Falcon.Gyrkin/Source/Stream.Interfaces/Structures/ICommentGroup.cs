namespace Stream.Framework.Structures
{
  public interface ICommentGroup
  {
    int CommentGroupId { get; set; }
    IGroup Group { get; set; }
    Enumerators.Stream.CommentGroupType CommentGroupType { get; set; }
    string Description { get; set; }
  }
}
