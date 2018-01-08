using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class ConfigDTO
  {
    [DataMember]
    public Int64 ConfigId { get; set; }
    [DataMember]
    public string DataEntity { get; set; }
    [DataMember]
    public int DataType { get; set; }
    [DataMember]
    public string DataSection { get; set; }
    [DataMember]
    public string DataValue { get; set; }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public PER_PersonDTO DeletedBy { get; set; }
    [DataMember]
    public PER_PersonDTO LastEditedBy { get; set; }
    [DataMember]
    public DateTime? CreatedDT { get; set; }
    [DataMember]
    public DateTime? DeletedDT { get; set; }
    [DataMember]
    public DateTime? LastEditedDT { get; set; }
  }
}
