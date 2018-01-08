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
  public class STR_CategoryHostDTO
  {
    [DataMember]
    public Int64 CategoryHostId { get; set; }
    [DataMember]
    public STR_CategoryDTO Category { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public DateTime? DisableDate { get; set; }
  }
}