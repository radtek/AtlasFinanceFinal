using System;
using System.Xml.Serialization;


namespace AvsEngineLight.Handlers.Xds.Xml
{  
  [XmlTypeAttribute(AnonymousType = true)]
  [XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class NoResult
  {    
    public string NotFound { get; set; }

   
    public string Error { get; set; }

  }
}