using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Atlas.NuCard.WCF.Interface
{
  /// <summary>
  /// Small class related to storing information about an ASS NuCard batch transfer
  /// </summary>
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/AtlasServer.WCF.Interface")]
  public class NuCardBatchToDispatch
  {
    [DataMember]
    public string DestinationBranchNum { get; set; }

    [DataMember]
    public List<NuCardStockItem> Cards { get; set; }

    [DataMember]
    public string SentByOperatorId { get; set; }

    [DataMember]
    public int DeliveryMechanismId { get; set; }

    [DataMember]
    public string SentViaPersonIdOrPassport { get; set; }

    [DataMember]
    public string SentViaCompanyName { get; set; }

    [DataMember]
    public string CourierOrPersonReference { get; set; }

    [DataMember]
    public string Comment { get; set; }

    [DataMember]
    public DateTime ETA { get; set; }
  }
}
