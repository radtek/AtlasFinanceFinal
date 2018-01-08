using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Atlas.ThirdParty.CS.WCF.Interface
{
  [DataContract]
  public class ScorecardSimpleResult
  {
    [DataMember]
    public bool Successful { get; set; }
       
    [DataMember]
    public int Score { get; set; }

    [DataMember]
    public List<AtlasProduct> AtlasProducts;

    [DataMember]
    public string Error { get; set; }

    [DataMember]
    public Int64 EnquiryId { get; set; }

  }
}
