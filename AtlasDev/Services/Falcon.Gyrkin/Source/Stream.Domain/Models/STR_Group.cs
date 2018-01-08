using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
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
    public Framework.Enumerators.Stream.GroupType GroupType
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.GroupType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.GroupType>();
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

    public STR_Group()
    { }
    public STR_Group(Session session) : base(session) { }

    #endregion

  }
}