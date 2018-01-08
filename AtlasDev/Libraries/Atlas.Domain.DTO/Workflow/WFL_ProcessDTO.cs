using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessDTO
  {
    public int ProcessId { get; set; }
    public WFL_WorkflowDTO Workflow { get; set; }
    public string Name { get; set; }
    public string Assembly { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public int Rank { get; set; }
  }
}
