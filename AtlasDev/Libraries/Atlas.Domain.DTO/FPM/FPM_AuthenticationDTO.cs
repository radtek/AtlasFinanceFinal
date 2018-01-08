
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_AuthenticationDTO
  {
    [DataMember]
    public Int64 AuthenticationId { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public BankDetailDTO BankDetail { get; set; }
    [DataMember]
    public ContactDTO Contact { get; set; }
    [DataMember]
    public bool Authenticated { get; set; }
    [DataMember]
    public bool Completed { get; set; }
    [DataMember]
    public string QuestionCount { get; set; }
    [DataMember]
    public decimal? AuthenticatedPercentage { get; set; }
    [DataMember]
    public decimal? RequiredPercentage { get; set; }
    [DataMember]
    public string Reference { get; set; }
    [DataMember]
    public bool Enabled { get; set; }
    [DataMember]
    public PER_PersonDTO OverridePerson { get; set; }
    [DataMember]
    public DateTime? OverrideDate { get; set; }
    [DataMember]
    public string OverrideReason { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public DateTime? CreateDate { get; set; }
  }
}
