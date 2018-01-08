using System;
using System.Xml.Serialization;

namespace AvsEngineLight.Handlers.NuCard.Xml
{
  /// <remarks/>
  [XmlType(AnonymousType = true)]
  public partial class reportAccountDetailHeader
  {    
    public string card_acceptor { get; set; }
      
    public string user_id { get; set; }
      
    public string reqConfirm { get; set; }
       
    public string recieveBank { get; set; }
     
    public string recieveBranch { get; set; }
      
    public string recieveAccno { get; set; }
      
    public string accType { get; set; }
       
    public string idno { get; set; }
     
    public string initials { get; set; }
      
    public string name { get; set; }
       
    public string accDebits { get; set; }
       
    public string accCredits { get; set; }

    public string accLenght { get; set; }
    
    public string datetime_stamp { get; set; }
  }
}
