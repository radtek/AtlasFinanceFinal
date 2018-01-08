using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
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
    public Enumerators.Stream.ActionType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.ActionType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.ActionType>();
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

    public STR_ActionType() : base() { }
    public STR_ActionType(Session session) : base(session) { }

    #endregion

  }
}