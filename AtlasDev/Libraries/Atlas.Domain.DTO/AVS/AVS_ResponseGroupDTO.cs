using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AVS_ResponseGroupDTO
  {
    [DataMember]
    public Int32 ResponseGroupId { get; set; }
    [DataMember]

    public Enumerators.AVS.ResponseGroup Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.ResponseGroup>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.ResponseGroup>();
      }
    }
    [DataMember]

    public string Description { get; set; }
  }
} 
