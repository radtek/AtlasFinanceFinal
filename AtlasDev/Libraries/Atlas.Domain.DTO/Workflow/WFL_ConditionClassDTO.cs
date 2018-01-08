using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_ConditionClassDTO
  {
    public long ConditionClassId { get; set; }
    public string Description { get; set; }
    public Enumerators.Workflow.ConditionClass Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Workflow.ConditionClass>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Workflow.ConditionClass>();
      }
    }
    public string Assembly { get; set; }
    public string Namespace { get; set; }
  }
}
