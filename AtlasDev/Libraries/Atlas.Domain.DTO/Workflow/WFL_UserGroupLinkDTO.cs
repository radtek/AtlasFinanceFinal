using System;

namespace Atlas.Domain.DTO
{
  public class WFL_UserGroupLinkDTO
  {
    public int UserGroupLinkId { get; set; }
    public WFL_UserGroupDTO UserGroup { get; set; }
    public PER_PersonDTO User { get; set; }
    public bool Enabled { get; set; }
    public DateTime? DisableDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}
