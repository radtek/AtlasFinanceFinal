using System;
using System.Runtime.Serialization;


namespace Atlas.ThirdParty.CS.WCF.Interface
{
  [DataContract]
  public class ScorecardV2Result
  {
    [DataMember]
    public bool Successful { get; set; }
   
    [DataMember]
    public string UserDisplayFileBase64 { get; set; }
    
    [DataMember]
    public string ScorecardXmlFileBase64 { get; set; }
    
    [DataMember]
    public string ErrorMessage { get; set; }

    [DataMember]
    public Int64 EnquiryId { get; set; }
  }
}
