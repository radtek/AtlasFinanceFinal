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
  public class STR_FrequencyDTO
  {
    [DataMember]
    public Int32 FrequencyId { get; set; }
    [DataMember]
    public Enumerators.Stream.Frequency Type { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
