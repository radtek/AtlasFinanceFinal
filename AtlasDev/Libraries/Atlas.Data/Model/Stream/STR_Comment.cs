using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_Comment : XPLiteObject
  {
    private int _commentId;
    [Key]
    public int CommentId
    {
      get
      {
        return _commentId;
      }
      set
      {
        SetPropertyValue("CommentId", ref _commentId, value);
      }
    }

    private STR_CommentGroup _commentGroup;
    [Persistent("CommentGroupId")]
    [Indexed]
    public STR_CommentGroup CommentGroup
    {
      get
      {
        return _commentGroup;
      }
      set
      {
        SetPropertyValue("CommentGroup", ref _commentGroup, value);
      }
    }

    private string _description;
    [Persistent, Size(300)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    private DateTime? _disableDate;
    [Persistent]
    public DateTime? DisableDate
    {
      get
      {
        return _disableDate;
      }
      set
      {
        SetPropertyValue("DisableDate", ref _disableDate, value);
      }
    }

    #region Constructors

    public STR_Comment() : base() { }
    public STR_Comment(Session session) : base(session) { }

    #endregion
  }
}