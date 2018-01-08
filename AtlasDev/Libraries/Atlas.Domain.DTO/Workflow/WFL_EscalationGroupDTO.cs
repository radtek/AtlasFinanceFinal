using System;

namespace Atlas.Domain.DTO
{
  public class WFL_EscalationGroupDTO
  {
    public int EscalationGroupId { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
