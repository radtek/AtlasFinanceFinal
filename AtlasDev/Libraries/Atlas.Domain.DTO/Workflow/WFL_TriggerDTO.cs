using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_TriggerDTO
  {
    public int TriggerId { get; set; }
    public Enumerators.Workflow.Trigger Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.Trigger>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.Trigger>();
      }
    }
    public string Name { get; set; }
  }
}
