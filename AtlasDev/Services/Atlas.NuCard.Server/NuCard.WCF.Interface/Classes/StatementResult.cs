using System;
using System.Runtime.Serialization;


namespace Atlas.NuCard.WCF.Interface
{
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/AtlasServer.WCF.Interface")]
  public class StatementResult
  {
    [DataMember]
    public int BalanceInCents { get; set; }

    [DataMember]
    public DateTime ExpiryDate { get; set; }

    [DataMember]
    public StatementLine[] StatementLines { get; set; }

    [DataMember]
    public string ProfileNum { get; set; }
  }
}
