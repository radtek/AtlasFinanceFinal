using System;

using Atlas.Enumerators;


namespace Atlas.WCF.FPServer.ClientState
{
  /// <summary>
  /// Class maintaining the state information for local GUI/LMS communication requests
  /// </summary>
  public class FPGuiRequest
  {
    public FPGuiRequest(string requestId, Biometric.RequestStatus status, DateTime started,
      Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3,
      string errorMessage, int timeoutSecs, byte[] compressedImage, bool webRequest)
    {
      RequestId = requestId;
      PersonId = personId;
      UserPersonId = userPersonId;
      AdminPersonId = adminPersonId;
      Status = status;
      Started = started;
      Message1 = message1;
      Message2 = message2;
      Message3 = message3;
      ErrorMessage = errorMessage;
      TimeoutSecs = timeoutSecs;
      if (compressedImage != null)
      {
        CompressedImage = new byte[compressedImage.Length];
        Array.Copy(compressedImage, CompressedImage, compressedImage.Length);
      }
      WebRequest = webRequest;
    }


    /// <summary>
    /// GUI request
    /// </summary>
    public string RequestId { get; private set; }

    /// <summary>
    /// DB Match: PER_Person.PersonId
    /// </summary>    
    public Int64 PersonId { get; set; }

    /// <summary>
    /// Status of the request
    /// </summary>
    public Biometric.RequestStatus Status { get; set; }

    /// <summary>
    /// Date/time request started
    /// </summary>
    public DateTime Started { get; private set; }

    /// <summary>
    /// Custom text 1
    /// </summary>
    public string Message1 { get; private set; }

    /// <summary>
    /// Custom text 2
    /// </summary>
    public string Message2 { get; private set; }

    /// <summary>
    /// Custom text 3
    /// </summary>
    public string Message3 { get; private set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Request time-out in seconds
    /// </summary>
    public int TimeoutSecs { get; private set; }

    /// <summary>
    /// User SecurityID
    /// </summary>
    public Int64 UserPersonId { get; set; }

    public Int64 AdminPersonId { get; set; }


    public byte[] CompressedImage { get; set; }


    public bool WebRequest { get; set; }

    /// <summary>
    /// Make deep copy of instance
    /// </summary>
    /// <returns>New deep copied instance</returns>
    internal FPGuiRequest DeepCopy()
    {
      return new FPGuiRequest(RequestId, Status, Started, PersonId, UserPersonId, AdminPersonId, Message1, Message2, Message3, ErrorMessage, TimeoutSecs, CompressedImage, WebRequest);
    }
  }
}
