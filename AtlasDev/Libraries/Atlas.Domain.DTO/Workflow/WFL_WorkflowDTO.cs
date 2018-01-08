using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_WorkflowDTO
  {
    public int WorkflowId { get; set; }
    public Enumerators.Workflow.WorkflowProcess Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.WorkflowProcess>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.WorkflowProcess>();
      }
    }

    public string Name { get; set; }
  }
}
