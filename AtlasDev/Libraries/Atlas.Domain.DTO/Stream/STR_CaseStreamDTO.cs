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
  public class STR_CaseStreamDTO
  {
    [DataMember]
    public Int64 CaseStreamId { get; set; }
    [DataMember]
    public STR_CaseDTO Case { get; set; }
    [DataMember]
    public STR_EscalationDTO Escalation { get; set; }
    [DataMember]
    public STR_StreamDTO Stream { get; set; }
    [DataMember]
    public STR_PriorityDTO Priority { get; set; }
    [DataMember]
    public DateTime LastPriorityDate { get; set; }
    [DataMember]
    public PER_PersonDTO CompletedUser { get; set; }
    [DataMember]
    public STR_CommentDTO CompleteComment { get; set; }
    [DataMember]
    public DateTime? CompleteDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}