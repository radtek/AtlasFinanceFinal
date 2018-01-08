using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class BUR_BandDTO
  {
    [DataMember]
    public int BandId { get; set; }
    [DataMember]
    public Enumerators.Risk.Band Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Risk.Band>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Risk.Band>();
      }
    }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public bool Pass { get; set; }
  }
}
