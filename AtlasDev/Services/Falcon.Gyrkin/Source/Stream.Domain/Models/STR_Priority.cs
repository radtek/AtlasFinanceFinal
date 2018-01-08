using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_Priority : XPLiteObject
  {
    private int _priorityId;
    [Key]
    public int PriorityId
    {
      get
      {
        return _priorityId;
      }
      set
      {
        SetPropertyValue("PriorityId", ref _priorityId, value);
      }
    }

    [NonPersistent]
    public Framework.Enumerators.Stream.PriorityType PriorityType
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.PriorityType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.PriorityType>();
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

    private int _value;
    [Persistent]
    [Indexed]
    public int Value
    {
      get
      {
        return _value;
      }
      set
      {
        SetPropertyValue("Value", ref _value, value);
      }
    }


    #region Constructors

    public STR_Priority()
    { }
    public STR_Priority(Session session) : base(session) { }

    #endregion

  }
}