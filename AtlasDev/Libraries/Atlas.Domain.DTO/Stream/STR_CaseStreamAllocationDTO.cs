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
  public class STR_CaseStreamAllocationDTO
  {
    [DataMember]
    public Int64 CaseStreamAllocationId { get; set; }
    [DataMember]
    public STR_CaseStreamDTO CaseStream { get; set; }
    [DataMember]
    public STR_EscalationDTO Escalation { get; set; }
    [DataMember]
    public PER_PersonDTO AllocatedUser { get; set; }
    [DataMember]
    public DateTime AllocatedDate { get; set; }
    [DataMember]
    public int NoActionCount { get; set; }
    [DataMember]
    public bool TransferredIn { get; set; }
    [DataMember]
    public DateTime? TransferredOutDate { get; set; }
    [DataMember]
    public bool TransferredOut { get; set; }
    [DataMember]
    public int SMSCount { get; set; }
    [DataMember]
    public DateTime? CompleteDate { get; set; }
    [DataMember]
    public STR_CommentDTO CompleteComment { get; set; }
  }
}
