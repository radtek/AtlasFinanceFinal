using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepEscalationDTO
  {
    public int ProcessStepEscalationId { get; set; }
    public WFL_ProcessStepDTO ProcessStep { get; set; }
    public WFL_EscalationLevelDTO EscalationLevel { get; set; }
    public WFL_EscalationTemplateDTO EscalationTemplate { get; set; }
    public int EscalationTime { get; set; }
    public WFL_PeriodFrequencyDTO EscalationTimePeriodFrequency { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}