namespace Atlas.Domain.DTO
{
  public class WFL_ScheduleProcessStepDTO
  {
    public int ScheduleProcessStepId { get; set; }
    public WFL_ScheduleProcessDTO ScheduleProcess { get; set; }
    public WFL_ProcessStepDTO ProcessStep { get; set; }
    public WFL_PeriodFrequencyDTO DelayPeriodFrequency { get; set; }
    public int Delay { get; set; }
    public bool UseDecisionGate { get; set; }
    public int Rank { get; set; }
  }
}
