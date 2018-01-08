using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ScheduleProcessDTO
  {
    public int ScheduleProcessId { get; set; }
    public WFL_ProcessDTO Process { get; set; }
    public WFL_ScheduleFrequencyDTO ScheduleFrequency { get; set; }
    public int Iteration { get; set; }
    public int CurrentIteration { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public WFL_ScheduleProcessStatusDTO ScheduleProcessStatus { get; set; }
    public DateTime? LastExecutionDate { get; set; }
    public DateTime NextExecutionDate { get; set; }
    public string LastResultMessage { get; set; }
    public bool Enabled { get; set; }
  }
}