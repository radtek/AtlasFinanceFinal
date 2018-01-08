using System;


namespace Atlas.Domain.Serializable
{
  [Serializable]
  public class NuCardAdditionalLogInfo
  {
    // Request parameters- JSON encoded
    public string SourceRequestParam { get; set; }
    // Request started
    public DateTime StartDT { get; set; }
    // Request ended
    public DateTime EndDT { get; set; }
    // The XML request
    public string XMLRequest { get; set; }
    // The XML result
    public string XMLResponse { get; set; }
  }
}