using Atlas.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class STR_StreamDTO
  {
    [DataMember]
    public int StreamId { get; set; }
    [DataMember]
    public Enumerators.Stream.StreamType StreamType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.StreamType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.StreamType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public STR_PriorityDTO Priority { get; set; }
    [DataMember]
    public STR_PriorityDTO DefaultCaseStreamPriority { get; set; }
  }
}