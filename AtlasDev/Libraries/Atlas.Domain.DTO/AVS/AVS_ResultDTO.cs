using System.Runtime.Serialization;
using Atlas.Common.Extensions;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class AVS_ResultDTO
  {
    [DataMember]
    public Int32 ResultId { get; set; }
    [DataMember]

    public Enumerators.AVS.Result Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.Result>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.Result>();
      }
    }
    [DataMember]

    public string Description { get; set; }
    [DataMember]

    public bool IsSuccess { get; set; }
    [DataMember]

    public bool HasWarnings { get; set; }
  } 
}
