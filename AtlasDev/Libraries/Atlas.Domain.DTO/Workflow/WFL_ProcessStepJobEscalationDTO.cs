using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepJobEscalationDTO
  {
    public int ProcessStepJobEscalationId{get;set;}
    public WFL_ProcessStepJobDTO ProcessStepJob{get;set;}
    public WFL_ProcessStepEscalationDTO ProcessStepEscalation{get;set;}
    public WFL_EscalationLevelDTO EscalationLevel{get;set;}
    public DateTime CreateDate { get; set; }
  }
}
