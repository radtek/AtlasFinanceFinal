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
  public class STR_PriorityDTO
  {
    [DataMember]
    public int PriorityId { get; set; }
    [DataMember]
    public Enumerators.Stream.PriorityType PriorityType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.PriorityType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.PriorityType>();
      }
    }

    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public int Value { get; set; }
  }
}