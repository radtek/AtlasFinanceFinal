using System.Runtime.Serialization;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NTF_HistoryDTO
  {
    [DataMember]
    public int NotificationId { get; set; }
    [DataMember]
    public NTF_TypeDTO Type { get; set; }
    [DataMember]
    public string From { get; set; }
    [DataMember]
    public string To { get; set; }
    [DataMember]
    public string Subject { get; set; }
    [DataMember]
    public string Body { get; set; }
    [DataMember]
    public bool IsHTML { get; set; }
    [DataMember]
    public NTF_PriorityDTO Priority { get; set; }
    [DataMember]
    public NTF_StatusDTO Status { get; set; }
    [DataMember]
    public DateTime StatusDate { get; set; }
    [DataMember]
    public int RetryCount { get; set; }
    [DataMember]
    public DateTime ActionDate { get; set; }
    [DataMember]
    public Guid? NotificationReference { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}