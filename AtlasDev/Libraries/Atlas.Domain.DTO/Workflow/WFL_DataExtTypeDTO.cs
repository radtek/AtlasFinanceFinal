using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_DataExtTypeDTO
  {
    public int DataExtTypeId { get; set; }
    public Enumerators.Workflow.WorkflowDataExtType Type
    {
      get
      {
        return Namespace.FromStringToEnum<Enumerators.Workflow.WorkflowDataExtType>();
      }
      set
      {
        value = Namespace.FromStringToEnum<Enumerators.Workflow.WorkflowDataExtType>();
      }
    }
    public string Namespace { get; set; }
    public string Assembly { get; set; }
  }
}