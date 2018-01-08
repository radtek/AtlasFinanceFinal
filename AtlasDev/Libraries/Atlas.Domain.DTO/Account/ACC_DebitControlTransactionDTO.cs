using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO.Account
{
  [Serializable]
  [DataContract(IsReference=true)]
  public class ACC_DebitControlTransactionDTO
  {
    [DataMember]
    public Int64 DebitControlTransactionId { get; set; }
    [DataMember]
    public ACC_DebitControlDTO DebitControl { get; set; }
    [DataMember]
    public DBT_TransactionDTO DebitTransaction { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public DateTime? DatePostedToLedger { get; set; }
  }
}