namespace Atlas.Domain.DTO
{
  public class WFL_WorkflowHostDTO
  {
    public int WorkflowHostId { get; set; }
    public WFL_WorkflowDTO Workflow { get; set; }
    public HostDTO Host { get; set; }
  }
}