using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AVS_StatusDTO
  {
    [DataMember]
    public Int32 StatusId { get; set; }
    [DataMember]
    public Enumerators.AVS.Status Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.Status>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.Status>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}