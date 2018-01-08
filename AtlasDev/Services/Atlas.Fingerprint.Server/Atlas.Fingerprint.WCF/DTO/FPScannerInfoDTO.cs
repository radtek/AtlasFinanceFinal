using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  /// <summary>
  /// Returns information about the fingerprint scanner
  /// </summary>
  [DataContract]
  public class FPScannerInfoDTO
  {
    [DataMember]
    public string Make { get; set; }

    [DataMember]
    public string Model { get; set; }

    [DataMember]
    public string Serial { get; set; }

    [DataMember]
    public string Description { get; set; }
  }
}
