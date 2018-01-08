using System.Runtime.Serialization;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class BankBranchDTO
  {
    [DataMember]
    public Int64 BranchId { get; set; }
    [DataMember]

    public BankDTO Bank { get; set; }
    [DataMember]

    public string BranchCode { get; set; }
  }
}
