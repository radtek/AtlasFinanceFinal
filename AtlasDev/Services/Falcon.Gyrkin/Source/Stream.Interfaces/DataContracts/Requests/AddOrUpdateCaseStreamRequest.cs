using System;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateCaseStreamRequest
  {
    public long CaseStreamId { get; set; }
    public long CaseId { get; set; }
    public Enumerators.Stream.EscalationType EscalationType { get; set; }
    public Enumerators.Stream.StreamType StreamType { get; set; }
    public Enumerators.Stream.PriorityType PriorityType { get; set; }
    public DateTime LastPriorityDate { get; set; }
    public long CompletedUserId { get; set; }
    public long CompleteCommentId { get; set; }
    public string CompleteNote { get; set; }
    public DateTime? CompleteDate { get; set; }
    public long CreateUserId { get; set; }
  }
}
