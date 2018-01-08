using System;
using System.Xml.Serialization;


namespace AvsEngineLight.Handlers.Xds.Xml
{
  /// <remarks/>
  [XmlTypeAttribute(AnonymousType = true)]
  [XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class AccountVerification
  {   
    public string Status { get; set; }
        
    public string ReferenceNo { get; set; }

  }
}