using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepEscalationGroupDTO
  {
    public int ProcessStepEscalationGroupId { get; set; }
    public WFL_ProcessStepEscalationDTO ProcessStepEscalation { get; set; }
    public WFL_EscalationGroupDTO EscalationGroup { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
