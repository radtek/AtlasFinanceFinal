using System;
using System.Xml.Serialization;


namespace AvsEngineLight.Handlers.NuCard.Xml
{
  [XmlType(AnonymousType = true)]
  public class reportAccountDetailResult
  {
    public string request_id { get; set; }

    public string reference { get; set; }

    public int respDate { get; set; }

    public int respTime { get; set; }

    public int respCount { get; set; }

    public int respCode { get; set; }

    public string respDesc { get; set; }

    public string accFound { get; set; }

    public string accOpen { get; set; }

    public string accTypeMatch { get; set; }

    public string idMatch { get; set; }

    public string initMatch { get; set; }

    public string nameMatch { get; set; }

    public string accDebits { get; set; }

    public string accCredits { get; set; }

    public string accLenghtmatch { get; set; }
  }
}
