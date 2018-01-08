using System.Runtime.Serialization;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class BankCDVDTO
  {
    [DataMember]
    public Int64 CDVId { get; set; }
    [DataMember]
    public BankDTO Bank { get; set; }
    [DataMember]
    public BankAccountTypeDTO AccountType { get; set; }
    [DataMember]
    public string Weighting { get; set; }
    [DataMember]
    public byte FudgeFactor { get; set; }
    [DataMember]
    public byte Modulus { get; set; }
    [DataMember]
    public char ExceptCode { get; set; }
  }
}