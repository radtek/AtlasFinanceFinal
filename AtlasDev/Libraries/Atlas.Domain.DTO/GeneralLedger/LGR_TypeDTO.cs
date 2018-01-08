using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class LGR_TypeDTO
  {
    [DataMember]
    public int TypeId { get; set; }
    [DataMember]
    public Enumerators.GeneralLedger.Type Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.GeneralLedger.Type>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.GeneralLedger.Type>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
