using System.Runtime.Serialization;
using System;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_BatchDTO
  {
    [DataMember]
    public Int64 BatchId { get; set; }
    [DataMember]
    public DBT_BatchStatusDTO BatchStatus { get; set; }
    [DataMember]
    public DateTime? LastStatusDate { get; set; }
    [DataMember]
    public DateTime? SubmitDate { get; set; }
    [DataMember]
    public PER_PersonDTO SubmitUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}