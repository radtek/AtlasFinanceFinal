using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_Escalation : XPLiteObject
  {
    private int _escalationId;
    [Key]
    public int EscalationId
    {
      get
      {
        return _escalationId;
      }
      set
      {
        SetPropertyValue("EscalationId", ref _escalationId, value);
      }
    }

    [NonPersistent]
    public Framework.Enumerators.Stream.EscalationType EscalationType
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.EscalationType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.EscalationType>();
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

    public STR_Escalation()
    { }
    public STR_Escalation(Session session) : base(session) { }

    #endregion

  }
}
