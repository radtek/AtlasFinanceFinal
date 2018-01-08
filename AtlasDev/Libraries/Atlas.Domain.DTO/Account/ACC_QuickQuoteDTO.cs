using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_QuickQuoteDTO
  {
    [DataMember]
    public Int64 QuickQuoteId { get; set; }
    [DataMember]
    public string IdNumber { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string Surname { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public Decimal AppliedAmount { get; set; }
    [DataMember]
    public int Period { get; set; }
    [DataMember]
    public ACC_PeriodFrequencyDTO PeriodFrequency { get; set; }
    [DataMember]
    public BUR_EnquiryDTO Enquiry { get; set; }
    [DataMember]
    public string HomeNo1 { get; set; }
    [DataMember]
    public string HomeNo2 { get; set; }
    [DataMember]
    public string WorkNo1 { get; set; }
    [DataMember]
    public string WorkNo2 { get; set; }
    [DataMember]
    public string CellNo1 { get; set; }
    [DataMember]
    public string CellNo2 { get; set; }
    [DataMember]
    public string Email { get; set; }
    [DataMember]
    public bool IsLead { get; set; }
    [DataMember]
    public CPY_CompanyDTO LeadCompany { get; set; }

  }
}
