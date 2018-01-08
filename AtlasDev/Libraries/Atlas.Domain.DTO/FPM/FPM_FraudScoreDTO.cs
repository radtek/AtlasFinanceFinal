using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_FraudScoreDTO
  {
    [DataMember]
    public Int64 FraudScoreId { get; set; }
    [DataMember]
    public BUR_EnquiryDTO Enquiry { get; set; }
    [DataMember]
    public string RecordSeq { get; set; }
    [DataMember]
    public string Part { get; set; }
    [DataMember]
    public string PartSeq { get; set; }
    [DataMember]
    public string Rating { get; set; }
    [DataMember]
    public string RatingDescription { get; set; }
    [DataMember]
    public string IDNumber { get; set; }
    [DataMember]
    public string BankAccountNo { get; set; }
    [DataMember]
    public string CellNo { get; set; }
    [DataMember]
    public bool Passed { get; set; }
    [DataMember]
    public DateTime? OverrideDate { get; set; }
    [DataMember]
    public PER_PersonDTO OverrideUser { get; set; }
    [DataMember]
    public string OverrideReason { get; set; }
    [DataMember]
    public DateTime? CreatedDate { get; set; }
  }
}
