using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_ActuatorType : XPLiteObject
  {
    private int _actuatorTypeId;
    [Key]
    public int ActuatorTypeId
    {
      get
      {
        return _actuatorTypeId;
      }
      set
      {
        SetPropertyValue("ActuatorTypeId", ref _actuatorTypeId, value);
      }
    }

    [NonPersistent]
    public Framework.Enumerators.Stream.ActuatorType Type
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.ActuatorType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.ActuatorType>();
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

    public STR_ActuatorType()
    { }
    public STR_ActuatorType(Session session) : base(session) { }

    #endregion

  }
}