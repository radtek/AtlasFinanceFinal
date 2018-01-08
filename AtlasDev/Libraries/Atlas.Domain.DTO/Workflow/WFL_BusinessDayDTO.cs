using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_BusinessDayDTO
  {
    public int DayId { get; set; }
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
    public string Name { get; set; }
  }
}
