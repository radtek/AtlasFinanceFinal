using System;

namespace Atlas.Domain.DTO
{
  public class WFL_UserGroupDTO
  {
    public int UserGroupId { get; set; }
    public string Name { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
