namespace Atlas.Domain.DTO
{
  public class WFL_DecisionDTO
  {
    public int DecisionId { get; set; }
    public WFL_ProcessDTO Process { get; set; }
    public WFL_ProcessStepDTO ProcessStep { get; set; }
    public WFL_ConditionGroupDTO ConditionGroup { get; set; }
    public int Rank { get; set; }
  }
}