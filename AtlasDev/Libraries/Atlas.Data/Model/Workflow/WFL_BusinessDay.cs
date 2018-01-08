using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class WFL_BusinessDay : XPLiteObject
  {
    private int _dayId;
    [Key]
    public int DayId
    {
      get
      {
        return _dayId;
      }
      set
      {
        SetPropertyValue("DayId", ref _dayId, value);
      }
    }
    
    [NonPersistent]
    public Enumerators.General.Days Day
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.General.Days>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.General.Days>();
      }
    } 

    private string _name;
    [Persistent, Size(10)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
      }
    }

    #region Constructors

    public WFL_BusinessDay() : base() { }
    public WFL_BusinessDay(Session session) : base(session) { }

    #endregion

  }
}
