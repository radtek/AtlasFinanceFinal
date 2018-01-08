using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_ScoreCardDTO
  {
    [DataMember]
    public Int64 ScoreCardId { get; set; }
    [DataMember]
    public BUR_EnquiryDTO Enquiry { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public bool IsNewClient { get; set; }
    [DataMember]
    public int Score { get; set; }
    [DataMember]
    public bool Passed { get; set; }
    [DataMember]
    public ACC_ScoreRiskLevelDTO ScoreRiskLevel { get; set; }
    [DataMember]
    public DateTime ExpiryDate { get; set; }
    [DataMember]
    public DateTime CreatedDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
  }
}
