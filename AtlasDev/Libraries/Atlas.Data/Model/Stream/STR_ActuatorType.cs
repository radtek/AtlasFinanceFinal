using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
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
    public Enumerators.Stream.ActuatorType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.ActuatorType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.ActuatorType>();
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

    public STR_ActuatorType() : base() { }
    public STR_ActuatorType(Session session) : base(session) { }

    #endregion

  }
}