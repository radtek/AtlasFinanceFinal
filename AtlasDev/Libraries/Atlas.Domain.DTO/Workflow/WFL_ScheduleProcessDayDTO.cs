namespace Atlas.Domain.DTO
{
  public class WFL_ScheduleProcessDayDTO
  {
    public int ScheduleProcessDayId { get; set; }
    public WFL_ScheduleProcessDTO ScheduleProcess { get; set; }
    public WFL_BusinessDayDTO Day { get; set; }
  }
}
