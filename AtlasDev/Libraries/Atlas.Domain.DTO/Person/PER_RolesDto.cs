using System;
using System.Runtime.Serialization;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class PER_RoleDTO
  {
    [DataMember]
    public Int64 PersonRoleId { get; set; }
    [DataMember]
    public RoleTypeDTO RoleType { get; set; }
    [DataMember]
    public PER_PersonDTO Person { get; set; }
  }
}
