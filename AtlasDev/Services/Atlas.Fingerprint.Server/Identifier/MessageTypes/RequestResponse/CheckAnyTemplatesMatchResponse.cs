
using System;

namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class CheckAnyTemplatesMatchResponse
  {
    public CheckAnyTemplatesMatchResponse(int fingerId, string errorMessage)
    {
      FingerId = fingerId;
      ErrorMessage = errorMessage;
    }


    public int FingerId { get; set; }

    public string ErrorMessage { get; set; }

  }
}
