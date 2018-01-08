using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ACC_DebitControlDTO
  {
    [DataMember]
    public Int64 DebitControlId { get; set; }
    [DataMember]
    public ACC_AccountDTO Account { get; set; }
    [DataMember]
    public DBT_ControlDTO Control { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}