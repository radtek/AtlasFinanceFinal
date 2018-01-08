using System;
using System.Runtime.Serialization;


namespace Atlas.NuCard.WCF.Interface
{
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/AtlasServer.WCF.Interface")]
  public class StatementLine
  {
    [DataMember]
    public DateTime TransactionDate{ get; set; }

    [DataMember]
    public int TransactionAmountInCents{ get; set; }

    [DataMember]
    public string TransactionDescription{ get; set; }

    [DataMember]
    public int TransactionType { get; set; }
  }
}
