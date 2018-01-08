namespace Atlas.Domain.DTO
{
  public class WFL_ConditionPrimaryKeyDTO
  {
    public int ConditionPrimaryKeyId { get; set; }
    public WFL_ConditionClassDTO ConditionClass { get; set; }
    public WFL_DataExtTypeDTO PrimaryKeyDataExtType { get; set; }
    public string PrimaryKeyProcessDataProperty { get; set; }
  }
}
