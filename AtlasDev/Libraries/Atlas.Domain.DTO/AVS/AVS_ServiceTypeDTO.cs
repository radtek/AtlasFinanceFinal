using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AVS_ServiceTypeDTO
  {
    [DataMember]
    public Int32 ServiceTypeId { get; set; }
    [DataMember]
    public Enumerators.AVS.ServiceType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.ServiceType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.ServiceType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
