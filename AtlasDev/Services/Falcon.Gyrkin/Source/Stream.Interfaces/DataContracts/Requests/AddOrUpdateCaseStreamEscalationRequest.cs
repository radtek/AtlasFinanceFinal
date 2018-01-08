using System;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateCaseStreamEscalationRequest
  {
    public long CaseStreamEscalationId { get; set; }
    public long CaseStreamId { get; set; }
    public Enumerators.Stream.EscalationType EscalationType { get; set; }
  }
}
