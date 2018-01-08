using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  /// <summary>
  /// Persons roles- from Per_Person
  /// </summary>
  [DataContract]
  public class PersonRoleDTO
  {
    [DataMember]
    public Int64 RoleTypeId { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public int Level { get; set; }
  }
}
