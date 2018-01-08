using System;
using System.Xml.Serialization;


namespace AvsEngineLight.Handlers.NuCard.Xml
{
  /// <remarks/>
  [XmlType(AnonymousType = true)]
  public partial class reportReportError
  {  
    public string error_code { get; set; }
       
    public string error_date { get; set; }
       
    public string error_msg { get; set; }
    
    public string error_detail { get; set; }
  }
}
