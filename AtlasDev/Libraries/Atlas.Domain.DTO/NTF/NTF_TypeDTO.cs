using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class NTF_TypeDTO
  {
    [DataMember]
    public int TypeId { get; set; }
    [DataMember]
    public Enumerators.Notification.NotificationType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
