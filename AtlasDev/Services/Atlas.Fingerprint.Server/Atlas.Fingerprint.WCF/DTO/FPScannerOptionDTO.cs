using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  /// <summary>
  /// Fingerprint Scanner options
  /// </summary>
  [DataContract]
  public class FPScannerOptionDTO
  {
    /// <summary>
    /// Minimum number of fingers to capture
    /// </summary>
    [DataMember]
    public int MinFingers { get; set; }

    /// <summary>
    /// Absolute minimum level for image acceptance
    /// </summary>
    [DataMember]
    public int MinQuality { get; set; }

    [DataMember]
    public int NFIQMin { get; set; }

    [DataMember]
    public int AcceptedQualityMin { get; set; }

    [DataMember]
    public bool DetectCore { get; set; }
  }
}
