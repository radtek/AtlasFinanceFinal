using System;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Atlas.ThirdParty.CS.WCF.Interface
{
  [DataContract]
  public class AtlasProduct
  {
    [DataMember]
    public string ProductType { get; set; }

    [DataMember]
    public string ProductDescription { get; set; }

    [DataMember]
    public List<string> Reasons { get; set; }

    [DataMember]
    public bool Outcome { get; set; }
  }
}
