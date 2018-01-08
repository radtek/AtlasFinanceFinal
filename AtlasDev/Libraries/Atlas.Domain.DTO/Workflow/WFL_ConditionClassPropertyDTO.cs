namespace Atlas.Domain.DTO
{
  public class WFL_ConditionClassPropertyDTO
  {
    public long ConditionClassPropertyId { get; set; }
    public WFL_ConditionClassDTO ConditionClass { get; set; }
    public string Property { get; set; }
    public string Description { get; set; }
  }
}