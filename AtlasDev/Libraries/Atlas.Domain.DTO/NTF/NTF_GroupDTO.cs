using Atlas.Common.Extensions;
using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class NTF_GroupDTO
  {
    [DataMember]
    public int GroupId { get; set; }
    [DataMember]
    public Enumerators.Notification.Group Group
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.Group>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.Group>();
      }
    }

    [DataMember]
    public string Description { get; set; }
  }
}