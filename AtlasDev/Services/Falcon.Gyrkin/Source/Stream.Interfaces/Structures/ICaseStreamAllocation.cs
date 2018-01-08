using System;

namespace Stream.Framework.Structures
{
  public interface ICaseStreamAllocation
  {
    long CaseStreamAllocationId { get; set; }
    long CaseStreamId { get; set; }
    Enumerators.Stream.EscalationType EscalationType { get; set; }
    string AllocatedUser { get; set; }
    long AllocatedUserId { get; set; }
    DateTime AllocatedDate { get; set; }
    int NoActionCount { get; set; }
    bool TransferredIn { get; set; }
    DateTime? TransferredOutDate { get; set; }
    bool TransferredOut { get; set; }
    int SmsCount { get; set; }
    DateTime? CompleteDate { get; set; }
    string CompleteComment { get; set; }
  }
}