using System;
using System.Collections.Generic;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class GetFingersResponse
  {
    public GetFingersResponse(ICollection<int> fingerIds)
    {
      FingerIds = fingerIds;
    }


    public ICollection<int> FingerIds { get; set; }

  }
}
