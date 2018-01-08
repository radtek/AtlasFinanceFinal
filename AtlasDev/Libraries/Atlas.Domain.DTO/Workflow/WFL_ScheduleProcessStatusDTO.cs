using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_ScheduleProcessStatusDTO
  {
    public int ScheduleProcessStatusId { get; set; }
    public Enumerators.Workflow.ScheduleProcessStatus Status
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.ScheduleProcessStatus>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.ScheduleProcessStatus>();
      }
    }
    public string Name { get; set; }
  }
}
