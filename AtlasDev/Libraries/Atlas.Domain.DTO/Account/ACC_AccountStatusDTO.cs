using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_AccountStatusDTO
  {
    [DataMember]
    public Int64 AccountStatusId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public ACC_StatusDTO Status { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}