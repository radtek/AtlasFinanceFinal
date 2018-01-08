using System;
using System.Runtime.Serialization;
using Atlas.Common.Extensions;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class PER_RelationTypeDTO
  {
    [DataMember]
    public Int64 RelationTypeId { get; set; }
    [DataMember]
    public Enumerators.General.RelationType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.RelationType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.RelationType>(); }
    }
    [DataMember]
    public string Description { get; set; }
  }
}
