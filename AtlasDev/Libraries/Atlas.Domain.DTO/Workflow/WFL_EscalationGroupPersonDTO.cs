using System;

namespace Atlas.Domain.DTO
{
  public class WFL_EscalationGroupPersonDTO
  {
    public int EscalationGroupPersonId{get;set;}
    public WFL_EscalationGroupDTO EscalationGroup{get;set;}
    public PER_PersonDTO Person{get;set;}
    public NTF_TypeDTO NotificationType{get;set;}
    public bool Enabled{get;set;}
    public DateTime? DisableDate{get;set;}
    public DateTime CreateDate { get; set; }
  }
}
