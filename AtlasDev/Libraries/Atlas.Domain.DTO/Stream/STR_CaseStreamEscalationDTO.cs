using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO.Stream
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class STR_CaseStreamEscalationDTO
  {
    [DataMember]
    public Int64 CaseStreamEscalationId { get; set; }
    [DataMember]
    public STR_CaseStreamDTO CaseStream { get; set; }
    [DataMember]
    public STR_EscalationDTO Escalation { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
