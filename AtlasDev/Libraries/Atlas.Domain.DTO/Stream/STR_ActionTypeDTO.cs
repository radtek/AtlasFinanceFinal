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
  public class STR_ActionTypeDTO
  {
    [DataMember]
    public int ActionTypeId { get; set; }
    [DataMember]
    public Enumerators.Stream.ActionType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.ActionType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.ActionType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}