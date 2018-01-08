using System;

namespace Stream.Framework.Structures
{
  public interface IComment
  {
    int CommentId { get; set; }
    ICommentGroup CommentGroup { get; set; }
    string Description { get; set; }
    DateTime? DisableDate { get; set; }
  }
}
