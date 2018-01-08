using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.User")]
  public class UserDetail
  {
    [DataMember]
    public string OperatorCode { get; set; }
    [DataMember]
    public string CellularNumber { get; set; }
    [DataMember]
    public string IdNumber { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string Surname { get; set; }
  }
}
