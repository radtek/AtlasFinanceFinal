using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.ScoreCard")]
  public class ScoreCardRequest
  {
    [DataMember]
    public string IdNumberOrPassport { get; set; }
    [DataMember]
    public bool IsPassport { get; set; }
    [DataMember]
    public DateTime DateOfBirth { get; set; }
    [DataMember]
    public string CellularNumber { get; set; }

    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string Surname { get; set; }

    // !!!!!!!!!!!!!!!!!!!!!!!! OPTIONALS !!!!!!!!!!!!!!!!!!!!!!!!
    [DataMember]
    public string Address1 { get; set; }
    [DataMember]
    public string Address2 { get; set; }
    [DataMember]
    public string Address3 { get; set; }
    [DataMember]
    public string Address4 { get; set; }
    [DataMember]
    public string AddressPO { get; set; }

    [DataMember]
    public string TelephoneNumber { get; set; }
    [DataMember]
    public string TelephoneAreaCode { get; set; }
  }
}
