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
  public class STR_TransactionStatusDTO
  {
    [DataMember]
    public int TransactionStatusId { get; set; }
    [DataMember]
    public Enumerators.Stream.TransactionStatus Status { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}