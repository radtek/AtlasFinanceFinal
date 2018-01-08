using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class NTF_StatusDTO
  {
    [DataMember]
    public int StatusId { get; set; }
    [DataMember]
    public Enumerators.Notification.NotificationStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationStatus>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
