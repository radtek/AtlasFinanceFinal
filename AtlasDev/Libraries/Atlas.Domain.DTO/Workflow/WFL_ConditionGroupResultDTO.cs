namespace Atlas.Domain.DTO
{
  public class WFL_ConditionGroupResultDTO
  {
    public long ConditionGroupResultId { get; set; }
    public WFL_ConditionGroupDTO ConditionGroup { get; set; }
    public bool Result { get; set; }
    public WFL_ConditionGroupDTO NextConditionGroup { get; set; }
    public WFL_ProcessStepDTO OutcomeProcessStep { get; set; }
  }
}