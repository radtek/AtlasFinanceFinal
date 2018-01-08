using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class Comment : IComment
  {
    public int CommentId { get; set; }
    public ICommentGroup CommentGroup { get; set; }
    public string Description { get; set; }
    public DateTime? DisableDate { get; set; }
  }
}
