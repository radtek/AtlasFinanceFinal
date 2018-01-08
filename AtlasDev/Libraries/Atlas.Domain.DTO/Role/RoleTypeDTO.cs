using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract]
  public class RoleTypeDTO
  {
    [DataMember]
    public Int64 RoleTypeId { get; set; }
    [DataMember]
    public Int16 Level { get; set; }
    [DataMember]
    public Enumerators.General.RoleType Type { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
