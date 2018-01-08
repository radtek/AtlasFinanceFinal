using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_EscalationLevelDTO
  {
    public int EscalationLevelId { get; set; }
    public Enumerators.Workflow.EscalationLevel Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.EscalationLevel>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.EscalationLevel>();
      }
    }
    public string Name { get; set; }
  }
}
