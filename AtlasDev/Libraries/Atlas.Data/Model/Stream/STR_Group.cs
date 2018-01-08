using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public class STR_Group : XPLiteObject
  {
    private int _groupId;
    [Key]
    public int GroupId
    {
      get
      {
        return _groupId;
      }
      set
      {
        SetPropertyValue("GroupId", ref _groupId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.GroupType GroupType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.GroupType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.GroupType>();
      }
    }

    private string _description;
    [Persistent, Size(40)]
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

    public STR_Group() : base() { }
    public STR_Group(Session session) : base(session) { }

    #endregion

  }
}