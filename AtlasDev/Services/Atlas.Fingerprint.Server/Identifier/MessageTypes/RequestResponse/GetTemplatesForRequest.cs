using System;


namespace Atlas.FP.Identifier.MessageTypes.RequestResponse
{
  public class GetTemplatesForRequest
  {
    public GetTemplatesForRequest(Int64 personId)
    {
      PersonId = personId;
    }


    public Int64 PersonId { get; set; }

  }
}
