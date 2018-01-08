using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_ActionType : XPLiteObject
  {
    private int _actionTypeId;
    [Key]
    public int ActionTypeId
    {
      get
      {
        return _actionTypeId;
      }
      set
      {
        SetPropertyValue("ActionTypeId", ref _actionTypeId, value);
      }
    }

    [NonPersistent]
    public Framework.Enumerators.Action.Type Type
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Action.Type>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Action.Type>();
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

    public STR_ActionType()
    { }
    public STR_ActionType(Session session) : base(session) { }

    #endregion

  }
}