using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_ScheduleFrequencyDTO
  {
    public int ScheduleFrequencyId { get; set; }
    public Enumerators.Workflow.ScheduleFrequency Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.ScheduleFrequency>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.ScheduleFrequency>();
      }
    }
    public string Name { get; set; }
  }
}
