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
  public class STR_CaseStreamActionDTO
  {
    [DataMember]
    public Int64 CaseStreamActionId { get; set; }
    [DataMember]
    public STR_CaseStreamDTO CaseStream { get; set; }
    [DataMember]
    public DateTime ActionDate { get; set; }
    [DataMember]
    public STR_ActionTypeDTO ActionType { get; set; }
    [DataMember]
    public DateTime? DateActioned { get; set; }
    [DataMember]
    public DateTime? CompleteDate { get; set; }
    [DataMember]
    public bool? IsSuccess { get; set; }
    [DataMember]
    public decimal? Amount { get; set; }
  }
}