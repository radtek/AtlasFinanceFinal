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
  public class STR_CaseDTO
  {
    [DataMember]
    public Int64 CaseId { get; set; }
    [DataMember]
    public STR_AccountDTO Account { get; set; }
    [DataMember]
    public STR_SubCategoryDTO SubCategory { get; set; }
    [DataMember]
    public STR_GroupDTO Group { get; set; }
    [DataMember]
    public STR_CaseStatusDTO CaseStatus { get; set; }
    [DataMember]
    public DateTime LastStatusDate { get; set; }
    [DataMember]
    public DateTime OpenDate { get; set; }
    [DataMember]
    public DateTime CloseDate { get; set; }
    [DataMember]
    public decimal Balance { get; set; }
    [DataMember]
    public DateTime? LastReceiptDate { get; set; }
    [DataMember]
    public decimal? LastReceiptAmount { get; set; }
    [DataMember]
    public decimal RequiredPayment { get; set; }
    [DataMember]
    public int InstalmentsOutstanding { get; set; }
    [DataMember]
    public decimal ArrearsAmount { get; set; }
    [DataMember]
    public PER_PersonDTO AllocatedUser { get; set; }
    [DataMember]
    public int SMSCount { get; set; }
    [DataMember]
    public DateTime? CompleteDate { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}