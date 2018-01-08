using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public class STR_CommentGroup : XPLiteObject
  {
    private int _commentGroupId;
    [Key(AutoGenerate = true)]
    public int CommentGroupId
    {
      get
      {
        return _commentGroupId;
      }
      set
      {
        SetPropertyValue("CommentGroupId", ref _commentGroupId, value);
      }
    }

    private STR_Group _group;
    [Persistent("GroupId")]
    [Indexed]
    public STR_Group Group
    {
      get
      {
        return _group;
      }
      set
      {
        SetPropertyValue("Group", ref _group, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.CommentGroupType CommentGroupType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.CommentGroupType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.CommentGroupType>();
      }
    }

    private string _description;
    [Persistent, Size(30)]
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


    #region Constructors

    public STR_CommentGroup() : base() { }
    public STR_CommentGroup(Session session) : base(session) { }

    #endregion

  }
}