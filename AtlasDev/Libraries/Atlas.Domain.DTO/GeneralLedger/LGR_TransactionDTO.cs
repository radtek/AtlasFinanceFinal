using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class LGR_TransactionDTO
  {
    [DataMember]
    public long TransactionId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public DateTime? CalculatedArrearsDate { get; set; }
    [DataMember]
    public LGR_TransactionTypeDTO TransactionType { get; set; }
    [DataMember]
    public LGR_FeeDTO Fee { get; set; }
    [DataMember]
    public LGR_TypeDTO Type { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
    [DataMember]
    public DateTime TransactionDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
    [DataMember]
    public DateTime? AccPacExportDate { get; set; }
  }
}