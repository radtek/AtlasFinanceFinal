using System;
using System.Runtime.Serialization;


namespace Atlas.NuCard.WCF.Interface
{
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/AtlasServer.WCF.Interface")]
  public class CardStatus
  {
    [DataMember]
    public bool Expired { get; set; }

    [DataMember]
    public bool Retired { get; set; }

    [DataMember]
    public bool Stopped { get; set; }

    [DataMember]
    public bool Stolen { get; set; }

    [DataMember]
    public bool Valid { get; set; }

    [DataMember]
    public bool Cancelled { get; set; }

    [DataMember]
    public bool Loaded { get; set; }

    [DataMember]
    public bool Lost { get; set; }

    [DataMember]
    public bool Activated { get; set; }

    [DataMember]
    public bool Redeemed { get; set; }

    [DataMember]
    public bool Empty { get; set; }

    [DataMember]
    public bool PINBlocked { get; set; }
  }
}
