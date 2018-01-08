namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepJobEscalationNotificationDTO
  {
    public int ProcessStepJobEscalationNotificationId { get; set; }
    public WFL_ProcessStepJobEscalationDTO ProcessStepJobEscalation { get; set; }
    public NTF_HistoryDTO NotificationHistory { get; set; }
  }
}
