using System;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class CheckAnyCImagesMatchResponse
  {
    public CheckAnyCImagesMatchResponse(int fingerId, string errorMessage)
    {
      FingerId = fingerId;
      ErrorMessage = errorMessage;
    }


    public int FingerId { get; set; }

    public string ErrorMessage { get; set; }

  }
}
