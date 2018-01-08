
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

  [Serializable]
  [DataContract(IsReference = true)]
  public class BUR_BatchDTO
  {
    [DataMember]
    public Int64 BatchId { get;set;}
    [DataMember]
    public BRN_BranchDTO Branch { get;set;}
    [DataMember]
    public DateTime? DeliveryDate { get;set;}
    [DataMember]
    public PER_PersonDTO CreateUser {get;set;}
    [DataMember]
    public DateTime? CreatedDate { get; set; }
  }
}
