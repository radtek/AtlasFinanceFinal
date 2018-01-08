using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class BUR_AccountsDTO
  {
    [DataMember]
    public Int64 BureauAccountId { get; set; }

    [DataMember]
    public ACC_AccountDTO Account { get; set; }

    [DataMember]
    public Enumerators.Risk.BureauAccountType Type { get; set; }

    [DataMember]
    public BUR_EnquiryDTO Enquiry { get; set; }

    [DataMember]
    public Enumerators.Risk.BureauAccountType AccountSource { get; set; }

    [DataMember]
    public BUR_AccountTypeCodeDTO AccountType { get; set; }

    [DataMember]
    public string Subscriber { get; set; }

    [DataMember]
    public string AccountNo { get; set; }

    [DataMember]
    public string SubAccountNo { get; set; }

    [DataMember]
    public BUR_AccountStatusCodeDTO AccountStatusCode { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public int? JointParticipants { get; set; }

    [DataMember]
    public string Period { get; set; }

    [DataMember]
    public string PeriodType { get; set; }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public DateTime? OpenDate { get; set; }

    [DataMember]
    public Decimal? Instalment { get; set; }

    [DataMember]
    public Decimal? OpenBalance { get; set; }

    [DataMember]
    public Decimal? CurrentBalance { get; set; }

    [DataMember]
    public Decimal? OverdueAmount { get; set; }

    [DataMember]
    public DateTime? BalanceDate { get; set; }

    [DataMember]
    public DateTime? LastPayDate { get; set; }

    [DataMember]
    public DateTime? StatusDate { get; set; }

    [DataMember]
    public DateTime? CreatedDate { get; set; }

    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }

    [DataMember]
    public DateTime? OverrideDate { get; set; }

    [DataMember]
    public PER_PersonDTO OverrideUser { get; set; }
  }
}