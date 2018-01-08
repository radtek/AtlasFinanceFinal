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
  public class STR_EscalationDTO
  {
    [DataMember]
    public int EscalationId { get; set; }

    [DataMember]
    public Enumerators.Stream.EscalationType EscalationType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.EscalationType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.EscalationType>();
      }
    }

    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public int Value { get; set; }
  }
}
