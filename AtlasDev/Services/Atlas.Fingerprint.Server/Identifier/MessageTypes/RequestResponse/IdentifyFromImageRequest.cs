using System;
using System.Collections.Generic;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  /// <summary>
  /// Fingerprint Identification request
  /// </summary>
  public class IdentifyFromImageRequest
  {
    public IdentifyFromImageRequest(DateTime timestamp,  ICollection<byte[]> images, int securityLevel)
    {
      Timestamp = timestamp;
      Images = images;
      SecurityLevel = securityLevel;
    }


    /// <summary>
    /// When the request was created
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// The compressed, raw image
    /// </summary>
    public ICollection<byte[]> Images { get; set; }

    /// <summary>
    /// THe matching level- 1-7
    /// </summary>
    public int SecurityLevel { get; set; }

  }
}