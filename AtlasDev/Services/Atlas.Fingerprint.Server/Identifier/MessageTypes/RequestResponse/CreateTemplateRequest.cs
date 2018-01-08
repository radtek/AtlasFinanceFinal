using System;
using System.Collections.Generic;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class CreateTemplateRequest
  {
    public CreateTemplateRequest(ICollection<byte[]> compressedImages)
    {
      CompressedImages = compressedImages;
    }


    public ICollection<byte[]> CompressedImages { get; set;}

  }
}
