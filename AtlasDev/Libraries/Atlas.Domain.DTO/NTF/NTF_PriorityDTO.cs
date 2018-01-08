using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public sealed class NTF_PriorityDTO
  {
    [DataMember]
    public int PriorityId { get; set; }
    [DataMember]
    public Enumerators.Notification.NotificationPriority Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationPriority>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationPriority>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
