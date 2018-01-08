using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepUserGroupDTO
  {
    public int ProcessStepUserGroupId { get; set; }
    public WFL_ProcessStepDTO ProcessStep { get; set; }
    public WFL_UserGroupDTO UserGroup { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}