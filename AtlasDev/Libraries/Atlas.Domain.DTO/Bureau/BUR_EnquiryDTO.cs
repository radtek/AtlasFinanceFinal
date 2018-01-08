using System;
using System.Runtime.Serialization;
using System.Collections.Generic;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public sealed class BUR_EnquiryDTO
  {
    [DataMember]
    public Int64 EnquiryId { get; set; }
    [DataMember]
    public int ObjectVersion { get; set; }
    [DataMember]
    public BRN_BranchDTO Branch { get; set; }
    [DataMember]
    public Guid CorrelationId { get; set; }
    [DataMember]
    public BUR_EnquiryDTO PreviousEnquiry { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public BUR_ServiceDTO Service { get; set; }
    [DataMember]
    public Enumerators.Risk.RiskEnquiryType EnquiryType { get; set; }
    [DataMember]
    public Enumerators.Risk.RiskTransactionType TransactionType { get; set; }
    [DataMember]
    public string IdentityNum { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string LastName { get; set; }
    [DataMember]
    public bool IsSucess { get; set; }
    [DataMember]
    public DateTime EnquiryDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedUser { get; set; }
    [DataMember]
    public DateTime? CreateDate { get; set; }
    
      //[DataMember]
    //public List<BUR_StorageDTO> Storage { get; set; }
  }
}
