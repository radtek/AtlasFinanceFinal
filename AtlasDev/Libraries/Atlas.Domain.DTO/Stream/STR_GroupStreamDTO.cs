using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO
{
  public class STR_GroupStreamDTO
  {
    [DataMember]
    public int GroupStreamId { get; set; }
    [DataMember]
    public STR_GroupDTO Group { get; set; }
    [DataMember]
    public STR_StreamDTO Stream { get; set; }
    [DataMember]
    public int Ordinal { get; set; }
  }
}