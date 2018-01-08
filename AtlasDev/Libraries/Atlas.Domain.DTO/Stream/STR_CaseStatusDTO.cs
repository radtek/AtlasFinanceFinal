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
  public class STR_CaseStatusDTO
  {
    [DataMember]
    public int CaseStatusId { get; set; }
    [DataMember]
    public Enumerators.Stream.CaseStatus Status { get; set; }
    [DataMember]
    public string Description{get;set;}
  }
}
