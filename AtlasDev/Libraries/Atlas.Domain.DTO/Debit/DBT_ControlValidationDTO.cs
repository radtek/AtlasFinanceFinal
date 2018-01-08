using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_ControlValidationDTO
  {
    [DataMember]
    public int ControlValidationId { get; set; }
    [DataMember]
    public DBT_ControlDTO Control { get; set; }
    [DataMember]
    public DBT_ValidationDTO Validation { get; set; }
  }
}