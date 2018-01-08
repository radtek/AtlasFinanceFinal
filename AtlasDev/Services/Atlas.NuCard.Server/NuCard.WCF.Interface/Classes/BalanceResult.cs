using System;
using System.Runtime.Serialization;


namespace Atlas.NuCard.WCF.Interface
{
  // Return from 'Balance'
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/AtlasServer.WCF.Interface")]
  public class BalanceResult
  {
    [DataMember]
    public int BalanceInCents { get; set; }

    [DataMember]
    public DateTime ExpiryDate { get; set; }

    [DataMember]
    public bool Expired { get; set; }

    [DataMember]
    public bool Stopped { get; set; }
    [DataMember]
    public bool Stolen { get; set; }

    [DataMember]
    public bool Lost { get; set; }

    [DataMember]
    public string ProfileNum { get; set; }
  }

}
