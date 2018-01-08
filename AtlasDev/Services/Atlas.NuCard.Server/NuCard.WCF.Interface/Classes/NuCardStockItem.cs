using System;
using System.Runtime.Serialization;


namespace Atlas.NuCard.WCF.Interface
{
  /// <summary>
  /// Small class to transfer key NuCard fields
  /// </summary>
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/AtlasServer.WCF.Interface")]
  public class NuCardStockItem
  {
    [DataMember]
    public Int64 NuCardId { get; set; }

    [DataMember]
    public string TrackingNumber { get; set; }

    [DataMember]
    public string FullCardNumber { get; set; }

    [DataMember]
    public DateTime ExpiryDT { get; set; }

    [DataMember]
    public DateTime IssueDT { get; set; }

    [DataMember]
    public Int64 CardStatus { get; set; }

  }
}
