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
  public class STR_ActuatorTypeDTO
  {
    [DataMember]
    public int ActionTypeId { get; set; }
    [DataMember]
    public Enumerators.Stream.ActuatorType Type { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
