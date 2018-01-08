using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepDTO
  {
    public int ProcessStepId { get; set; }
    public WFL_ProcessDTO Process { get; set; }
    public WFL_TriggerDTO Trigger { get; set; }
    public string Name { get; set; }
    public string Namespace { get; set; }
    public bool Locked { get; set; }
    // public bool Jumpable { get; set; }
    public WFL_PeriodFrequencyDTO ThresholdPeriodFrequency { get; set; }
    public int Threshold { get; set; }
    public bool CanParallelRun { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public int Rank { get; set; }
  }
}