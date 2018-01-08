using System;
using Priority = Atlas.Enumerators.Notification;


namespace Atlas.RabbitMQ.Messages.Notification
{
  public enum Provider
  {
    EUROCOM = 0,
    BLUELABEL = 1,
    SMSPORTAL = 2
  }

  #region IEurocomSMSMessageBase

  public interface IEurocomSMSMessageBase
  {
    Guid CorrelationId { get; set; }
    string CellNo { get; set; }
    string Message { get; set; }
    int? CampaignId { get; set; }
    long? NotificationId { get; set; }
  }

  #endregion

  #region Eurocom SMS Request Message

  [Serializable]
  public class EurocomSMSRequestMessage : IEurocomSMSMessageBase
  {
    public Guid CorrelationId { get; set; }
    public string CellNo { get; set; }
    public string Message { get; set; }
    public int? CampaignId { get; set; }
    public long? NotificationId { get; set; }
  }

  #endregion

  #region Eurocom SMS Start Request Message

  [Serializable]
  public class EurocomSMSStartRequestMessage : IEurocomSMSMessageBase
  {
    public Guid CorrelationId { get; set; }
    public string CellNo { get; set; }
    public string Message { get; set; }
    public int? CampaignId { get; set; }
    public long? NotificationId { get; set; }
  }


  #endregion

  #region Eurocom SMS Completed Request Message

  [Serializable]
  public class EurocomSMSCompletedRequestMessage : IEurocomSMSMessageBase
  {
    public Guid CorrelationId { get; set; }
    public string CellNo { get; set; }
    public string Message { get; set; }
    public int? CampaignId { get; set; }
    public long? NotificationId { get; set; }
  }


  #endregion

  [Serializable]
  public class SMSNotifyMessage
  {
    public SMSNotifyMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Provider Provider { get; set; }
    public string To { get; set; }
    public string Body { get; set; }
    public Priority.NotificationPriority Priority { get; set; }
    public DateTime ActionDate { get; set; }
  }

  public class SMSNotifyUpdateWithStatus
  {
    public SMSNotifyUpdateWithStatus(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }

    public long NotificationId { get; set; }

    public Enumerators.Notification.NotificationStatus Status { get; set; }
    
  }
}
