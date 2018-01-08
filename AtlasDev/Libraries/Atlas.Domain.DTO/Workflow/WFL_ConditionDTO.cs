namespace Atlas.Domain.DTO
{
  public class WFL_ConditionDTO
  {
    public long ConditionId { get; set; }
    public WFL_ConditionGroupDTO ConditionGroup { get; set; }
    public WFL_ConditionClassPropertyDTO ConditionClassProperty { get; set; }
    public string ConditionValue { get; set; }
  }
}
