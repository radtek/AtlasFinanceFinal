using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class STR_TransactionDTO
  {
    [DataMember]
    public long TransactionId { get; set; }
    [DataMember]
    public STR_AccountDTO Account { get; set; }
    [DataMember]
    public long Reference { get; set; }
    [DataMember]
    public DateTime TransactionDate { get; set; }
    [DataMember]
    public STR_TransactionTypeDTO TransactionType { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
    [DataMember]
    public int InstalmentNumber { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
