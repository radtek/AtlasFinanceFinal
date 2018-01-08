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
  public class STR_ActuatorDTO
  {
    [DataMember]
    public Int32 ActuatorId { get; set; }
    [DataMember]
    public BRN_BranchDTO Branch { get; set; }
    [DataMember]
    public RegionDTO Region { get; set; }
    [DataMember]
    public DateTime RangeStart { get; set; }
    [DataMember]
    public DateTime RangeEnd { get; set; }
    [DataMember]
    public bool IsActive { get; set; }
    [DataMember]
    public DateTime? DisableDate { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
