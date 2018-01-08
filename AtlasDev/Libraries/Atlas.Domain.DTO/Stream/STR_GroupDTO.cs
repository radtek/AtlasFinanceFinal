using Atlas.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO
{
  public class STR_GroupDTO
  {
    [DataMember]
    public int GroupId { get; set; }
    [DataMember]
    public Enumerators.Stream.GroupType GroupType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.GroupType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.GroupType>();
      }
    }
    [DataMember]
    public string Description { get; set; }
  }
}