using System;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  /// <summary>
  /// Fingerprint Identification response
  /// </summary>
  public class IdentifyFromImageResponse
  {
    public IdentifyFromImageResponse(Int64 personId, int fingerId, Int64 milliseconds, int score)
    {
      PersonId = personId;
      FingerId = fingerId;
      Milliseconds = milliseconds;
      Score = score;
    }


    public Int64 PersonId { get; set; }

    public int FingerId { get; set; }

    public Int64 Milliseconds { get; set; }

    public int Score { get; set; }

  }
}