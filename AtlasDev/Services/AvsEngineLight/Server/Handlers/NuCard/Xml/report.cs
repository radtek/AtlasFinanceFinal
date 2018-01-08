using System;
using System.Xml.Serialization;


namespace AvsEngineLight.Handlers.NuCard.Xml
{ 
  [XmlType(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class report
  {   
    public reportAccountDetailHeader accountDetailHeader { get; set; }
        
    public reportAccountDetailResult accountDetailResult { get; set; }

    public reportReportError ReportError { get; set; }
  }

}
