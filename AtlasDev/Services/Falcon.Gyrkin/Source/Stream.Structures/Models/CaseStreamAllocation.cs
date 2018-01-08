using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class CaseStreamAllocation : ICaseStreamAllocation
  {
    public long CaseStreamAllocationId { get; set; }
    public long CaseStreamId { get; set; }
    public Framework.Enumerators.Stream.EscalationType EscalationType { get; set; }
    public string AllocatedUser { get; set; }
    public long AllocatedUserId { get; set; }
    public DateTime AllocatedDate { get; set; }
    public int NoActionCount { get; set; }
    public bool TransferredIn { get; set; }
    public DateTime? TransferredOutDate { get; set; }
    public bool TransferredOut { get; set; }
    public int SmsCount { get; set; }
    public DateTime? CompleteDate { get; set; }
    public string CompleteComment { get; set; }
  }
}