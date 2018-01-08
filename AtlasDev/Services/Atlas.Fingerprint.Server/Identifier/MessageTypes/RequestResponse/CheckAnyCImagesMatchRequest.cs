using System;
using System.Collections.Generic;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class CheckAnyCImagesMatchRequest
  {
    public CheckAnyCImagesMatchRequest(ICollection<byte[]> compressedImages, ICollection<Tuple<int, byte[]>> templates, int securityLevel)
    {
      CompressedImages = compressedImages;
      Templates = templates;
      SecurityLevel = securityLevel;
    }


    /// <summary>
    /// Compressed image(s)
    /// </summary>
    public ICollection<byte[]> CompressedImages { get; set; }

    /// <summary>
    /// Templates to match against
    /// </summary>
    public ICollection<Tuple<int, byte[]>> Templates { get; set; }

    /// <summary>
    /// Matching security level to use
    /// </summary>
    public int SecurityLevel { get; set;}

  }
}
