using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PYT_BatchDTO
  {
    [DataMember]
    public long BatchId { get; set; }
    [DataMember]
    public PYT_ServiceDTO Service { get; set; }
    [DataMember]
    public PYT_BatchStatusDTO BatchStatus { get; set; }
    [DataMember]
    public DateTime LastStatusDate { get; set; }
    [DataMember]
    public PER_PersonDTO AuthoriseUser { get; set; }
    [DataMember]
    public DateTime? AuthoriseDate { get; set; }
    [DataMember]
    public PER_PersonDTO SubmitUser { get; set; }
    [DataMember]
    public DateTime? SubmitDate { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}
