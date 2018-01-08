using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class HostDTO
  {
    [DataMember]
    public int HostId { get; set; }
    [DataMember]
    public Enumerators.General.Host Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.General.Host>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.General.Host>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
