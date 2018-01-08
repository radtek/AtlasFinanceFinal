using Atlas.Common.Extensions;
using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NTF_TemplateTypeDTO
  {
    [DataMember]
    public int TemplateTypeId { get; set; }
    [DataMember]
    public Enumerators.Notification.NotificationTemplate Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationTemplate>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationTemplate>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}