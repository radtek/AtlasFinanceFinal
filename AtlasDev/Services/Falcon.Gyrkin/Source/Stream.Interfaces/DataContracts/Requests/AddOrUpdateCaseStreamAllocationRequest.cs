using System;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateCaseStreamAllocationRequest
  {
    public long CaseStreamAllocationId { get; set; }
    public long CaseStreamId { get; set; }
    public Enumerators.Stream.EscalationType EscalationType { get; set; }
    public long AllocatedUserId { get; set; }
    public DateTime AllocatedDate { get; set; }
    public int NoActionCount { get; set; }
    public bool TransferredIn { get; set; }
    public DateTime? TransferredOutDate { get; set; }
    public bool TransferredOut { get; set; }
    public int SmsCount { get; set; }
    public DateTime? CompleteDate { get; set; }
    public long CompleteCommentId { get; set; }
  }
}
