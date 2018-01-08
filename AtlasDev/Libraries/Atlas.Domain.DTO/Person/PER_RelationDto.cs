using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PER_RelationDTO
  {
    [DataMember]
    public Int64 PersonRelationId { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
    [DataMember]
    public PER_RelationTypeDTO Relation { get; set; }
    [DataMember]
    public PER_PersonDTO RelationPerson { get; set; }
  }
}
